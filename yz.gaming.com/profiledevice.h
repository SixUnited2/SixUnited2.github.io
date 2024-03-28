#pragma once
#include "inc/auto_handle.h"
#include "hotplugbulletin.h"
#include "comimpl.h"
#include "profile.h"

namespace YZ::Gaming::Com
{
	class HidIOCompletion;

    class ProfileDevice
    {
    public:
        using Ptr = shared_ptr<ProfileDevice>;

        ProfileDevice() = delete;
        explicit ProfileDevice(wstring const& deviceName, PFN_PROFILEDEVICEEVENTHANDLER pfnProfileDeviceEventHandler, LPVOID lpContext);
        bool IsHandleOwned(HANDLE hProfileDevice) const noexcept;
        bool ReadProfileData(BYTE modeProfileNumber, Profile& profile, DWORD& fwVersion);
        bool WriteProfileData(BYTE modeProfileNumber, Profile& profile);
        
        void OnDeviceAdded(shared_ptr<HotplugBulletin> hotplugBulletin);
        void OnDeviceRemoved();

    private:
		void GetNextReport();
		void ParseReport();
		void ParseReportForKeyCode(CHAR ch, UCHAR mask, bool& flag, KEYCODE key, wstring const& description);
		void ParseReportForThumb(CHAR ch, BYTE& value, Thumb thumb, wstring const& description);
		void ParseReportForTrigger(CHAR ch, BYTE& value, Trigger trigger, wstring const& description);
		void ParseReportForMode(CHAR ch, UCHAR mask, wstring const& description);
		void ParseReportForVersion(CHAR major, CHAR minor, wstring const& description);

        auto_handle<HANDLE> m_ahProfileDevice;
        HANDLE m_hDeviceNotification{ nullptr };
		HidIOCompletion* m_pHidIOCompletion{ nullptr };
		
		PFN_PROFILEDEVICEEVENTHANDLER m_pfnProfileDeviceEventHandler;
		LPVOID m_lpContext{ nullptr };

		mutable bool m_isQuickDown{ false };
		mutable bool m_isL3Down{ false };
		mutable bool m_isR3Down{ false };
		mutable bool m_isBackDown{ false };
		mutable bool m_isStartDown{ false };
		mutable bool m_isDPadRightDown{ false };
		mutable bool m_isDPadLeftDown{ false };
		mutable bool m_isDPadDownDown{ false };
		mutable bool m_isDPadUpDown{ false };
		mutable bool m_isYDown{ false };
		mutable bool m_isXDown{ false };
		mutable bool m_isBDown{ false };
		mutable bool m_isADown{ false };
		mutable bool m_isR1Down{ false };
		mutable bool m_isL1Down{ false };
        mutable bool m_isM1Down{ false };
        mutable bool m_isM2Down{ false };
		mutable unsigned char m_thumbLX{ 0 };
		mutable unsigned char m_thumbLY{ 0 };
		mutable unsigned char m_thumbRX{ 0 };
		mutable unsigned char m_thumbRY{ 0 };
		mutable unsigned char m_triggerL2{ 0 };
		mutable unsigned char m_triggerR2{ 0 };
		mutable unsigned char m_mode{ UCHAR_MAX };
		mutable unsigned char m_majorVersion{ 0 };
		mutable unsigned char m_minorVersion{ 0 };
    };

	class HidIOCompletion : public WithInvariant
	{
	public:
		HidIOCompletion(int reportSize, ProfileDevice* pHidDevice);
		~HidIOCompletion();

		virtual void CheckInvariant() const;

		// Quick function to determine if this strucutre is still useable
		bool IsValid() const { return pHidDevice_ != 0; }

		// Sets the object's pHidDevice_ pointer, flagging the object as not useable
		void Invalidate() { pHidDevice_ = 0; }

		// Accessors
		char* GetReportBuffer() { CheckInvariant(); return pReportBuffer_; }
		char* GetReportBuffer() const { CheckInvariant(); return pReportBuffer_; }
		int GetReportSize() const { CheckInvariant(); return reportSize_; }
		OVERLAPPED* GetOverlappedIO() { CheckInvariant(); return &overlappedIO_; }
		ProfileDevice* GetDevice() { CheckInvariant(); return pHidDevice_; }

	private:
		// report buffer and size for HID reports
		// This must remain char* reportBuffer_ as HID reports are of BYTE(=char) type 
		char* pReportBuffer_;
		int reportSize_;

		// structure for asynchronous file I/O
		OVERLAPPED overlappedIO_;

		// A back pointer to the HidDevice that is using the char buffer and OVERLAPPED structure.
		// This pointer will be NULLed after the HidDevice is deleted, signaling that the HidDevice
		// is no longer available for use.
		ProfileDevice* pHidDevice_;
	};
}

