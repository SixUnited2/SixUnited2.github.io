//
// HotplugBulletin.cpp
//
// implements class HotplugBulletin
//

#include "pch.h"
#include "HotplugBulletin.h"

#include <initguid.h>
#pragma warning( push )
#pragma warning( disable : 4201 ) // nonstandard extension used : nameless struct/union
#include <ntddmou.h>
#pragma warning( push )
#include <ntddkbd.h>

#include "inc/wpptrace.h"
#include "HotplugBulletin.tmh"

///////////////////////// declare local namespace /////////////////////////////


namespace 
{
    // Wrapper for User32 export RegisterDeviceNotification
    HANDLE BulletinRegisterDeviceNotification(HWND notifyWindow, 
                                              void* pNotifyFilter, 
                                              DWORD flags);

    // Wrapper for User32 export UnregisterDeviceNotification
    void BulletinUnregisterDeviceNotification(HANDLE deviceNotificationHandle);
}

namespace YZ::Gaming::Com
{
    // constructor
    //        Window is message window used for messaging
    //        client is client object to notify
    //
    HotplugBulletin::HotplugBulletin(const wstring& windowName,
                                     AbstractDeviceChangeBulletin& deviceClient) :
        Window<HotplugBulletin>(windowName),
        deviceClient_(deviceClient),
        hidDeviceNotificationHandle_(nullptr)
    {
        try
        {
            // Use the conversion operator from Window
            Require(::IsWindow(*this) == TRUE);

            Create();

            CheckInvariant();
        }
        catch (std::exception&)
        {
            throw;
        }
    }

    //
    // destructor
    //
    HotplugBulletin::~HotplugBulletin() noexcept
    {
        try
        {
            Destroy();
        }
        catch (std::exception&)
        {
            WPPTrace(TRACE_LEVEL_ERROR, PNP, L"HotplugBulletin destructor threw an exception");
            AssertDBC(false);
        }
    }


    // make sure that client object ref is valid, message window is valid
    // and deviceNotificationHandle is valid
    //
    void HotplugBulletin::CheckInvariant() const
    {
        // the client object is valid
        Validate(&deviceClient_ != NULL);

        // our window is valid: uses the conversion operator from Window
        Validate(::IsWindow(*this) == TRUE);

        // we're registered for device notification
        Validate(hidDeviceNotificationHandle_ != nullptr);
        Validate(hidDeviceNotificationHandle_ != INVALID_HANDLE_VALUE);
    }

    ///////////////////////////// creation interface //////////////////////////////


    //
    // Creates hidden window for WM_DEVICECHANGE messages.  Register for 
    // notification of any change in the status of connected HID devices.
    //
    void HotplugBulletin::Create()
    {
        // get guid for hid class devices
        GUID hidGuid;
        ::HidD_GetHidGuid(&hidGuid);

        // register for hotplug notification
        DEV_BROADCAST_DEVICEINTERFACE interfaceNotifyFilter;
        ::ZeroMemory(&interfaceNotifyFilter, sizeof(interfaceNotifyFilter));

        interfaceNotifyFilter.dbcc_size = sizeof(DEV_BROADCAST_DEVICEINTERFACE);
        interfaceNotifyFilter.dbcc_devicetype = DBT_DEVTYP_DEVICEINTERFACE;
        interfaceNotifyFilter.dbcc_classguid = hidGuid;

        hidDeviceNotificationHandle_ = BulletinRegisterDeviceNotification(
            *this, //conversion operator from Window
            &interfaceNotifyFilter,
            DEVICE_NOTIFY_WINDOW_HANDLE);
        Check(hidDeviceNotificationHandle_ != nullptr);
    }


    //
    // Unregister for device notification
    //

    void HotplugBulletin::Destroy()
    {
        // unregister hotplug notification
        BulletinUnregisterDeviceNotification(hidDeviceNotificationHandle_);
    }



    ///////////////////////////// public interface ////////////////////////////////


    //
    // callback for hotplugging events 
    //
    LRESULT HotplugBulletin::SendDeviceUpdate(UINT message, WPARAM event, LPARAM data)
    {
        WPPTrace(TRACE_LEVEL_VERBOSE, PNP, L"PNP: Entering WM_DEVICECHANGE handler, wParam = 0x%Ix, lParam = 0x%Ix", event, data);

        CheckInvariant();

        // make sure it's the right message
        Require(message == WM_DEVICECHANGE);

        //NOTE: dwData can legitimately be zero

        LRESULT result = FALSE;
        DEV_BROADCAST_HDR* pHeader = reinterpret_cast<DEV_BROADCAST_HDR*>(data);

        switch (event)
        {
            // a new device has attached
        case DBT_DEVICEARRIVAL:

            WPPTrace(TRACE_LEVEL_INFORMATION, PNP, L"Received DBT_DEVICEARRIVAL");

            // we're only interested in HID devices
            if (pHeader->dbch_devicetype == DBT_DEVTYP_DEVICEINTERFACE)
            {
                DEV_BROADCAST_DEVICEINTERFACE* pInterface =
                    reinterpret_cast<DEV_BROADCAST_DEVICEINTERFACE*>(data);

                WPPTrace(TRACE_LEVEL_INFORMATION, PNP, L"Device %ws arrival", pInterface->dbcc_name);

                // ask device bulletin object to attach this device
                AttachDevice(pInterface->dbcc_name, pInterface->dbcc_classguid);
            }

            result = TRUE;
            break;


            // a device has been removed
        case DBT_DEVICEQUERYREMOVE:
        case DBT_DEVICEREMOVECOMPLETE:
        case DBT_DEVICEREMOVEPENDING:

            WPPTrace(TRACE_LEVEL_INFORMATION, PNP, L"Received %ws",
                (event == DBT_DEVICEQUERYREMOVE) ? L"DBT_DEVICEQUERYREMOVE" :
                (event == DBT_DEVICEREMOVEPENDING) ? L"DBT_DEVICEREMOVEPENDING"
                : L"DBT_DEVICEREMOVECOMPLETE");

            if (pHeader->dbch_devicetype == DBT_DEVTYP_HANDLE)
            {    // A handle to this device is open.
                DEV_BROADCAST_HANDLE* pBroadcast = reinterpret_cast<DEV_BROADCAST_HANDLE*>(data);

                // unregister device change notification for this device
                HANDLE devNotify = static_cast<HANDLE>(pBroadcast->dbch_hdevnotify);
                WPPTrace(TRACE_LEVEL_INFORMATION, PNP, L"PNP: Unregistering notifications for HANDLE = 0x%p", pBroadcast->dbch_handle);
                BulletinUnregisterDeviceNotification(devNotify);

                // ask device bulletin object to remove this device
                RemoveDevice(pBroadcast->dbch_handle);
            }
            else if (pHeader->dbch_devicetype == DBT_DEVTYP_DEVICEINTERFACE)
            {   // Used to catch device removals when we could
                // not open a handle to the device.
                DEV_BROADCAST_DEVICEINTERFACE* pInterface =
                    reinterpret_cast<DEV_BROADCAST_DEVICEINTERFACE*>(data);

                WPPTrace(TRACE_LEVEL_INFORMATION, PNP, L"Remove unhandled device %ws", pInterface->dbcc_name);

                // ask device bulletin object to remove this device
                RemoveDevice(pInterface->dbcc_name, pInterface->dbcc_classguid);
            }


            result = TRUE;
            break;


            // system was unable to remove device
        case DBT_DEVICEQUERYREMOVEFAILED:

            // HotplugBulletin: received DBT_DEVICEQUERYREMOVEFAILED
            WPPTrace(TRACE_LEVEL_WARNING, PNP, L"Device removal failed");

            result = TRUE;
            break;

        case DBT_CUSTOMEVENT:

            DeviceCustomEvent(pHeader);
            result = TRUE;

            break;

        default:
            break;
        }
        // make sure that client object ref is valid, message window is valid
        // and deviceNotificationHandle is valid
        CheckInvariant();

        WPPTrace(TRACE_LEVEL_INFORMATION, PNP, L"PNP: Leaving WM_DEVICECHANGE handler. result = %Id", result);

        // Return TRUE to grant the request.
        return TRUE;
    }


    //
    // RegisterDevice registers a single collection object for handle-based 
    // WM_DEVICECHANGE notification.  Object will automatically be unregistered
    // when a removal notification is received.
    //
    void HotplugBulletin::RegisterDevice(HANDLE device, HANDLE* pDeviceNotification)
    {
        WPPTrace(TRACE_LEVEL_INFORMATION, PNP, L"PNP: %!FUNC! called for HANDLE = 0x%p", device);
        // make sure that client object ref is valid, message window is valid
        // and deviceNotificationHandle is valid
        CheckInvariant();

        // make sure device is valid
        Require(device != 0);
        Require(device != INVALID_HANDLE_VALUE);

        // register for hotplug notification
        DEV_BROADCAST_HANDLE handleNotifyFilter;
        ::ZeroMemory(&handleNotifyFilter, sizeof(handleNotifyFilter));

        handleNotifyFilter.dbch_size = sizeof(DEV_BROADCAST_HANDLE);
        handleNotifyFilter.dbch_devicetype = DBT_DEVTYP_HANDLE;
        handleNotifyFilter.dbch_handle = device;

        HANDLE deviceNotification = BulletinRegisterDeviceNotification(*this, //conversion operator from Window
            &handleNotifyFilter,
            DEVICE_NOTIFY_WINDOW_HANDLE);
        Check(deviceNotification != NULL);

        if (pDeviceNotification != 0)
        {
            *pDeviceNotification = deviceNotification;
        }

        CheckInvariant();
    }

    //
    // Unregister a device being monitored for device change events.
    // deviceNotification is handle to the device notification
    //
    void CALLBACK HotplugBulletin::UnregisterDevice(HANDLE deviceNotification)
    {
        Require(deviceNotification != 0 && deviceNotification != INVALID_HANDLE_VALUE);

        // unregister device change notification for this device
        WPPTrace(TRACE_LEVEL_INFORMATION, PNP, L"PNP: Unregistering notifications for notification HANDLE = 0x%p", deviceNotification);
        BulletinUnregisterDeviceNotification(deviceNotification);
    }

    //
    // Implements callback interface method ADeviceChangeBulletin::AttachDevice.
    // Simply forwards the message to the registered client.
    //
    void HotplugBulletin::AttachDevice(const wchar_t* pDeviceName, const GUID& devClassGuid)
    {
        // make sure that client object ref is valid, message window is valid
        // and deviceNotificationHandle is valid
        CheckInvariant();

        // make sure devicename is legit
        Require(pDeviceName != 0);
        // NOTE: do NOT require that device name is nonempty -- CreateDevice
        // will check this for our devices, and we don't care for other devices.

        // forward call to client
        deviceClient_.AttachDevice(pDeviceName, devClassGuid);

        CheckInvariant();
    }


    //
    // Implements callback interface method ADeviceChangeBulletin::RemoveDevice.
    // Simply forwards the message to the registered client.
    //
    void HotplugBulletin::RemoveDevice(HANDLE device)
    {
        // no preconditions -- invalid values won't be in our list, so harmless

        // make sure that client object ref is valid, message window is valid
        // and deviceNotificationHandle is valid
        CheckInvariant();

        // forward call to client
        deviceClient_.RemoveDevice(device);

        CheckInvariant();
    }


    //
    // Implements callback interface method ADeviceChangeBulletin::RemoveDevice.
    // Simply forwards the message to the registered client.
    //
    void HotplugBulletin::RemoveDevice(const wchar_t* pDeviceName, const GUID& devClassGuid)
    {
        // no preconditions -- invalid values won't be in our list, so harmless

        // make sure that client object ref is valid, message window is valid
        // and deviceNotificationHandle is valid
        CheckInvariant();

        // forward call to client
        deviceClient_.RemoveDevice(pDeviceName, devClassGuid);

        CheckInvariant();
    }


    void HotplugBulletin::DeviceCustomEvent(const DEV_BROADCAST_HDR* pBroadcast)
    {
        CheckInvariant();
        deviceClient_.DeviceCustomEvent(pBroadcast);
        CheckInvariant();
    }
}

////////////////////////// private implementation  ////////////////////////////


// begin local namespace
namespace 
{ 
    //
    // Wrapper for User32 export RegisterDeviceNotification
    //

    // pointer to User32 export RegisterDeviceNotification
    typedef HANDLE (WINAPI* RDN)(HANDLE, void*, DWORD);
    RDN pRegisterDeviceNotification = 0;

    HANDLE BulletinRegisterDeviceNotification(const HWND notifyWindow, 
                                              void* pNotifyFilter, 
                                              const DWORD flags)
    { 
        HANDLE deviceNotification = NULL;

        //make sure device is valid
        Require(pNotifyFilter != 0);

        // load device registration function
        if (pRegisterDeviceNotification == 0)
        {
            HINSTANCE hUser32 = ::GetModuleHandle(L"User32.dll");
            Check(hUser32 != 0);

            pRegisterDeviceNotification = reinterpret_cast<RDN>
                (::GetProcAddress(hUser32, "RegisterDeviceNotificationW"));
        }
        Check(pRegisterDeviceNotification != 0);

        deviceNotification = pRegisterDeviceNotification(notifyWindow, pNotifyFilter, flags);
        Check(deviceNotification != 0);

        return deviceNotification;
    }


    //
    // Wrapper for User32 export UnregisterDeviceNotification
    //

    // pointer to User32 export UnregisterDeviceNotification
    typedef BOOL (WINAPI* URDN)(HANDLE);
    URDN pUnregisterDeviceNotification = 0;

    void BulletinUnregisterDeviceNotification(HANDLE deviceNotification)
    {
        BOOL success = false;
        // use if instead of dbc here -- don't crash on bad data from OS,
        // just ignore.
        if( (deviceNotification != 0) 
            && (deviceNotification != INVALID_HANDLE_VALUE) )
        {

            // load device registration function
            if (pUnregisterDeviceNotification == 0)
            {
                HINSTANCE hUser32 = ::GetModuleHandle(L"User32.dll");
                pUnregisterDeviceNotification = reinterpret_cast<URDN>
                    (::GetProcAddress(hUser32, "UnregisterDeviceNotification"));
            }
            Check(pUnregisterDeviceNotification != NULL);

            // call device registration function
            success = pUnregisterDeviceNotification(deviceNotification);
            // Bug #15742.  Better to silently fail than call Check(success != FALSE);
        }

        WPPTrace( TRACE_LEVEL_INFORMATION,
                  PNP,
                  L"Hotplug - %ws device notification 0x%p",
                  success ? L"unregistered " : L"unable to unregister ",
                  deviceNotification );
    }


} // end local namespace
