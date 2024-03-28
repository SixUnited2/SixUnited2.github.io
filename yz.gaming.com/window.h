///////////////////////////////////////////////////////////////////////////////
//
//	Window.h
//
//	Implementation of template class Window
//
///////////////////////////////////////////////////////////////////////////////

#pragma once 


//disable the L4 warning in the system header file
#pragma warning(push)
#pragma warning(disable: 4702)
#include <map>
#pragma warning(pop)

#include <stdexcept>
#include <windows.h>
#include <typeinfo>
#include "inc/contract.h"
#include <atlstr.h>

#include "inc/wpptrace.h"
#include "window.h.tmh"

namespace YZ::Gaming::Com
{

//
// RegisterWindowClass
// 
// RegisterWindowClass should be used to register DPG window classes to ensure
// that the window procedure registered is pointing to correct window proc. 
// See bug 18413 for details. 
//
// The consequences of this bug are described in:
//
// http://blogs.msdn.com/oldnewthing/archive/2006/09/20/763727.aspx. 
//
// Basically this bug occurs when a ::RegisterClass() returns the error ERROR_CLASS_ALREADY_EXISTS, 
// AND the window proc in the current registered class no longer points to the correct window 
// proc or points to an address in no-mans land. A more subtle form of this bug can also occur 
// if other fields of the WNDCLASS structure have been changed but not updated. Needless to say  
// this bug can be very difficult to debug.
//
// To use this function replace ::RegisterClass() with RegisterWindowClass(). The return value 
// will be true if the class is correctly registered. A return value of false should be considered 
// a fatal logic error. You do not need to use this function if you know that the registered class
// is being correctly unregistered.
//

bool RegisterWindowClass( WNDCLASSEX& wndClass ) noexcept;

//
//	Template class Window is a generic class parameterized by 
//	the type of the creating object.  
//
//	Like MFC::CWnd or ATL::CWindow, it allows the user to wrap a C++ class 
//	around a Win32 window class.
//
//	Unlike MFC or ATL, it is small, typesafe, AND macro-free.
//
//	Window messages are mapped to their handlers using the STL 'map' 
//	template.  See Stroustrup 3ed, section 15.5, "Pointers to Members" for 
//	suggestion of mapping messages to member functions via a dispatch table.
//
//
//	Things to know about Window:
//
//	1.	Window cannot be used in an inheritance hierarchy with virtual function. 
//	This is because the Window doesn’t use the virtual function table. The message 
//	handlers are bound statically to the window procedure.
//
//	2.	Further, if the mapped message handler (member function) calls a virtual function 
//	which is also implemented in a derived class, it will call its own implementation 
//	of the virtual function, although we would expect it to call the derived class’s 
//	implementation of it, based on the theory that a call to a virtual function inside a 
//	member function would be a vtable call with an implicit use of ‘this’. We don’t have 
//	a concrete answer as to why it is not working as expected.	
//	REVIEW (bug 441) get an answer.
//

template <typename Wrapper> 
class Window
{

//
// creation procedures
//

// protected creation procedures
protected:

    //
    // Window is only intended to be used in an inheritance relationship.
    // The protected constructor binds a C++ class to a Win32 window class.
    // The binding happens on reciept of WM_CREATE.  Note that some messages 
    // may be sent before WM_CREATE, and we will not map those.
    //

    Window(const std::wstring& className);	// name of the win32 window class to bind



// public creation procedures
public:

    //
    // Lifetime of Window object and HWND are synchronized.  Win32 object will
    // be destroyed in Window destructor.
    //

    virtual ~Window() noexcept;


// hidden default constructors
private:
    Window();
    Window(const Window<Wrapper>&);
    Window<Wrapper>& operator=(const Window<Wrapper>&);


//
// public interface
//

public:

    // pointer to message handler member function
    typedef LRESULT (Wrapper::*MessageHandler)(UINT, WPARAM, LPARAM);


    //
    // associate a window message with a member function
    //
    // member function must have the following signature:
    //
    //		LRESULT HandleMessage(
	//			UINT message,		// window message ID
    //			WPARAM wParam,		// first  message argument
    //			LPARAM lParam);		// second message argument
    //
    // ...fortunately, the compiler will enforce this
    //

    virtual void MapMessage(
        UINT message,					// message to associate handler with
        const MessageHandler pFunction);	// pointer to message handler


    //
    // dissociate window message from function pointer 
    //

    virtual void UnmapMessage(UINT message);		// message to unmap


    //
    //	allow the client to declare a message map in the class declaration, ie:
    //
    //	class MyClass : public Window<MyClass>
    //	{
    //	public:
    //		void DeclarePublicMessageMap()
    //		{
    //			MapMessage(WM_THIS, &MyClass::DoThis);
    //			MapMessage(WM_THAT, &MyClass::DoThat);
    //		}
    //			
    //		LRESULT DoThis(UINT message, WPARAM wParam, LPARAM lParam);
    //
    //		LRESULT DoThat(UINT message, WPARAM wParam, LPARAM lParam);
    //	};
    //

    void DeclarePublicMessageMap()
    {
		// null implementation 
	}


    //
    // get the underlying window handle
    //

    HWND GetWindowHandle() const
    {
		return ::IsWindow(hWindow_) ? hWindow_ : 0;
	}


    //
    // conversion operator returns the underlying window handle
    //

    operator HWND() const
    {
		return GetWindowHandle();
	}


//
// private implementation
//

// implementation functions
private:

    //
    // initialize the object
    //

    void Init(
        const std::wstring& className,	// name of the client class
        Wrapper* pObject);					// pointer to client object


    //
    // bind this HWND to this Window<> object
    //

    void Bind(HWND hWindow);


    //
    // unbind the HWND from this Window<> object
    //

    void Unbind();


    //
    // look up message handler and call it
    //

    LRESULT CallMessageHandler(
        UINT message,		// window message
        WPARAM wParam,		// first  message argument 
        LPARAM lParam);		// second message argument


    //
    // window message dispatch procedure
    //

    static LRESULT WINAPI DispatchWindowMessage(
        HWND hWindow,		// handle of window receiving message
        UINT message,		// window message
        WPARAM wparam,		// first  message argument
        LPARAM lparam);		// second message argument


    //
    // get the Window<> object associated with a window handle
    //

    static Window<Wrapper>* GetWindowObject(HWND hWindow);


    //
    // get the key name for the win32 property that associates HWNDs with 
    // Window object pointers
    //

    static const wchar_t* GetPropName()
    {
		return L"Window";   
	}

// instance data
private:

	// pointer to the client object
	Wrapper* pObject_;		

	// handle of window created to receive messages
	HWND hWindow_;	

	wstring className_;

	// map of window messages to function pointers	
    typedef std::map<UINT, MessageHandler> MessageMap;
	MessageMap messageMap_;	
}; // end class Window


///////////////////////////// creation procedures /////////////////////////////

template <typename Wrapper>
Window<Wrapper>::Window(const std::wstring& className) :	// name of the client class
	pObject_(0), 
	hWindow_(0)
{
	Require( className.length() != 0 );
	className_ = className;
	Init(className, static_cast<Wrapper*>(this));
}

template <typename Wrapper>
Window<Wrapper>::~Window() noexcept
{
	if( ::IsWindow( hWindow_ ) != 0 )
    {
        // WM_DESTROY handler could throw an exception.  We must prevent 
        // this exception from leaving the destructor.
        try
        {
            ::DestroyWindow(hWindow_);
            if(hWindow_ != 0)
            {
                Window<Wrapper>* pWindow = GetWindowObject(hWindow_);
                if(pWindow != 0)
                {
                    ::RemovePropW(hWindow_, GetPropName());
                }
            }
        }

        catch (...)
        {
			_ASSERT(false);
        }
    }

	hWindow_ = 0;
	pObject_ = 0;
}

////////////////////////////// public interface ///////////////////////////////



template <typename Wrapper>
void Window<Wrapper>::MapMessage(
    UINT message,                   // message to associate handler with
    const MessageHandler pFunction) // pointer to message handler
{
    using std::invalid_argument;

    if (pFunction == 0) 
	{
		WPPTrace( TRACE_LEVEL_FATAL, WINDOW, L"Null function pointer" );
		ThrowDBC( invalid_argument );
	}

    // associate the function pointer with the message id
    messageMap_[message] = pFunction;
}


template <typename Wrapper>
void Window<Wrapper>::UnmapMessage(
    UINT message)                   // message to unmap
{
    // delete the function associated with this message id
    messageMap_.erase(message);
}


/////////////////////////// protected implementation //////////////////////////



template <typename Wrapper>
void Window<Wrapper>::Init(
    const std::wstring& className,      // name of the client class
    Wrapper* pObject)                   // pointer to client object
{
    using std::invalid_argument;
    using std::logic_error;
    using std::runtime_error;

	Require( className.length() != 0 );

	if (pObject == 0) 
	{
		WPPTrace( TRACE_LEVEL_FATAL, WINDOW, L"%s - null object passed to constructor", typeid(this).name());
		ThrowDBC( invalid_argument );
	}

    HINSTANCE hInstance = ::GetModuleHandle(0);
    if (hInstance == 0)
	{
		ThrowDBC( runtime_error );
	}

    // register messages declared in the client interface
    pObject->DeclarePublicMessageMap();

    // register the window class
	WNDCLASSEX wndClass = {0};
	wndClass.cbSize			= sizeof(wndClass);
    wndClass.lpfnWndProc	= Window<Wrapper>::DispatchWindowMessage;
    wndClass.hInstance		= hInstance;
    wndClass.lpszClassName	= className.c_str();
	wndClass.hCursor		= ::LoadCursor( NULL, IDC_ARROW );

	if ( RegisterWindowClass( wndClass ) == true )
	{
		// We need to fix our architecture to fix this - See comments at the top of this file
		// This is relatively safe provided that we do not unload any winproc that exists in a dll
		// BTW: This is the behavior before 6.3 except that we did not notice that there was
		// a potential problem.
		WPPTrace( TRACE_LEVEL_INFORMATION, WINDOW, L"Attempt to register duplicate window class, %ws, with different win proc", className.c_str() );
	}

    // save a back-pointer to the client object
    pObject_ = pObject;

    // create the window
    HWND hWindow = ::CreateWindowW(className.c_str(),0,0,0,0,0,0,0,0,hInstance,this);
    if (hWindow == 0) 
    {
		::UnregisterClassW( wndClass.lpszClassName, wndClass.hInstance );
        // rollback object variables
	    hWindow_ = 0;	
        pObject_ = 0;

		WPPTrace( TRACE_LEVEL_FATAL, WINDOW, L"%s - failed to create window", typeid(this).name());
		ThrowDBC( logic_error );
    }

    // check that binding was successful in response to WM_CREATE
    Window<Wrapper>* pWindow = GetWindowObject(hWindow);
    if (pWindow != this)
    {
        // rollback system calls
	    ::DestroyWindow(hWindow);
	    ::UnregisterClassW( wndClass.lpszClassName, wndClass.hInstance );

        // rollback object variables
	    hWindow_ = 0;	
        pObject_ = 0;

		WPPTrace( TRACE_LEVEL_FATAL, WINDOW, L"failed to bind window to object");
		ThrowDBC( logic_error );
    }
}


template <typename Wrapper>
void Window<Wrapper>::Bind(HWND hWindow)
{
    using std::invalid_argument;
    using std::logic_error;

    if (::IsWindow(hWindow_)) 
	{
		WPPTrace( TRACE_LEVEL_FATAL, WINDOW, L"window is already bound" );
		ThrowDBC( logic_error );
	}
    if (!::IsWindow(hWindow))
	{
		WPPTrace( TRACE_LEVEL_FATAL, WINDOW, L"bad window handle" );
		ThrowDBC( invalid_argument );
	}

    BOOL result = ::SetPropW(hWindow, GetPropName(), this);
    if (result == FALSE)
	{
		WPPTrace( TRACE_LEVEL_FATAL, WINDOW, L"unable to bind window");
		ThrowDBC( logic_error );
	}

    hWindow_ = hWindow;
}


template <typename Wrapper>
void Window<Wrapper>::Unbind()
{
    ::RemovePropW(hWindow_, GetPropName());
    hWindow_ = 0;
}


template <typename Wrapper>
Window<Wrapper>* Window<Wrapper>::GetWindowObject(
    HWND hWindow)
{   
    const wchar_t* propName = Window::GetPropName();
    return static_cast<Window<Wrapper>*>(::GetPropW(hWindow, propName)); 
}


template <typename Wrapper>
LRESULT WINAPI Window<Wrapper>::DispatchWindowMessage(
    HWND hWindow,				// handle of window receiving message
    UINT message,				// window message
    WPARAM wParam,				// first  message argument
    LPARAM lParam)				// second message argument
{
    LRESULT result = ERROR_SUCCESS;

    // get object associated with window
    Window<Wrapper>* pWindow = GetWindowObject(hWindow);

    // in WM_CREATE, the lParam contains a pointer to the Window<> object
    bool isNewWindow = (pWindow == 0) && (message == WM_NCCREATE) && (lParam != 0);

    if (isNewWindow)
    {
        // the window creation data includes the identity of the Window<> object
        CREATESTRUCT* pCS = reinterpret_cast<CREATESTRUCT*>(lParam);

        // get the encapsulating Window<> object
        pWindow = reinterpret_cast<Window<Wrapper>*>(pCS->lpCreateParams);

        // Bind the Window<> object to the window.  This must be done here
        // to allow windows messages that happen after WM_CREATE, but before 
        // return of CreateWindow(...) to be processed by the bound object

        pWindow->Bind(hWindow);
    }

    // if this window is bound to a Window<> object
    if (pWindow != 0)
    {
        // call object's message handler
        result = pWindow->CallMessageHandler(message, wParam, lParam);

        // unbind the Window<> if the window is destroyed
        if (message == WM_DESTROY)
        {
            pWindow->Unbind();
        }
    }
    else
    {
        // no object associated with window...call default handler
        result = ::DefWindowProc(hWindow, message, wParam, lParam);
    }

    return result;
}


template <typename Wrapper>
LRESULT Window<Wrapper>::CallMessageHandler(
    UINT message,				// window message
    WPARAM wParam,				// first  message argument 
    LPARAM lParam)				// second message argument
{
    using std::logic_error;

    if (pObject_ == 0)
	{
		WPPTrace( TRACE_LEVEL_FATAL, WINDOW, L"dispatching message to unbound window" );
		ThrowDBC( logic_error );
	}

    LRESULT result = ERROR_SUCCESS;

    auto pMessage = messageMap_.find(message);

    // is this message in the table?
    if (pMessage != messageMap_.end())
    {
        // get function pointer associated with this message
        MessageHandler pFunction = pMessage->second;

        // call the member function
        if (pFunction != 0) 
        {
	        result = (pObject_->*(pFunction))(message, wParam, lParam);
        }
    }
    else
    {
        // no handler associated with message...call default handler
        result = ::DefWindowProc(hWindow_, message, wParam, lParam);
    }

    return result;
}


} // end namespace DPG

