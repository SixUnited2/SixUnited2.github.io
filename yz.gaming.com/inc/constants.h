///////////////////////////////////////////////////////////////////////////////
//
//  StringLengths.h
//
//  Lengths to use when declaring strings
//
///////////////////////////////////////////////////////////////////////////////

#pragma once

#include <windows.h>

namespace YZ::Gaming::Com
{
    namespace Constants
    {
        namespace StringLength
        {
            // Use this for storing any paths or paths with filenames.  This length
            // is 4 times MAX_PATH + 1 because some Asian characters in MBCS can 
            // be up to 4 bytes long.
            const unsigned int MAX_PATH_NAME = 4 * (MAX_PATH + 1);

            // Use this for filenames
            const unsigned int MAX_FILE_NAME = 512;

            // Use this for storing classnames
            const unsigned int MAX_CLASS_NAME = 256;

            // Use this for storing captionnames
            const unsigned int MAX_CAPTION_NAME = 256;

            // Use this for error strings
            const unsigned int MAX_ERROR_STRING = 1024;

            // Use this for general strings
            const unsigned int MAX_STRING = 1024;

            // Use this for small strings
            const unsigned int MAX_SMALL_STRING = 128;

            // From MSDN: The maximum length of a key name 
            // is 256 Unicode characters. 
            // The maximum length of a key value is 
            // 16383 Unicode characters

            // Use this for Registry Keys
            const unsigned int MAX_REG_KEY = 256;

            // Use this for registry paths
            const unsigned int MAX_REG_PATH = 1024;

            // Use this for Registry Values until we see it breaking
            const unsigned int MAX_REG_VALUE = 1024;

            // Maximun size of the dns host name
            const unsigned int MAX_DNS_HOST_NAME = 64;
        }

        namespace DeviceID
        {
            constexpr unsigned short VID = 0x2f24;
            constexpr unsigned short PID = 0x0135;
            constexpr unsigned short USAGE_PAGE = 0xff00;
            constexpr unsigned short USAGE = 0x0001;
        }

        namespace WindowClass
        {
            const wchar_t HotpluginBulletin[] = L"yz.gaming.com.hotpluginbulletin";
        }
    }
}
