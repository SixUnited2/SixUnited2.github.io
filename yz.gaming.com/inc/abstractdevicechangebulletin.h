//
//	AbstractDeviceChangeBulletin.h
//
//	Definition of the abstract class AbstractDeviceChangeBulletin which is the base class
//	for all classes that need notification of hotplugging events.
//	

#pragma once

#include <windows.h>
#include "contract.h"
#include <dbt.h>

namespace YZ::Gaming::Com
{
	class AbstractDeviceChangeBulletin : public WithInvariant
	{

		// public interface
	public:

		//
		// add a device to the list of attached devices
		//		deviceName is file name of device to attach
		//

		virtual void AttachDevice(const wchar_t* deviceName, const GUID& devClassGuid) = 0;


		//
		// remove a device from the list of attached devices
		//		hDevice is handle of device to remove
		//

		virtual void RemoveDevice(HANDLE device) = 0;


		//
		// Alternate version for devices we could not get a handle for.
		// Notice that this version is not mandatory to implement.
		//		deviceName is file name of device to attach
		//

		virtual void RemoveDevice(const wchar_t* /*pDeviceName*/, const GUID& /*devClassGuid*/)
		{
		}

		//
		// register a device to be monitored for device change events
		//		hDevice is handle of device to register
		//

		virtual void RegisterDevice(HANDLE device, HANDLE* pDeviceNotification = 0) = 0;

		//
		// handle custom event from device. Handler must examine 
		// pBroadcast->dbch_devicetype to obtain type of device, then convert 
		// pointer to this structure to pointer to device type specific structure 
		// (such as DEV_BROADCAST_HANDLE, or DEV_BROADCAST_VOLUME, etc.)
		//		pBroadcast is pointer to structure describing event. 
		//
		virtual void DeviceCustomEvent(const DEV_BROADCAST_HDR* pBroadcast) = 0;

		//
		// Invariant
		//
		virtual void CheckInvariant() const = 0;


		// default virtual destructor
	public:

		virtual ~AbstractDeviceChangeBulletin() throw()
		{
		}

	};
}

