//
//	HotplugBulletin.h
//
//	HotplugBulletin is a class for dealing with hotplugging of HID devices
//	

#pragma once

#include <windows.h>
#include <dbt.h>
#include <string>

#include "inc/abstractdevicechangebulletin.h"
#include "window.h"
#include "inc/contract.h"

using std::wstring;

namespace YZ::Gaming::Com
{
	//
	//	Concrete class HotplugBulletin listens for system notifications that 
	//	a device has been attached or removed.  All device attach events are 
	//	forwarded to the client.  If the client is interested in devices of this 
	//	type and opens a handle to the device, then it may register the handle 
	//	with the bulletin.  The client will be notified of a removal event for 
	//	the registered device.
	//
	//	HotplugBulletin is similar to a publisher/subscriber system, but at the
	//	moment, only a single client is supported.  Multiple instances can coexist
	//	if more than one client requires notification.
	//

	class HotplugBulletin : public AbstractDeviceChangeBulletin,
							public Window<HotplugBulletin>
	{

		//
		// creation interface
		//

		// public creation procedures
	public:

		//
		// constructor
		//		Window is message window used for messaging
		//		client is client object to notify
		//
		HotplugBulletin(const wstring& windowName,
						AbstractDeviceChangeBulletin& deviceClient);


		virtual ~HotplugBulletin() noexcept;


		// protected creation procedures
	protected:

		void Create();
		void Destroy();


		//
		// public interface
		//

	public:

		//
		// The object will respond to these messages:
		//
		void DeclarePublicMessageMap()
		{
			MapMessage(WM_DEVICECHANGE, &HotplugBulletin::SendDeviceUpdate);
		}


		//
		// Send a device change notification to the registered client.
		//		event is DBT_x code defined in winuser.h
		//		data is event-specific data
		//
		virtual LRESULT SendDeviceUpdate(UINT message, WPARAM event, LPARAM data);


		//
		// Add a device to the list of attached devices.
		// Called in response to WM_DEVICECHANGE: DBT_DEVICEARRIVAL.
		//		deviceName is file name of device to attach
		//
		virtual void AttachDevice(const wchar_t* pDeviceName, const GUID& devClassGuid);


		//
		// Register a device to be monitored for device change events.
		//		hDevice is handle of device to register
		//
		virtual void RegisterDevice(HANDLE device, HANDLE* pDeviceNotification);

		//
		// Unregister a device being monitored for device change events.
		//		deviceNotification is handle to the device notification
		//
		static void CALLBACK UnregisterDevice(HANDLE deviceNotification);

		//
		// Remove a device from the list of attached devices.
		// Called in response to WM_DEVICECHANGE: DBT_DEVICEREMOVED
		//		hDevice is handle of device to remove
		//
		virtual void RemoveDevice(HANDLE device);

		//
		// Alternate version for devices we could not get a handle for.
		//		deviceName is file name of device to attach
		//
		virtual void RemoveDevice(const wchar_t* pDeviceName, const GUID& devClassGuid);

		//
		// Handle custom event from device. 
		// Called in response to WM_DEVICECHANGE: DBT_CUSTOM
		//
		virtual void DeviceCustomEvent(const DEV_BROADCAST_HDR* pBroadcast);


		// implements interface WithInvariant
	public:

		virtual void CheckInvariant() const;

		// instance data
	protected:

		// client object will be notified of device change events
		AbstractDeviceChangeBulletin& deviceClient_;

		// registration for device notification
		HANDLE hidDeviceNotificationHandle_;

	};
}
