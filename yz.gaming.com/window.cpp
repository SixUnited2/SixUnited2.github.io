///////////////////////////////////////////////////////////////////////////////
//
//	Window.cpp
//
//	Implements DPG window helpers
//
///////////////////////////////////////////////////////////////////////////////
#include "pch.h"
#include "window.h"
#include "inc/wpptrace.h"
#include "window.tmh"

namespace YZ::Gaming::Com
{

//
// This implementation of RegisterWindowClass uses a lazy implementation to avoid needing to
// matching ::RegisterClass() with ::UnregisterClass() after the window class is no longer needed.
// It does this by using ::GetClassInfo() and then comparing WNDCLASS for the existing registered
// class with the one being registered. If they do not match then an attempt will be made to 
// unregister the class. This will fail if the current registered class is being used by an 
// existing window. 
//
// The return value will be true if the class was registered. A return value of false should  
// generally be considered a fatal logic error.
//

bool RegisterWindowClass( WNDCLASSEX& wndClass ) noexcept
{
	bool succeeded = true;

	WNDCLASSEX currentWndClass = {0};
	currentWndClass.cbSize = sizeof(currentWndClass);
	if ( ::GetClassInfoEx( wndClass.hInstance, wndClass.lpszClassName, &currentWndClass ) && 
		memcmp( &currentWndClass, &wndClass, sizeof( wndClass ) ) != 0 )
	{
		// If there is any difference in the existing and new window class
		// unregister window class. UnregisterClass will fail if the class is being used
		if ( ::UnregisterClassW( currentWndClass.lpszClassName, currentWndClass.hInstance ) == FALSE )
		{
			WPPTrace( TRACE_LEVEL_ERROR, WINDOW, L"Window class %ws could not be unregistered: error %!WINERROR!", wndClass.lpszClassName, ::GetLastError() );
			succeeded = false;
		}
	}

	if ( succeeded )
	{
		ATOM wndClassAtom = ::RegisterClassEx( &wndClass );
		// If the last error is ERROR_CLASS_ALREADY_EXISTS this is OK since
		// we have already checked for a mismatched window class
		if ( wndClassAtom == 0 && ::GetLastError() != ERROR_CLASS_ALREADY_EXISTS )
		{
			WPPTrace( TRACE_LEVEL_ERROR, WINDOW, L"Window class %ws could not be registered: error %!WINERROR!", wndClass.lpszClassName, ::GetLastError() );
			succeeded = false;
		}
	}
	return succeeded;
}

}
