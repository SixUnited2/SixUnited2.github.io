// HidUtil.h: interface for the CHidUtil class.
//
//////////////////////////////////////////////////////////////////////

#if !defined(AFX_HIDUTIL_H__3B5AF3E3_E09D_473D_B099_C04972492522__INCLUDED_)
#define AFX_HIDUTIL_H__3B5AF3E3_E09D_473D_B099_C04972492522__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

//#include <wdm.h>
#include "Setupapi.h"
#include "Dinput.h"
extern "C" {
#include "Hidsdi.h"
}

HANDLE GetDeviceViaInterface(GUID* pGuid, DWORD instance, LPTSTR pszName = NULL);

extern unsigned int DeviceVersion;
extern unsigned int  DeviceID;
#endif // !defined(AFX_HIDUTIL_H__3B5AF3E3_E09D_473D_B099_C04972492522__INCLUDED_)
