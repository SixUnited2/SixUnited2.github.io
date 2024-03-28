////////////////////////////////////////////////////////////////////////
//
// hidutils.h
//
// Declaration of utility functions dealing with hid and device installation
//
///////////////////////////////////////////////////////////////////////

#pragma once

namespace YZ::Gaming::Com
{
    namespace HidUtils
    {
        /// <summary>
        /// Enumerate all present HID device names
        /// </summary>
        /// <returns>All HID device names</returns>
        vector<wstring> EnumerateHIDDeviceNames();
    }
}
