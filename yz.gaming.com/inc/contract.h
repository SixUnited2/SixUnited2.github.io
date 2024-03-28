///////////////////////////////////////////////////////////////////////////////
//
//	Contract.h
//
//	Design by Contract
//
//
//	Contract.h defines Design-by-Contract (tm) functions in the style of
//	the Eiffel programming language as defined in the landmark software
//	engineering textbook, Object-Oriented Software Construction,
//	second edition, by Betrand Meyer.
//
//	It is important to understand that simply calling these functions 
//	is not sufficient to implement full Design-by-Contract semantics.  
//	A true DbC system is atomic in the presence of exceptions, that is,
//	it is "exception-safe".  The code example below satisfies this property
//	by deferring updates to the state of the object and validating its 
//	postcondition before finally changing (committing) the state of the 
//	object.  If the routine throws an exception, no observable state 
//	changes will be made to the system overall (rollback).
//
//	An excellent description of exception-safety can be found in:
//	Exceptional C++, by Herb Sutter.
//
//
//	MyClass.h
//
//	class MyClass : public WithInvariant
//	{
//	public:
//
//		MyClass()
//		: state_(SOME_VALID_STATE)
//		{
//			// construct the object
//			Init();
//
//			// class Invariant
//			CheckInvariant();
//		}
//
//		void Operation(int arg)
//		{
//			// class Invariant
//			CheckInvariant();
//
//			// precondition
//			Require(arg != 0);
//
//			// do some stuff
//			State state = ::GetState();
//			Result result = ::DoSomething(state);
//			Check(result == true);
//
//			// postcondition
//			Ensure(state != 0);
//
//			// class Invariant
//			CheckInvariant();
//
//			// commit changes
//			state_ = state;
//		}
//
//		virtual void CheckInvariant() const
//		{
//			Validate(SomeInvariantClause());
//		}
//
//	private:
//	
//		State state_;
//
//	};
//

// REVIEW (bug 442) add fault injection

#pragma once

#include <crtdbg.h>
#include <windows.h>

#include <string>
#include <stdexcept>

#include "constants.h"
#include "faultinjection.h"

#include "wpptrace.h"
#include "contract.h.tmh"

using namespace YZ::Gaming::Com::Constants;
using std::logic_error;

// do not change this to use std::exceptions take only chars
using std::string;

// _ReturnAddress a complier intrinsic provides the address 
// of the instruction in the calling function that will be executed 
// after control returns to the caller.
// It should be prototyped before use (MSDN) 
#ifdef __cplusplus
extern "C"
#endif
void * _ReturnAddress(void);
#pragma intrinsic(_ReturnAddress)

namespace
{
	// Prefix string for error instrution address
	const char* ERROR_ADDRESS_STRING = "addr = 0x";

	// Indirectly get the current instruction address by using _ReturnAddress
	// This provides a non-machine specfic way getting current instruction address
	// without any __asm call or call to GetThreadContext()
	__declspec(noinline) void * GetCurrentInstructionAddress()
	{
		return _ReturnAddress();
	}
}

// This is needed so that debug output will not spew in release mode (bug 9547)
#ifndef NDEBUG
#define OutputDebugInfo( infoString ) ::OutputDebugStringA( infoString )
#else
#define OutputDebugInfo( infoString )
#endif

//
//	WithInvariant class: All classes that need an 
//	invariant should derive from this class
//

class WithInvariant
{
public:
	// The invariant function to be implemented by all classes deriving from WithInvariant
	virtual void CheckInvariant() const = 0;
};


//
//	contract_precondition should be thrown for precondition failures
//

class contract_precondition : public logic_error
{
public:
	// construct from message string
	explicit contract_precondition(const string& message)
		: logic_error(message)
	{
	}

	// destroy the object
	virtual ~contract_precondition()
	{
	}
};


//
//	contract_check should be thrown from checks in functions other than pre and post condition
//

class contract_check : public logic_error
{
public:
	// construct from message string
	explicit contract_check(const string& message)
		: logic_error(message)
	{
	}

	// destroy the object
	virtual ~contract_check()
	{
	}
};


//
//	contract_postcondition should be thrown from post conditions
//

class contract_postcondition : public logic_error
{
public:
	// construct from message string
	explicit contract_postcondition(const string& message)
		: logic_error(message)
	{
	}

	// destroy the object
	virtual ~contract_postcondition()
	{
	}
};


//
//	contract_invariant should be thrown from class invariants
//

class contract_invariant : public logic_error
{
public:
	// construct from message string
	explicit contract_invariant(const string& message)
		: logic_error(message)
	{
	}

	// destroy the object
	virtual ~contract_invariant()
	{
	}
};


//
//	Implementation of the contract functions Require, Check, and Ensure 
//	Require is used for Preconditions. Check is used for checks throughout 
//	functions. Ensure is used for Postconditions. Validate is used in Invariants
//

//
//  All contract functions (Require, Check, Ensure and Validate) are needed
//	be to always be inline functions, hence, __forceinline keyqord used.
//  However, functions that throw cannot be inline. That's why each function is 
//  separated into 2 functions, the contract fucntion itself and the associate 
//  ThrowXXX funcion. Due to condition above, these ThrowXXX functions will 
//  NEVER be inline by the complier the keyword inline is to help us just to
//  get pass the linker error of multiple definition as this contract.h is 
//  included everywhere.
//

inline void ThrowPrecondition(char const* file, int line, void * currentIP, char const* function)
{
	char buffer[StringLength::MAX_STRING] = {0};
	::_snprintf_s(buffer, StringLength::MAX_STRING, StringLength::MAX_STRING - 1, "Precondition exception: %s (%d), %s, %s%p, last error = %ul", file, line, function, ERROR_ADDRESS_STRING, currentIP, ::GetLastError() );
	WPPTrace( TRACE_LEVEL_FATAL, CONTRACT, L"%s", buffer );
	throw contract_precondition(buffer);
}

__forceinline void RequireFunction(bool condition, char const* file, int line, char const* function)
{
#ifdef FAULT_INJECT
	condition = FaultInjection::ProcessFaultInjection(condition, file, line, function);
#endif
	if (!condition)
	{
		void * currentIP = GetCurrentInstructionAddress();
		ThrowPrecondition(file, line, currentIP, function);
	}
}

inline void ThrowCheck(char const* file, int line, void * currentIP, char const* function)
{
	char buffer[StringLength::MAX_STRING] = {0};
	::_snprintf_s( buffer, StringLength::MAX_STRING, StringLength::MAX_STRING - 1, "Check exception: %s (%d), %s, %s%p, last error = %ul", file, line, function, ERROR_ADDRESS_STRING, currentIP, ::GetLastError() );
	WPPTrace( TRACE_LEVEL_FATAL, CONTRACT, L"%s", buffer );
	throw contract_check(buffer);
}

__forceinline void CheckFunction(bool condition, char const* file, int line, char const* function)
{
#ifdef FAULT_INJECT
	condition = FaultInjection::ProcessFaultInjection(condition, file, line, function);
#endif
	if (!condition)
	{
		void * currentIP = GetCurrentInstructionAddress();
		ThrowCheck(file, line, currentIP, function);
	}
}

inline void ThrowPostcondition(char const* file, int line, void * currentIP, char const* function)
{
	char buffer[StringLength::MAX_STRING] = {0};
	::_snprintf_s(buffer, StringLength::MAX_STRING, StringLength::MAX_STRING - 1, "Postcondition exception: %s (%d), %s, %s%p, last error = %ul", file, line, function, ERROR_ADDRESS_STRING, currentIP, ::GetLastError() );
	WPPTrace( TRACE_LEVEL_FATAL, CONTRACT, L"%s", buffer );
	throw contract_postcondition(buffer);
}

__forceinline void EnsureFunction(bool condition, char const* file, int line, char const* function)
{
#ifdef FAULT_INJECT
	condition = FaultInjection::ProcessFaultInjection(condition, file, line, function);
#endif
	if (!condition)
	{
		void * currentIP = GetCurrentInstructionAddress();
		ThrowPostcondition(file, line, currentIP, function);
	}
}

inline void ThrowInvariant(char const* file, int line, void * currentIP, char const* function)
{
	char buffer[StringLength::MAX_STRING] = {0};
	::_snprintf_s(buffer, StringLength::MAX_STRING, StringLength::MAX_STRING - 1, "ClassInvariant exception: %s (%d), %s, %s%p, last error = %ul", file, line, function, ERROR_ADDRESS_STRING, currentIP, ::GetLastError() );
	WPPTrace( TRACE_LEVEL_FATAL, CONTRACT, L"%s", buffer );
	throw contract_invariant(buffer);
}

__forceinline void ValidateFunction(bool condition, char const* file, int line, char const* function)
{
#ifdef FAULT_INJECT
	condition = FaultInjection::ProcessFaultInjection(condition, file, line, function);
#endif
	if (!condition)
	{
		void * currentIP = GetCurrentInstructionAddress();
		ThrowInvariant(file, line, currentIP, function);
	}
}

// There are two different versions of this function:  DEBUG and RELEASE.  The DEBUG
// version is the only useful one.  It displays a message box and outputs debug info
// to the debug consoles.  The RELEASE version does absolutely nothing.
#ifdef _DEBUG
inline void AssertFunction(bool condition, char const* file, int line, char const* function)
{
	// written this way to avoid the warning in release build
	if (!condition)
	{
		char buffer[StringLength::MAX_STRING] = {0};
		::_snprintf_s(buffer, StringLength::MAX_STRING, StringLength::MAX_STRING - 1, "Assert: %s (%d), %s, last error = %ul", file, line, function, ::GetLastError() );
		::MessageBoxA(NULL, buffer, "PLEASE LOG BUG WITH THIS INFORMATION", MB_OK);
		WPPTrace( TRACE_LEVEL_FATAL, CONTRACT, L"%s", buffer );
		_ASSERT(false);
	}
}
#else
inline void AssertFunction( bool, char const*, int, char const* )
{
}
#endif

#undef Require
#undef Check
#undef Ensure
#undef Validate

#ifndef _PREFAST_

// This is the template function that should be used to throw 
// all std::exceptions (which is the only thing our code should be throwing.
// It used to be a macro but the WPPTrace can't be called in a macro.
template<typename ExceptionType>
void ThrowFunction( char const* file, int line, char const* function )
{
	char throwExceptionBuffer[StringLength::MAX_STRING + 1 ];
	void * currentIP = GetCurrentInstructionAddress();
	::_snprintf_s( throwExceptionBuffer, StringLength::MAX_STRING + 1, StringLength::MAX_STRING, "Throwing...: %s (%d), %s, %s%p, last error = %ul", file, line, function ,ERROR_ADDRESS_STRING, currentIP, ::GetLastError() );
	WPPTrace( TRACE_LEVEL_FATAL, CONTRACT, L"%s", throwExceptionBuffer );
	throw ExceptionType( throwExceptionBuffer );
}

#define ThrowDBC(exception)    ThrowFunction<exception>(__FILE__, __LINE__, __FUNCTION__)
#define Require(condition)	RequireFunction(condition, __FILE__, __LINE__, __FUNCTION__)
#define Check(condition)	CheckFunction(condition, __FILE__, __LINE__, __FUNCTION__)
#define Ensure(condition)	EnsureFunction(condition, __FILE__, __LINE__, __FUNCTION__)
#define Validate(condition)	ValidateFunction(condition, __FILE__, __LINE__, __FUNCTION__)
#define AssertDBC(condition)	AssertFunction(condition, __FILE__, __LINE__, __FUNCTION__)
#define DoCheck(condition, exception) if ( !(condition) ) { ThrowDBC( exception ); }

#else

#define ThrowDBC( exception ) throw logic_error( "logic error" )
#define Require(condition)	if ( !(condition) ) { throw logic_error( "logic error" ); }
#define Check(condition)	if ( !(condition) ) { throw logic_error( "logic error" ); }
#define Ensure(condition)	if ( !(condition) ) { throw logic_error( "logic error" ); }
#define Validate(condition)	if ( !(condition) ) { throw logic_error( "logic error" ); }
#define AssertDBC(condition)	AssertFunction(condition, __FILE__, __LINE__, __FUNCTION__)
#define DoCheck(condition, exception)  if ( !(condition) ) { throw logic_error( "logic error" ); }

#endif

//
// Function helps extract back fault address from the errorString
// assumed generated by the contract functions, return 0 if the address
// cannot be found.
//
inline DWORD_PTR ExtractFaultAddress(const char* errorString)
{
	int faultAddress = 0;

	//first find the error address string
	char* faultAddressString = ::strstr(const_cast<char *>(errorString), ERROR_ADDRESS_STRING);
	if(faultAddressString != 0)
	{
		//found error address string, move the pointer the actual number
		faultAddressString += ::strlen(ERROR_ADDRESS_STRING);

		//extract hex number, sscanf should return 1 as there's only
		//1 field assignment
		if(::sscanf_s(faultAddressString, "%x", &faultAddress) != 1)
		{
			faultAddress = 0;
		}
	}

	return static_cast<DWORD_PTR>(faultAddress);
}

//
//	Macro CompileTimeCheck(condition, message) provides a static
//	assertion mechanism.  If an error condition can be evaluated
//	at compile time, then we should attempt to catch that condition
//	and fail to compile if it occurs.  The macro provides for a
//	meaningful diagnostic message to be printed within the compiler
//	error message.
//
//	See _Modern C++ Design: Generic Programming and Design Patterns Applied_, 
//	by Andrei Alexandrescu for more information.
//
//	CompileTimeCheck(sizeof(int) == 4, int_is_not_4_bytes);
//

template <bool> struct CompileTimeChecker	{	CompileTimeChecker(...);	};
template <> struct CompileTimeChecker<false> {	};
#define CompileTimeCheck(condition, message)	\
{\
	class ERROR_##message {};\
	(void)sizeof(CompileTimeChecker<(condition) != 0>((ERROR_##message())));\
}