///////////////////////////////////////////////////////////////////////////////
//
// Object used when running FaultInjection testing.  Only use is in Contract.h
//
///////////////////////////////////////////////////////////////////////////////

#pragma once

//#define FAULT_INJECT
#include <atlbase.h>
#include <atlcom.h>
#include <string>
#include <stdlib.h>
#include <intsafe.h>

using std::wstring;

#define FAULT_INJECT_FILE L"C:\\FaultInject.xml"


class FaultInjection
{
public:
    FaultInjection(const wchar_t* xml, bool isXML = false) : dirty_(false), filePath_(NULL)
    {
        HRESULT hr;
        
        hr = CoInitialize(0);
        if (FAILED(hr))
        {
            throw "Cannot CoInitialize()";
        }

        hr = DOM_.CoCreateInstance(__uuidof(DOMDocument));
        if (FAILED(hr) || DOM_.p == NULL)
        {
            throw "Unable to create XML parser object";
        }

        if (isXML)
        {
            VARIANT_BOOL bSuccess = false;
            hr = DOM_->loadXML(CComBSTR(xml), &bSuccess);
            if (FAILED(hr) || !bSuccess) 
            {
                DOM_ = NULL;
            }
        }
        else
        {
            VARIANT_BOOL bSuccess = false;
            hr = DOM_->load(CComVariant(xml), &bSuccess);
            if (FAILED(hr) || !bSuccess) 
            {
                DOM_ = NULL;
            }

            size_t filePathSize = 0;
            hr = ::IntToSizeT( lstrlen(xml), &filePathSize );
            if ( FAILED( hr ) )
            {
                throw "Integer overflow";
            }

            hr = ::SizeTAdd( filePathSize, 1, &filePathSize );
            if ( FAILED( hr ) )
            {
                throw "Integer overflow";
            }

            filePath_ = new wchar_t[filePathSize];
            if (filePath_ == NULL)
            {
                throw "Cannot allocate memory";
            }
            
            wcscpy_s(filePath_, filePathSize, xml);
        }
        if (DOM_ == NULL)
        {
            throw "Cannot load xml";
        }
    }

    ~FaultInjection(void)
    {
        if (filePath_ != NULL && dirty_)
        {
            DOM_->save(CComVariant(filePath_));
        }
        DOM_ = NULL;
        delete filePath_;

        CoUninitialize();
    }

    bool ShouldFaultInject(const char* file, int line)
    {
        bool result = false;
        CComQIPtr<IXMLDOMElement> selectedElt;
        selectedElt = GetNodeForFileLine(file, line);
        if (selectedElt != NULL)
        {
            CComVariant hasEnc;
            HRESULT hr = selectedElt->getAttribute(CComBSTR(L"hasEncountered"), &hasEnc);
            if (FAILED(hr) || hasEnc.vt != VT_BSTR)
            {
                throw "Cannot get hasEncountered attribute";
            }
            result = (lstrcmp(hasEnc.bstrVal, L"false") == 0);
        }
        return result;
    }

    void SetHasEncountered(const char* file, int line, bool newValue = true)
    {
        CComQIPtr<IXMLDOMElement> selectedElt;
        selectedElt = GetNodeForFileLine(file, line);

        if (selectedElt == NULL)
        {
            throw "Unable to find specified element";
        }
        HRESULT hr = selectedElt->setAttribute(CComBSTR(L"hasEncountered"), CComVariant(newValue ? L"true" : L"false"));
        if (FAILED(hr)) 
        {
            throw "Unable to set hasEncountered attribute";
        }
        dirty_ = true;
    }

    static bool ProcessFaultInjection(const bool condition, const char* file, int line, const char* function)
    {
        bool result = condition;
        if (condition)
        {
            try
            {
                FaultInjection faultInjection(FAULT_INJECT_FILE);
                if (faultInjection.ShouldFaultInject(ShortFileName(file), line))
                {
                    wstring message = L"We are about to inject a fault at ";
                    {
                        USES_CONVERSION;
                        wchar_t atowBuffer[34];
                        _itow_s(line, atowBuffer, 34, 10);
#pragma warning(push) // warning C6387(Code Analysis): ShortFileName() never return null
#pragma warning(disable : 6387)
                        message.append(A2W(ShortFileName(file)));
#pragma warning(pop)
                        message.append(L"(");
                        message.append(atowBuffer);
                        message.append(L") in function ");
#pragma warning(push) // warning C6387(Code Analysis): function is never null
#pragma warning(disable : 6387)
                        message.append(A2W(function));
#pragma warning(pop)
                        message.append(L".");
                    }

                    int mBoxResult = MessageBox(NULL, message.c_str(), L"DPG Fault Injection", MB_OKCANCEL);
                    // if MessageBox fails it returns 0.  This will happen when exe
                    // is starting up and DLLs are still loading.  If this happens assume OK
                    if (mBoxResult == IDOK || mBoxResult == 0)
                    {
                        faultInjection.SetHasEncountered(ShortFileName(file), line);
                        result = false;
                    }
                }
            }
            catch ( ... )
            {
                MessageBox(NULL, L"Cannot load " FAULT_INJECT_FILE, L"DPG Fault Injection", MB_ICONERROR); 
                return condition;
            }
        }
        return result;
    }

private:
    CComPtr<IXMLDOMDocument> DOM_;
    bool dirty_;
    wchar_t* filePath_;

    CComPtr<IXMLDOMNode> GetNodeForFileLine(const char* file, int line)
    {
        USES_CONVERSION;
        // build "/DBCCalls/DBCCall[@file=\"<file>\" and @line=\"<line>\"]" string
        std::wstring wFile{ ATL::CA2W(file) };
        wchar_t atowBuffer[34];
        _itow_s(line, atowBuffer, 34, 10);
        wstring xpath = L"/DBCCalls/DBCCall[@file=\"";
        xpath.append(wFile.c_str());
        xpath.append(L"\" and @line=\"");
        xpath.append(atowBuffer);
        xpath.append(L"\"]");

        CComBSTR bstrXPath(xpath.c_str());
        CComPtr<IXMLDOMNode> selected;
        HRESULT hr = DOM_->selectSingleNode(bstrXPath, &selected);
        if (FAILED(hr))
        {
            throw "Unable to locate 'xmlnode' XML node";
        }
        return selected;
    }

    _Ret_notnull_
    static const char* ShortFileName(const char* file)
    {
        const char* result = strrchr(file, '\\');
        if (result == NULL)
        {
            result = file;
        }
        else
        {
            result++;
        }
        return result;
    }
};