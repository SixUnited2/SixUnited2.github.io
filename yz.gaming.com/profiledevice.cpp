#include "pch.h"
#include "profiledevice.h"
#include "profile.h"
#include "inc/contract.h"
#include "inc/wpptrace.h"
#include "profiledevice.tmh"

namespace YZ::Gaming::Com
{
	////////////////////////////// ProfileDevice class ///////////////////////////
	ProfileDevice::ProfileDevice(wstring const& deviceName, PFN_PROFILEDEVICEEVENTHANDLER pfnProfileDeviceEventHandler, LPVOID lpContext) :
		m_pfnProfileDeviceEventHandler(pfnProfileDeviceEventHandler),
		m_lpContext(lpContext)
    {
        auto_handle<HANDLE> h(::CreateFile(deviceName.c_str(),
                                           GENERIC_READ | GENERIC_WRITE,
                                           FILE_SHARE_READ | FILE_SHARE_WRITE,
                                           nullptr,
                                           OPEN_EXISTING,
										   FILE_FLAG_OVERLAPPED,
                                           nullptr));
        if (!h)
        {
            DWORD dwError = ::GetLastError();

            WPPTrace(TRACE_LEVEL_INFORMATION,
                     PROFILE,
                     L"Openning the hid device: %ws failed due to %!WINERROR!",
                     deviceName.c_str(),
                     dwError);
            Check(false);
        }
        else
        {
            HIDD_ATTRIBUTES hiddAttr{};
            hiddAttr.Size = sizeof(HIDD_ATTRIBUTES);

            if (BOOL result = ::HidD_GetAttributes(h.get(), &hiddAttr);
                result == TRUE)
            {
                Check((hiddAttr.VendorID == Constants::DeviceID::VID) &&
                      (hiddAttr.ProductID == Constants::DeviceID::PID));
                
                PHIDP_PREPARSED_DATA pHidpPreparsedData = nullptr;
                if (result = ::HidD_GetPreparsedData(h.get(), &pHidpPreparsedData);
                    result == TRUE)
                {
                    HIDP_CAPS hidpCaps;
                    ::HidP_GetCaps(pHidpPreparsedData, &hidpCaps);
                    ::HidD_FreePreparsedData(pHidpPreparsedData);

                    Check((hidpCaps.Usage == Constants::DeviceID::USAGE) &&
                          (hidpCaps.UsagePage == Constants::DeviceID::USAGE_PAGE));
                    
                    WPPTrace(TRACE_LEVEL_INFORMATION, PROFILE, L"The profile device is found: %ws", deviceName.c_str());
                    m_ahProfileDevice = h;

					m_pHidIOCompletion = new HidIOCompletion(hidpCaps.InputReportByteLength, this);
                }
                else
                {
                    DWORD dwError = ::GetLastError();

                    WPPTrace(TRACE_LEVEL_INFORMATION,
                             PROFILE,
                             L"Failed in calling HidD_GetPreparsedData for device %ws due to %!WINERROR!",
                             deviceName.c_str(),
                             dwError);
                    Check(false);
                }
            }
            else
            {
                WPPTrace(TRACE_LEVEL_INFORMATION, PROFILE, L"Failed in calling HidD_GetAttributes for device %ws", deviceName.c_str());
                Check(false);
            }
        }
    }

    bool ProfileDevice::IsHandleOwned(HANDLE hProfileDevice) const noexcept
    {
        return m_ahProfileDevice.get() == hProfileDevice;
    }

    void ProfileDevice::OnDeviceAdded(shared_ptr<HotplugBulletin> hotplugBulletin)
    {
        hotplugBulletin->RegisterDevice(m_ahProfileDevice.get(), &m_hDeviceNotification);

		// start next read
		m_pHidIOCompletion->GetDevice()->GetNextReport();
    }

    void ProfileDevice::OnDeviceRemoved()
    {
		::CancelIoEx(m_ahProfileDevice.get(), m_pHidIOCompletion->GetOverlappedIO());

        if (m_hDeviceNotification != nullptr)
        {
            HotplugBulletin::UnregisterDevice(m_hDeviceNotification);
            m_hDeviceNotification = nullptr;
        }
    }

    bool ProfileDevice::ReadProfileData(BYTE modeProfileNumber, Profile& profile, DWORD& fwVersion)
	{
		bool result = true;

		try
		{
			//预备读命令，发送后下位机应答固件版本等相关信息
			BYTE cmd = static_cast<BYTE>(ProfileCmd::R_INFO);

			BYTE requestWriteBuf[64];
			memset(requestWriteBuf, 0, sizeof(requestWriteBuf));
			requestWriteBuf[0] = REPORTID_1;
			requestWriteBuf[1] = 0xA5;
			requestWriteBuf[2] = cmd;//准备读Profile数据命令前获取版本号
			requestWriteBuf[3] = ~0xA5;
			requestWriteBuf[4] = ~cmd;
			requestWriteBuf[5] = modeProfileNumber;

			//获取下位机应答固件版本等相关信息
			if (::HidD_SetOutputReport(m_ahProfileDevice.get(), &requestWriteBuf, sizeof(requestWriteBuf)) == FALSE)
			{
				DWORD dwRetVal = ::GetLastError();
				WPPTrace(TRACE_LEVEL_ERROR, PROFILE, L"Failed in calling HidD_SetOutputReport for the firmware version due to %!WINERROR!", dwRetVal);
				Check(false);
			}
			::Sleep(5);

			BYTE rdBuf[64] = { 0 };
			memset(rdBuf, 0, sizeof(rdBuf));
			rdBuf[0] = REPORTID_1;
			do
			{
				if (::HidD_GetInputReport(m_ahProfileDevice.get(), &rdBuf, sizeof(rdBuf)) == FALSE)
				{
					DWORD dwRetVal = ::GetLastError();
					WPPTrace(TRACE_LEVEL_ERROR, PROFILE, L"Failed in calling HidD_GetInputReport for the firmware version due to %!WINERROR!", dwRetVal);
					Check(false);
				}
				::Sleep(5);
			} while (rdBuf[8] != 0xAA);

			wchar_t firmwareVersion[Constants::StringLength::MAX_SMALL_STRING] = { 0 };
			::StringCchPrintf(firmwareVersion,
				              Constants::StringLength::MAX_SMALL_STRING,
				              L"Ver:%X.%02X.%02X.%02X",
							  rdBuf[10],
						      rdBuf[9],
				              rdBuf[12],
				              rdBuf[11]);
			WPPTrace(TRACE_LEVEL_INFORMATION, PROFILE, L"The profile version: %ws", firmwareVersion);
			fwVersion = MAKELONG(MAKEWORD(rdBuf[10], rdBuf[9]), MAKEWORD(rdBuf[12], rdBuf[11]));

			//获取Profile数据
			auto profileSize = sizeof(Profile);
			int nPage = static_cast<int>(profileSize / 32);
            auto remainder = profileSize % 32;
            if (remainder != 0)
            {
                nPage++;
            }

			LPBYTE dataBuffer = reinterpret_cast<LPBYTE>(profile.data);
			for (int i = 0; i < nPage; i++)
			{
				memset(requestWriteBuf, 0, sizeof(requestWriteBuf));
				cmd = static_cast<BYTE>(ProfileCmd::R_PF_DATA);//读Profile数据命令
				requestWriteBuf[0] = REPORTID_1;
				requestWriteBuf[1] = 0xA5;
				requestWriteBuf[2] = cmd;
				requestWriteBuf[3] = ~0xA5;
				requestWriteBuf[4] = ~cmd;
				requestWriteBuf[6] = i;

				if (::HidD_SetOutputReport(m_ahProfileDevice.get(), &requestWriteBuf, sizeof(requestWriteBuf)) == FALSE)
				{
					DWORD dwRetVal = ::GetLastError();
					WPPTrace(TRACE_LEVEL_ERROR, PROFILE, L"Failed in calling HidD_SetOutputReport for the profile data due to %!WINERROR!", dwRetVal);
					Check(false);
				}
				::Sleep(5);

				memset(rdBuf, 0, sizeof(rdBuf));
				rdBuf[0] = REPORTID_1;
				if (::HidD_GetInputReport(m_ahProfileDevice.get(), &rdBuf, sizeof(rdBuf)) == FALSE)
				{
					DWORD dwRetVal = ::GetLastError();
					WPPTrace(TRACE_LEVEL_ERROR, PROFILE, L"Failed in calling HidD_GetInputReport for the profile data due to %!WINERROR!", dwRetVal);
					Check(false);
				}
                
                if (remainder != 0)
                {
                    memcpy(&dataBuffer[i * 32], &rdBuf[1], (i != nPage - 1) ? 32 : remainder);
                }
                else
                {
                    memcpy(&dataBuffer[i * 32], &rdBuf[1], 32);
                }

				::Sleep(5);
			}

			//验证Profile读取准确性
			memset(requestWriteBuf, 0, sizeof(requestWriteBuf));
			cmd = static_cast<BYTE>(ProfileCmd ::R_V_DATA);
			requestWriteBuf[0] = 0x01;
			requestWriteBuf[1] = 0xA5;
			requestWriteBuf[2] = cmd;
			requestWriteBuf[3] = ~requestWriteBuf[1];
			requestWriteBuf[4] = ~cmd;

			if (::HidD_SetOutputReport(m_ahProfileDevice.get(), &requestWriteBuf, sizeof(requestWriteBuf)) == FALSE)
			{
				DWORD dwRetVal = ::GetLastError();
				WPPTrace(TRACE_LEVEL_ERROR, PROFILE, L"Failed in calling HidD_SetOutputReport for verification due to %!WINERROR!", dwRetVal);
				Check(false);
			}
			::Sleep(5);

			//获取下位机应答的验证码
			memset(rdBuf, 0, sizeof(rdBuf));
			rdBuf[0] = 0x01;
			if (::HidD_GetInputReport(m_ahProfileDevice.get(), &rdBuf, sizeof(rdBuf)) == FALSE)
			{
				DWORD dwRetVal = ::GetLastError();
				WPPTrace(TRACE_LEVEL_ERROR, PROFILE, L"Failed in calling HidD_GetInputReport for verification due to %!WINERROR!", dwRetVal);
				Check(false);
			}
			int iReadCheckSum = rdBuf[27] << 24 | rdBuf[26] << 16 | rdBuf[25] << 8 | rdBuf[24];


			//上位机计算的验证码
			int iGetCheckSum = 0;

			LPBYTE tmpBuf = reinterpret_cast<LPBYTE>(&profile.data);
			for (int i = 0; i < profileSize; i++)
			{
				iGetCheckSum += tmpBuf[i];
			}

			if (iGetCheckSum != iReadCheckSum)
			{
				//验证失败
				WPPTrace(TRACE_LEVEL_ERROR, PROFILE, L"Verification failed");
				Check(false);
			}
		}
		catch (...)
		{
			WPPTrace(TRACE_LEVEL_ERROR, PROFILE, L"Failed in reading profile data");
			result = false;
		}

		return result;
    }

	bool ProfileDevice::WriteProfileData(BYTE modeProfileNumber, Profile& profile)
	{
		bool result = true;

		try
		{
			//准备写数据命令
			BYTE cmd = static_cast<BYTE>(ProfileCmd::W_INFO);

			BYTE requestWriteBuf[64];
			memset(requestWriteBuf, 0, sizeof(requestWriteBuf));
			requestWriteBuf[0] = 0x01;
			requestWriteBuf[1] = 0xA5;
			requestWriteBuf[2] = cmd;
			requestWriteBuf[3] = ~0xA5;
			requestWriteBuf[4] = ~cmd;
			requestWriteBuf[5] = modeProfileNumber;
			
			if (::HidD_SetOutputReport(m_ahProfileDevice.get(), &requestWriteBuf, sizeof(requestWriteBuf)) == FALSE)
			{
				DWORD dwRetVal = ::GetLastError();
				WPPTrace(TRACE_LEVEL_ERROR, PROFILE, L"Failed in calling HidD_SetOutputReport for ready to write due to %!WINERROR!", dwRetVal);
				Check(false);
			}
			::Sleep(5);

			BYTE rdBuf[64] = { 0 };
			memset(rdBuf, 0, sizeof(rdBuf));
			rdBuf[0] = 0x01;
			do
			{
				//获取下位机应答
				if (::HidD_GetInputReport(m_ahProfileDevice.get(), &rdBuf, sizeof(rdBuf)) == FALSE)
				{
					DWORD dwRetVal = ::GetLastError();
					WPPTrace(TRACE_LEVEL_ERROR, PROFILE, L"Failed in calling HidD_GetInputReport for ready to write due to %!WINERROR!", dwRetVal);
					Check(false);
				}
				::Sleep(5);
			} while (rdBuf[8] != 0xAA);

			//写数据命令
			auto profileSize = sizeof(Profile);
			int nPage = static_cast<int>(profileSize / 16);
            auto remainder = profileSize % 16;
            if (remainder != 0)
            {
                nPage++;
            }

			LPBYTE dataBuffer = reinterpret_cast<LPBYTE>(profile.data);
			//m_profile.config_base_index.light_type = 0x1;
			//m_profile.config_base_index.light_rgb = 0xffffff;
			//m_profile.config_base_index.light_intensity = 2;

			for (int i = 0; i < nPage; i++)
			{
				memset(requestWriteBuf, 0, 64);
				cmd = static_cast<BYTE>(ProfileCmd::W_PF_DATA);
				requestWriteBuf[0] = 0x01;
				requestWriteBuf[1] = 0xA5;
				requestWriteBuf[2] = cmd;
				requestWriteBuf[3] = 0x5A;
				requestWriteBuf[4] = ~cmd;
				requestWriteBuf[6] = i;

                if (remainder != 0)
                {
                    memcpy(&requestWriteBuf[8], &dataBuffer[i * 16], (i != nPage - 1) ? 16 : remainder);
                }
                else
                {
                    memcpy(&requestWriteBuf[8], &dataBuffer[i * 16], 16);
                }

				//发写数据命令
				if (::HidD_SetOutputReport(m_ahProfileDevice.get(), &requestWriteBuf, sizeof(requestWriteBuf)) == FALSE)
				{
					DWORD dwRetVal = ::GetLastError();
					WPPTrace(TRACE_LEVEL_ERROR, PROFILE, L"Failed in calling HidD_SetOutputReport for writing profile data due to %!WINERROR!", dwRetVal);
					Check(false);
				}
				::Sleep(5);

				do
				{
					memset(rdBuf, 0, sizeof(rdBuf));
					rdBuf[0] = 0x01;
					if (::HidD_GetInputReport(m_ahProfileDevice.get(), &rdBuf, sizeof(rdBuf)) == FALSE)
					{
						DWORD dwRetVal = ::GetLastError();
						WPPTrace(TRACE_LEVEL_ERROR, PROFILE, L"Failed in calling HidD_GetInputReport for writing profile data due to %!WINERROR!", dwRetVal);
						Check(false);
					}
				} while (rdBuf[8] != 0xAA);
			}

			//验证写入的数据正确性
			memset(requestWriteBuf, 0, sizeof(requestWriteBuf));
			cmd = static_cast<BYTE>(ProfileCmd::W_V_DATA);
			requestWriteBuf[0] = 0x01;
			requestWriteBuf[1] = 0xA5;
			requestWriteBuf[2] = cmd;
			requestWriteBuf[3] = 0x5A;
			requestWriteBuf[4] = ~cmd;

			//发命令
			if (::HidD_SetOutputReport(m_ahProfileDevice.get(), &requestWriteBuf, sizeof(requestWriteBuf)) == FALSE)
			{
				DWORD dwRetVal = ::GetLastError();
				WPPTrace(TRACE_LEVEL_ERROR, PROFILE, L"Failed in calling HidD_SetOutputReport for verifying due to %!WINERROR!", dwRetVal);
				Check(false);
			}
			::Sleep(5);

			//获取下位机应答的验证码
			memset(rdBuf, 0, sizeof(rdBuf));
			rdBuf[0] = 0x01;
			if (::HidD_GetInputReport(m_ahProfileDevice.get(), &rdBuf, sizeof(rdBuf)) == FALSE)
			{
				DWORD dwRetVal = ::GetLastError();
				WPPTrace(TRACE_LEVEL_ERROR, PROFILE, L"Failed in calling HidD_GetInputReport for verifying due to %!WINERROR!", dwRetVal);
				Check(false);
			}
			int iReadCheckSum = rdBuf[25] << 8 | rdBuf[24];

			//上位机计算的验证码
			int iGetCheckSum = 0;
			for (int i = 0; i < profileSize; i++)
			{
				iGetCheckSum = iGetCheckSum + dataBuffer[i];
			}

			//验证失败
			if (iGetCheckSum != iReadCheckSum)
			{
				WPPTrace(TRACE_LEVEL_ERROR, PROFILE, L"Verification failed");
				Check(false);
			}
			::Sleep(5);

			//通知下位机把验证正确的数据存入Flash中
			memset(requestWriteBuf, 0, sizeof(requestWriteBuf));
			cmd = static_cast<BYTE>(ProfileCmd::W_S_DATA);
			requestWriteBuf[0] = 0x01;
			requestWriteBuf[1] = 0xA5;
			requestWriteBuf[2] = cmd;
			requestWriteBuf[3] = 0x5A;
			requestWriteBuf[4] = ~cmd;
			if (::HidD_SetOutputReport(m_ahProfileDevice.get(), &requestWriteBuf, sizeof(requestWriteBuf)) == FALSE)
			{
				DWORD dwRetVal = ::GetLastError();
				WPPTrace(TRACE_LEVEL_ERROR, PROFILE, L"Failed in calling HidD_SetOutputReport for writing flash due to %!WINERROR!", dwRetVal);
				Check(false);
			}
		}
		catch (...)
		{
			WPPTrace(TRACE_LEVEL_ERROR, PROFILE, L"Failed in writing profile data");
			result = false;
		}

		return result;
	}

	void ProfileDevice::GetNextReport()
	{
		// initiate asynchronous read
		BOOL result = ::ReadFileEx(m_ahProfileDevice.get(),
								   m_pHidIOCompletion->GetReportBuffer(),
								   m_pHidIOCompletion->GetReportSize(),
								   m_pHidIOCompletion->GetOverlappedIO(),
			[](const DWORD errorCode, DWORD, OVERLAPPED* pOverlapped)
			{
				// get object pointer from user-defined field in OVERLAPPED
				HidIOCompletion* pHidIOCompletion = static_cast<HidIOCompletion*>(pOverlapped->hEvent);

				if (pHidIOCompletion != NULL && pHidIOCompletion->IsValid())
				{
					if (errorCode == NOERROR)
					{
						// call collection object's dispatch routine 
						pHidIOCompletion->GetDevice()->ParseReport();
						// start next read
						pHidIOCompletion->GetDevice()->GetNextReport();
					}
				}
			});
		if (result == FALSE)
		{
			// Error handling is in HidDeviceReadCompleteProc
		}
	}

	void ProfileDevice::ParseReport()
	{
		if (auto pchBuffer{ m_pHidIOCompletion->GetReportBuffer() };
			pchBuffer != nullptr)
		{
			switch (pchBuffer[0])
			{
			case Com::REPORTID_2: // For mapping keys
				ParseReportForKeyCode(pchBuffer[9], 0x01, m_isQuickDown, KEYCODE::Quick, L"Quick");
				ParseReportForKeyCode(pchBuffer[7], 0x80, m_isR3Down, KEYCODE::R3, L"R3");
				ParseReportForKeyCode(pchBuffer[7], 0x40, m_isL3Down, KEYCODE::L3, L"L3");
				ParseReportForKeyCode(pchBuffer[7], 0x20, m_isBackDown, KEYCODE::Back, L"BACK");
				ParseReportForKeyCode(pchBuffer[7], 0x10, m_isStartDown, KEYCODE::Start, L"START");
				ParseReportForKeyCode(pchBuffer[7], 0x08, m_isDPadRightDown, KEYCODE::DPAD_RIGHT, L"DPAD RIGHT");
				ParseReportForKeyCode(pchBuffer[7], 0x04, m_isDPadLeftDown, KEYCODE::DPAD_LEFT, L"DPAD LEFT");
				ParseReportForKeyCode(pchBuffer[7], 0x02, m_isDPadDownDown, KEYCODE::DPAD_DOWN, L"DPAD DOWN");
				ParseReportForKeyCode(pchBuffer[7], 0x01, m_isDPadUpDown, KEYCODE::DPAD_UP, L"DPAD UP");
				ParseReportForKeyCode(pchBuffer[8], 0x80, m_isYDown, KEYCODE::Y, L"Y");
				ParseReportForKeyCode(pchBuffer[8], 0x40, m_isXDown, KEYCODE::X, L"X");
				ParseReportForKeyCode(pchBuffer[8], 0x20, m_isBDown, KEYCODE::B, L"B");
				ParseReportForKeyCode(pchBuffer[8], 0x10, m_isADown, KEYCODE::A, L"A");
				ParseReportForKeyCode(pchBuffer[8], 0x02, m_isR1Down, KEYCODE::R1, L"R1");
				ParseReportForKeyCode(pchBuffer[8], 0x01, m_isL1Down, KEYCODE::L1, L"L1");

				ParseReportForThumb(pchBuffer[1], m_thumbLX, Thumb::LX, L"Thumb LX");
				ParseReportForThumb(pchBuffer[2], m_thumbLY, Thumb::LY, L"Thumb LY");
				ParseReportForThumb(pchBuffer[3], m_thumbRX, Thumb::RX, L"Thumb RX");
				ParseReportForThumb(pchBuffer[4], m_thumbRY, Thumb::RY, L"Thumb RY");

				ParseReportForTrigger(pchBuffer[5], m_triggerL2, Trigger::Left, L"Trigger Left");
				ParseReportForTrigger(pchBuffer[6], m_triggerR2, Trigger::Right, L"Trigger Right");
				break;

			case Com::REPORTID_3: // For mode and fw version only
				ParseReportForMode(pchBuffer[9], 0x04, L"Controller Mode");
				ParseReportForVersion(pchBuffer[11], pchBuffer[10], L"Firmware Version");
                ParseReportForKeyCode(pchBuffer[9], 0x08, m_isM1Down, KEYCODE::M1, L"M1");
                ParseReportForKeyCode(pchBuffer[9], 0x10, m_isM2Down, KEYCODE::M2, L"M2");
				break;

			default:
				WPPTrace(TRACE_LEVEL_INFORMATION, PROFILE, L"Unexcepted report: %d", pchBuffer[0]);
				break;
			}			
		}
	}

	void ProfileDevice::ParseReportForKeyCode(CHAR ch, UCHAR mask, bool& flag, KEYCODE key, wstring const& description)
	{
		if ((ch & mask) && !flag)
		{
			WPPTrace(TRACE_LEVEL_INFORMATION, PROFILE, L"%ws is down", description.c_str());

			flag = true;
			m_pfnProfileDeviceEventHandler(ProfileDeviceEvent::KeyPressed,
										   this,
										   key,
										   true,
										   m_lpContext);					
		}

		if (!(ch & mask) && flag)
		{
			WPPTrace(TRACE_LEVEL_INFORMATION, PROFILE, L"%ws is up", description.c_str());

			flag = false;
			m_pfnProfileDeviceEventHandler(ProfileDeviceEvent::KeyPressed,
										   this,
										   key,
										   false,
										   m_lpContext);
		}
	}

	void ProfileDevice::ParseReportForThumb(CHAR ch, BYTE& value, Thumb thumb, wstring const& description)
	{
		// Requires to notify all thumb report no matter if it changes
		auto v = (BYTE)(ch);
		{
			WPPTrace(TRACE_LEVEL_INFORMATION, PROFILE, L"%ws is %d", description.c_str(), v);

			value = v;
			m_pfnProfileDeviceEventHandler(ProfileDeviceEvent::ThumbPressed,
										   this,
										   thumb,
										   v,
										   m_lpContext);
		}
	}

	void ProfileDevice::ParseReportForTrigger(CHAR ch, BYTE& value, Trigger trigger, wstring const& description)
	{	
		if (auto v = (BYTE)ch; 
			v != value)
		{
			WPPTrace(TRACE_LEVEL_INFORMATION, PROFILE, L"%ws is %d", description.c_str(), v);

			value = v;
			m_pfnProfileDeviceEventHandler(ProfileDeviceEvent::TriggerPressed,
										   this,
										   trigger,
										   v,
										   m_lpContext);
		}
	}

	void ProfileDevice::ParseReportForMode(CHAR ch, UCHAR mask, wstring const& description)
	{
		if (auto newMode = static_cast<BYTE>(ch & mask);
			m_mode != newMode)
		{
			WPPTrace(TRACE_LEVEL_INFORMATION, PROFILE, L"%ws is %d", description.c_str(), newMode);

			BYTE oldMode = m_mode;
			m_mode = newMode;
			m_pfnProfileDeviceEventHandler(ProfileDeviceEvent::ModeChanged,
										   this,
										   oldMode,
										   newMode,
										   m_lpContext);
		}
	}

	void ProfileDevice::ParseReportForVersion(CHAR major, CHAR minor, wstring const& description)
	{
		if ((BYTE)major != m_majorVersion || (BYTE)minor != m_minorVersion)
		{
			WPPTrace(TRACE_LEVEL_INFORMATION, PROFILE, L"%ws is %d.%d", description.c_str(), major, minor);

			m_majorVersion = major;
			m_minorVersion = minor;
			m_pfnProfileDeviceEventHandler(ProfileDeviceEvent::VersionArrived,
				                           this,
				                           major,
				                           minor,
				                           m_lpContext);
		}
	}

	////////////////////////////// HidIOCompletion class ///////////////////////////
	HidIOCompletion::HidIOCompletion(int reportSize, ProfileDevice* pHidDevice) :
		reportSize_(reportSize),
		pHidDevice_(pHidDevice),
		pReportBuffer_(0)
	{
		// OVERLAPPED::hEvent is user-defined when used with ReadFileEx()
		::ZeroMemory(&overlappedIO_, sizeof(overlappedIO_));
		overlappedIO_.hEvent = this;

		// allocate report buffer
		pReportBuffer_ = new char[reportSize_];
		Check(pReportBuffer_ != 0);
	}

	HidIOCompletion::~HidIOCompletion()
	{
		delete[] pReportBuffer_;
	}

	void HidIOCompletion::CheckInvariant() const
	{
		Validate(reportSize_ > 0);
		Validate(pReportBuffer_ != 0);
	}
}