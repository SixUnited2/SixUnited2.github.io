////////////////////////////////////////////////////////////////////////
//
// hidutils.cpp
//
// Implementation of utility functions dealing with hid and device installation
//
////////////////////////////////////////////////////////////////////////

#include "pch.h"
#include "hidutils.h"

namespace YZ::Gaming::Com
{
    namespace HidUtils
    {   
        vector<wstring> EnumerateHIDDeviceNames()
        {
            vector<wstring> deviceNames;

            // Find all present HID devices
            GUID hidGUID;
            ::HidD_GetHidGuid(&hidGUID);

            HDEVINFO deviceInfo = ::SetupDiGetClassDevs(&hidGUID, 0, 0, DIGCF_PRESENT | DIGCF_INTERFACEDEVICE);
            if (deviceInfo != INVALID_HANDLE_VALUE)
            {
                // device interface data
                SP_DEVICE_INTERFACE_DATA deviceInterfaceData;
                deviceInterfaceData.cbSize = sizeof(SP_INTERFACE_DEVICE_DATA);

                // device interface index
                DWORD deviceIndex = 0;

                // enumurate through all device
                while (::SetupDiEnumDeviceInterfaces(deviceInfo, 0, &hidGUID, deviceIndex, &deviceInterfaceData) != FALSE)
                {
                    // getting device path
                    // call the API first time (this will fail) to get the require size of details structure
                    ULONG requiredSize = 0;
                    ::SetupDiGetDeviceInterfaceDetail(deviceInfo, &deviceInterfaceData, nullptr, 0, &requiredSize, 0);

                    vector<BYTE> deviceDetail(requiredSize, 0);
                    if (auto pDeviceDetail = reinterpret_cast<SP_INTERFACE_DEVICE_DETAIL_DATA*>(deviceDetail.data());
                        pDeviceDetail != nullptr)
                    {
                        pDeviceDetail->cbSize = sizeof(SP_INTERFACE_DEVICE_DETAIL_DATA);

                        BOOL isSuccess = ::SetupDiGetDeviceInterfaceDetail(deviceInfo,
                                                                           &deviceInterfaceData,
                                                                           pDeviceDetail,
                                                                           requiredSize,
                                                                           nullptr,
                                                                           0);
                        if (isSuccess != FALSE)
                        {
                            deviceNames.emplace_back(pDeviceDetail->DevicePath);
                        }
                    }

                    ++deviceIndex;
                }

                // free the handle allocated by GetClassDevs
                ::SetupDiDestroyDeviceInfoList(deviceInfo);
            }

            return deviceNames;
        }
    }
}