#include "pch.h"
#include "profileoperation.h"
#include "hidutils.h"
#include "inc\contract.h"
#include "inc\constants.h"
#include "inc\wpptrace.h"
#include "profileoperation.tmh"

namespace YZ::Gaming::Com
{
    ProfileOperation::ProfileOperation(PFN_PROFILEDEVICEEVENTHANDLER pfnProfileDeviceEventHandler, LPVOID lpContext) :
        m_pfnProfileDeviceEventHandler(pfnProfileDeviceEventHandler),
        m_lpContext(lpContext)
    {
        Require(m_pfnProfileDeviceEventHandler != nullptr);
        OpenProfileDevices();
    }

    ProfileOperation::~ProfileOperation() noexcept
    {
    }

    bool ProfileOperation::Initialize()
    {
        m_hotpluginBulletinThread = thread([this]()
        {
            HRESULT hr = ::CoInitializeEx(nullptr, COINIT_MULTITHREADED);
            Check(SUCCEEDED(hr));

            m_hotpluginBulletin = make_shared<HotplugBulletin>(WindowClass::HotpluginBulletin, *this);
            Check(bool{ m_hotpluginBulletin });

            auto profileDevices{ GetProfileDevices() };
            for (auto& profileDevice : profileDevices)
            {
                profileDevice->OnDeviceAdded(m_hotpluginBulletin);

                m_pfnProfileDeviceEventHandler(ProfileDeviceEvent::DeviceAdded,
                                               profileDevice.get(),
                                               NULL,
                                               NULL,
                                               m_lpContext);
            }

            // process any queued messages
            for (;;)
            {
                // Call MsgWaitForMultipleObjectsEx() in order to put the thread in an
                // alertable wait state.  HIDClient uses ReadFileEx, and the call-back
                // never happens if this function is not called.
                ::MsgWaitForMultipleObjectsEx(0, nullptr, 10, QS_ALLINPUT, MWMO_ALERTABLE);

                MSG msg{};
                msg.message = WM_NULL;

                while (::PeekMessage(&msg, nullptr, 0, 0, PM_REMOVE) != FALSE)
                {
                    if (msg.message == WM_QUIT)
                    {
                        break;
                    }

                    ::TranslateMessage(&msg);
                    ::DispatchMessage(&msg);
                }

                // We broke out of the above loop.  If it was a quit message, break 
                // entirely out of the message loop
                if (msg.message == WM_QUIT)
                {
                    break;
                }
            }

            ::CoUninitialize();
        });
        
        return true;
    }

    void ProfileOperation::Uninitialize()
    {
        ::PostThreadMessage(::GetThreadId(m_hotpluginBulletinThread.native_handle()), WM_QUIT, NULL, NULL);
        ::WaitForSingleObject(m_hotpluginBulletinThread.native_handle(), INFINITE);

        if (m_hotpluginBulletinThread.joinable())
        {
            m_hotpluginBulletinThread.join();
        }
    }

    vector<ProfileDevice::Ptr> ProfileOperation::GetProfileDevices() const
    {
        return m_profileDevices;
    }

    void ProfileOperation::AttachDevice(const wchar_t* deviceName, const GUID& devClassGuid)
    {
        UNREFERENCED_PARAMETER(devClassGuid);

        Require(bool{ m_hotpluginBulletin });

        try
        {
            if (auto profileDevice{ make_shared<ProfileDevice>(deviceName, m_pfnProfileDeviceEventHandler, m_lpContext) };
                bool{ profileDevice })
            {
                m_profileDevices.emplace_back(profileDevice);

                profileDevice->OnDeviceAdded(m_hotpluginBulletin);

                m_pfnProfileDeviceEventHandler(ProfileDeviceEvent::DeviceAdded,
                                               profileDevice.get(),
                                               NULL,
                                               NULL,
                                               m_lpContext);
            }
        }
        catch (exception&)
        {
            WPPTrace(TRACE_LEVEL_INFORMATION, PROFILE, L"Exception is thrown while initializing the profile device, ignore it");
        }
    }

    void ProfileOperation::RemoveDevice(HANDLE device)
    {
        auto iter = find_if(m_profileDevices.begin(), m_profileDevices.end(), [device](auto&& profileDevice) {
            return profileDevice->IsHandleOwned(device);
            });
        if (iter != m_profileDevices.end())
        {
            auto& profileDevice = *iter;
            m_pfnProfileDeviceEventHandler(ProfileDeviceEvent::DeviceRemoved,
                                           profileDevice.get(),
                                           NULL,
                                           NULL,
                                           m_lpContext);

            profileDevice->OnDeviceRemoved();
            m_profileDevices.erase(iter);
        }
    }

    void ProfileOperation::RemoveDevice(const wchar_t* pDeviceName, const GUID& devClassGuid)
    {
        UNREFERENCED_PARAMETER(pDeviceName);
        UNREFERENCED_PARAMETER(devClassGuid);
    }

    void ProfileOperation::CheckInvariant() const
    {
    }

    void ProfileOperation::OpenProfileDevices()
    {
        auto&& deviceNames = HidUtils::EnumerateHIDDeviceNames();
        for (auto const& deviceName : deviceNames)
        {
            try
            {   
                if (auto profileDevice{ make_shared<ProfileDevice>(deviceName, m_pfnProfileDeviceEventHandler, m_lpContext) };
                    bool{ profileDevice })
                {
                    m_profileDevices.emplace_back(profileDevice);
                }
            }
            catch (exception&)
            {
                WPPTrace(TRACE_LEVEL_INFORMATION, PROFILE, L"Exception is thrown while initializing the profile device, ignore it");
            }
        }
    }
}