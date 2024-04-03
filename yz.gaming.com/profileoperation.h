#pragma once
#include "inc/abstractdevicechangebulletin.h"
#include "hotplugbulletin.h"
#include "profiledevice.h"
#include "comimpl.h"

namespace YZ::Gaming::Com
{    
    class ProfileOperation : public AbstractDeviceChangeBulletin
    {
    public:
        ProfileOperation(PFN_PROFILEDEVICEEVENTHANDLER pfnProfileDeviceEventHandler, LPVOID lpContext);
        ~ProfileOperation() noexcept;

        bool Initialize();
        void Uninitialize();
        vector<ProfileDevice::Ptr> GetProfileDevices() const;        

        // Interface AbstractDeviceChangeBulletin        
        void AttachDevice(const wchar_t* deviceName, const GUID& devClassGuid) override;
        void RemoveDevice(HANDLE device) override;
        void RemoveDevice(const wchar_t* pDeviceName, const GUID& devClassGuid) override;
        void DeviceCustomEvent(const DEV_BROADCAST_HDR*) override {}
        void RegisterDevice(HANDLE, HANDLE*) override {}
        void CheckInvariant() const override;

    private:
        void OpenProfileDevices();

        vector<ProfileDevice::Ptr> m_profileDevices;
        shared_ptr<HotplugBulletin> m_hotpluginBulletin;
        thread m_hotpluginBulletinThread;
        PFN_PROFILEDEVICEEVENTHANDLER m_pfnProfileDeviceEventHandler;
        LPVOID m_lpContext{ nullptr };
    };
}

