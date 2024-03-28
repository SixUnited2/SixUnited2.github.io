#pragma once

#define WIN32_LEAN_AND_MEAN             // Exclude rarely-used stuff from Windows headers
// Windows Header Files
#include <windows.h>
#include <setupapi.h>
#include <hidsdi.h>
#include <strsafe.h>
#include <xinput.h>
#include <wbemidl.h> 

// STL header files
#include <string>
#include <vector>
#include <memory>
#include <exception>
#include <thread>
#include <algorithm>

using namespace std;
