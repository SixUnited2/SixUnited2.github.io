///////////////////////////////////////////////////////////////////////////////
//
//	auto_handle.h
//
//	Implementation of auto_handles for various windows types (similar to auto_ptr)
//
///////////////////////////////////////////////////////////////////////////////

#pragma once 

#include <Windows.h>

namespace YZ::Gaming::Com
{
    template <class HandleType, class HandleTraitsType>
    class base_handle
    {
    public:

	    base_handle()
	    {}

	    static void dispose(HandleType h) noexcept
	    {	HandleTraitsType::dispose(h);	}

	    static HandleType copy(HandleType& h) noexcept
	    {	return h;	}

    private:

	    base_handle(base_handle&);
	    base_handle& operator=(base_handle&);
    };

    template <class HandleType, class HandleTraitsType>
    class exclusive_owner
    {
    public:

	    exclusive_owner(){}
	    exclusive_owner(exclusive_owner&){}

	    static void dispose(HandleType h) noexcept
	    {	HandleTraitsType::dispose(h);	}

	    static HandleType copy(HandleType& h) noexcept
	    {	
		    HandleType handle(h);

		    h = HandleTraitsType::null_value();

		    return handle;	
	    }
    };

    template <class HandleType>
    struct handle_traits
    {
	    // These functions need to be inline so that it can be used by any # of libraries 
	    //  which are linked by a dll/exe. Otherwise we will get an error regarding multiple definitions.
	    inline static void dispose(HandleType) noexcept;
	    inline static HandleType null_value() noexcept;
    };

    // A template class for returning 
    // auto_handles by value
    template<class HandleType> 
    struct auto_handle_ref
    {
        explicit auto_handle_ref(HandleType h) 
            : handle_(h) {}    
        HandleType handle_;
    };


    // auto_handle
    template<
	    class HandleType, 
        class HandleTraitsType = handle_traits<HandleType>,
        class OwnershipSemanticsType = exclusive_owner<HandleType, HandleTraitsType> > 
    class auto_handle : private OwnershipSemanticsType
    {
    public:
        // 20.4.5.1 construct/copy/destroy 
        explicit auto_handle(
            const HandleType& h = HandleTraitsType::null_value()) noexcept
        : handle_(h) {}    
    
        auto_handle(auto_handle& that) noexcept
            : OwnershipSemanticsType(that),
            handle_(OwnershipSemanticsType::copy(that.handle_)) {}
        ~auto_handle() noexcept
        { 
		    if(handle_ != HandleTraitsType::null_value())
		    {
			    OwnershipSemanticsType::dispose(handle_);
		    }
        }    

        // 20.4.5.2 members
        HandleType get() const noexcept
        { return handle_; }

        // release ownership
        HandleType release() noexcept
        { 
            HandleType h(handle_);

            handle_ = HandleTraitsType::null_value();
            return h;
        }
 
        void reset(const HandleType& handle) noexcept
        {
            if (handle_ != handle)
            {
			    if(handle_ != HandleTraitsType::null_value())
			    {
				    HandleTraitsType::dispose(handle_);
			    }
                handle_ = handle;
            }
        }

        // 20.4.5.3 conversions
        // implicit ctor, clients may write
        // auto_handle<some_class> h = func()
        // where func returns auto_handle by
        // value
        auto_handle(
            const auto_handle_ref<HandleType>& r) noexcept
            : handle_(r.handle_) 
        {}
        operator auto_handle_ref<HandleType>() 
        { return auto_handle_ref<HandleType>(release()); }

        // other operators
        auto_handle& operator=(
            const auto_handle_ref<HandleType>& r) 
        {
            auto_handle tmp(r);
		
            std::swap(handle_, tmp.handle_);
            return *this;
        }

        auto_handle& operator=(
            auto_handle& rhs)
        {
            auto_handle tmp(rhs);

            std::swap(handle_, tmp.handle_);
            return *this;
        }
    
        bool operator !() const
        { return handle_ == HandleTraitsType::null_value(); }

    private:
        HandleType handle_;
    };



    //
    // specializations for win32 handle types
    //


    //
    // registry key
    //

    template<> inline HKEY handle_traits<HKEY>::null_value() noexcept
    {	return 0;	}

    template<> inline void handle_traits<HKEY>::dispose(HKEY h) noexcept
    {	::RegCloseKey(h);	}


    //
    // KERNEL objects
    //

    // generic win32 handle

    template<> inline HANDLE handle_traits<HANDLE>::null_value() noexcept
    {	return INVALID_HANDLE_VALUE;	}

    template<> inline void handle_traits<HANDLE>::dispose(HANDLE h) noexcept
    {	
	    // Some API functions (like CreateEvent) return zero rather than INVALID_HANDLE_VALUE.
	    // The caller checks for INVALID_HANDLE_VALUE (::null_value above) but because
	    // of the inconsistencies in Win32 we check for zero here as well.
	    if (h != 0)
	    {
		    ::CloseHandle(h);	
	    }
    }


    //
    // USER objects
    //

    // window

    template<> inline HWND handle_traits<HWND>::null_value() noexcept
    {	return 0;	}

    template<> inline void handle_traits<HWND>::dispose(HWND h) noexcept
    {	::DestroyWindow(h);	}

    // menu

    template<> inline HMENU handle_traits<HMENU>::null_value() noexcept
    {	return 0;	}

    template<> inline void handle_traits<HMENU>::dispose(HMENU h) noexcept
    {	::DestroyMenu(h);	}

    // icon

    template<> inline HICON handle_traits<HICON>::null_value() noexcept
    {	return 0;	}

    template<> inline void handle_traits<HICON>::dispose(HICON h) noexcept
    {	::DestroyIcon(h);	}


    //
    // Device Context
    //

    template<> inline HDC handle_traits<HDC>::null_value() noexcept
    {	return 0;	}

    template<> inline void handle_traits<HDC>::dispose(HDC h) noexcept
    {	::DeleteDC(h);	}


    //
    // GDI objects
    //

    // bitmap

    template<> inline HBITMAP handle_traits<HBITMAP>::null_value() noexcept
    {	return 0;	}

    template<> inline void handle_traits<HBITMAP>::dispose(HBITMAP h) noexcept
    {	::DeleteObject(h);	}

    // brush

    template<> inline HBRUSH handle_traits<HBRUSH>::null_value() noexcept
    {	return 0;	}

    template<> inline void handle_traits<HBRUSH>::dispose(HBRUSH h) noexcept
    {	::DeleteObject(h);	}

    // font

    template<> inline HFONT handle_traits<HFONT>::null_value() noexcept
    {	return 0;	}

    template<> inline void handle_traits<HFONT>::dispose(HFONT h) noexcept
    {	::DeleteObject(h);	}

    // pen

    template<> inline HPEN handle_traits<HPEN>::null_value() noexcept
    {	return 0;	}

    template<> inline void handle_traits<HPEN>::dispose(HPEN h) noexcept
    {	::DeleteObject(h);	}

    // region

    template<> inline HRGN handle_traits<HRGN>::null_value() noexcept
    {	return 0;	}

    template<> inline void handle_traits<HRGN>::dispose(HRGN h) noexcept
    {	::DeleteObject(h);	}

    // palette

    template<> inline HPALETTE handle_traits<HPALETTE>::null_value() noexcept
    {	return 0;	}

    template<> inline void handle_traits<HPALETTE>::dispose(HPALETTE h) noexcept
    {	::DeleteObject(h);	}
} // end namespace DPG

