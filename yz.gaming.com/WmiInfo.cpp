#include "pch.h"
#include "wmiInfo.h"
#include <comutil.h>
#include <timeapi.h>
#include "inc/wpptrace.h"
#include "wmiinfo.tmh"

#pragma comment(lib, "winmm.lib")
#pragma comment(lib, "WbemUuid.lib")
#pragma comment(lib, "comsuppw.lib")

namespace YZ::Gaming::Com
{
	CWmiInfo::CWmiInfo(const string& rootPath)
	{
		m_pWbemSvc = NULL;
		m_pWbemLoc = NULL;
		m_rootPath = rootPath;
		InitWmi();
	}

	CWmiInfo::~CWmiInfo()
	{
		ReleaseWmi();
	}

	HRESULT CWmiInfo::InitWmi()
	{
		HRESULT hr;
		hr = ::CoInitialize(0);
		if (SUCCEEDED(hr) || RPC_E_CHANGED_MODE == hr)
		{
			//设置进程的安全级别，（调用COM组件时在初始化COM之后要调用CoInitializeSecurity设置进程安全级别，否则会被系统识别为病毒）  
			hr = ::CoInitializeSecurity(NULL,
				-1,
				NULL,
				NULL,
				RPC_C_AUTHN_LEVEL_PKT,
				RPC_C_IMP_LEVEL_IMPERSONATE,
				NULL,
				EOAC_NONE,
				NULL);

			//二、创建一个WMI命名空间连接  
			//创建一个CLSID_WbemLocator对象  
			hr = ::CoCreateInstance(CLSID_WbemLocator,
				0,
				CLSCTX_INPROC_SERVER,
				IID_IWbemLocator,
				(LPVOID*)&m_pWbemLoc);

			if (SUCCEEDED(hr))
			{
				//使用m_pWbemLoc连接到"root\cimv2"并设置m_pWbemSvc的指针  
				hr = m_pWbemLoc->ConnectServer(bstr_t(m_rootPath.c_str()),
					NULL,
					NULL,
					0,
					NULL,
					0,
					0,
					&m_pWbemSvc);

				if (SUCCEEDED(hr))
				{
					//三、设置WMI连接的安全性  
					hr = ::CoSetProxyBlanket(m_pWbemSvc,
						RPC_C_AUTHN_WINNT,
						RPC_C_AUTHZ_NONE,
						NULL,
						RPC_C_AUTHN_LEVEL_CALL,
						RPC_C_IMP_LEVEL_IMPERSONATE,
						NULL,
						EOAC_NONE);
				}
			}
		}
		return(hr);
	}

	HRESULT CWmiInfo::ReleaseWmi()
	{
		HRESULT hr = S_OK;

		if (NULL != m_pWbemSvc)
		{
			hr = m_pWbemSvc->Release();
		}
		if (NULL != m_pWbemLoc)
		{
			hr = m_pWbemLoc->Release();
		}
		::CoUninitialize();
		
		return hr;
	}

	vector<string> CWmiInfo::GetSingleItemInfo(const string& class_name, const string& class_member)
	{
		bstr_t query("SELECT * FROM ");
		VARIANT vtProp;
		HRESULT hr;
		vector<string> result;
		if (NULL != m_pWbemSvc)
		{
			IEnumWbemClassObject* penumclsobj = nullptr;
			query += class_name.c_str();
			hr = m_pWbemSvc->ExecQuery(bstr_t("WQL"), query, WBEM_FLAG_FORWARD_ONLY | WBEM_FLAG_RETURN_IMMEDIATELY,
				0, &penumclsobj);
			if (SUCCEEDED(hr))
			{
				//初始化vtProp值  
				VariantInit(&vtProp);

				IWbemClassObject* pclsobj;
				while (penumclsobj)
				{
					//返回从当前位置起的第一个对象到pclsobj中
					ULONG uReturn = 0;
					hr = penumclsobj->Next(WBEM_INFINITE, 1, &pclsobj, &uReturn);
					if (0 == uReturn)
					{
						break;
					}

					//找出ClassMember标识的成员属性值,并保存到vtProp变量中  
					hr = pclsobj->Get(bstr_t(class_member.c_str()), 0, &vtProp, 0, 0);
					if (SUCCEEDED(hr))
					{
						string val;
						VariantToString(&vtProp, val);
						VariantClear(&vtProp);//清空vtProp  
						result.push_back(val);
					}
					pclsobj->Release();
				}
				penumclsobj->Release();
			}
		}
		return result;
	}

	void CWmiInfo::VariantToString(const LPVARIANT pVar, string& chRetValue) const
	{
		HRESULT hr;

		switch (pVar->vt)
		{
		case VT_BSTR:
			chRetValue = bstr_t(pVar->bstrVal);
			break;

		case VT_BOOL:
			if (VARIANT_TRUE == pVar->boolVal)
			{
				chRetValue = "是";
			}
			else
			{
				chRetValue = "否";
			}
			break;

		case VT_I4:
			chRetValue = to_string(pVar->lVal);
			break;

		case VT_UI1:
			chRetValue = to_string(pVar->bVal);
			break;

		case VT_UI4:
			chRetValue = to_string(pVar->ulVal);
			break;

		case VT_BSTR | VT_ARRAY:
			{
				char HUGEP* pBstr;
				hr = SafeArrayAccessData(pVar->parray, (void HUGEP**) & pBstr);
				chRetValue = pBstr;
				hr = SafeArrayUnaccessData(pVar->parray);

			}
			break;

		case VT_I4 | VT_ARRAY:
			{
				LONG low, high;

				SafeArrayGetLBound(pVar->parray, 1, &low);
				SafeArrayGetUBound(pVar->parray, 1, &high);

				char HUGEP* pBuf;
				hr = SafeArrayAccessData(pVar->parray, (void HUGEP**) & pBuf);
				hr = SafeArrayUnaccessData(pVar->parray);

				high = min(high, MAX_PATH * 2 - 1);
				for (int i = low; i <= high; ++i)
				{
					char buff[3] = { 0 };
					sprintf_s(buff, "%02x", pBuf[i]);
					chRetValue += buff;
				}
			}
			break;

		default:
			break;
		}
	}

	int CWmiInfo::GetPowerMode()
	{
		if (NULL == m_pWbemSvc)
		{
			WPPTrace(TRACE_LEVEL_ERROR, WMI, L"Error in GetPowerMode: WbemSvc is NULL.");
			return -1;  // Error code indicating WbemSvc is NULL
		}

		IWbemClassObject* pClass = NULL;
		IWbemClassObject* pInParamsDefinition = NULL;
		IWbemClassObject* pOutParams = NULL;

		_bstr_t className = "PowerSwitchInterface";

		// Get the class definition
		HRESULT hr = m_pWbemSvc->GetObject(className, 0, NULL, &pClass, NULL);
		if (FAILED(hr))
		{
			WPPTrace(TRACE_LEVEL_ERROR, WMI, L"Error in GetPowerMode: Failed to get class definition due to %!HRESULT!", hr);
			return -2;  // Error code indicating failure to get class definition
		}

		// Get the method from the class definition
		hr = pClass->GetMethod(_bstr_t(L"GetPowerMode"), 0, &pInParamsDefinition, NULL);
		if (FAILED(hr))
		{
			WPPTrace(TRACE_LEVEL_ERROR, WMI, L"Error in GetPowerMode: Failed to get method from class definition.");
			pClass->Release();
			return -3;  // Error code indicating failure to get method from class
		}

		_bstr_t objectPath = "PowerSwitchInterface.InstanceName='ACPI\\PNP0C14\\IP3POWERSWITCH_0'";
		hr = m_pWbemSvc->ExecMethod(objectPath, _bstr_t(L"GetPowerMode"), 0, NULL, pInParamsDefinition, &pOutParams, NULL);
		if (FAILED(hr))
		{
			WPPTrace(TRACE_LEVEL_ERROR, WMI, L"Error in GetPowerMode: Failed to execute method.");
			pClass->Release();
			if (pInParamsDefinition)
				pInParamsDefinition->Release();
			return -4;  // Error code indicating failure to execute method
		}

		VARIANT vtProp;
		hr = pOutParams->Get(_bstr_t(L"CurrentPowerMode"), 0, &vtProp, 0, 0);
		if (FAILED(hr) || vtProp.vt != VT_UI1) // Assuming the value type is VT_UI1 (BYTE)
		{
			WPPTrace(TRACE_LEVEL_ERROR, WMI, L"Error in GetPowerMode: Failed to get CurrentPowerMode.");
			pClass->Release();
			if (pInParamsDefinition)
				pInParamsDefinition->Release();
			if (pOutParams)
				pOutParams->Release();
			return -5;  // Error code indicating failure to get CurrentPowerMode
		}

		int powerMode = static_cast<int>(vtProp.bVal);

		VariantClear(&vtProp);
		pClass->Release();
		if (pInParamsDefinition)
			pInParamsDefinition->Release();
		if (pOutParams)
			pOutParams->Release();

		return powerMode;
	}

	HRESULT CWmiInfo::SetPowerMode(BYTE modeValue)
	{
		if (!m_pWbemSvc)
		{
			WPPTrace(TRACE_LEVEL_ERROR, WMI, L"Error in SetPowerMode: WMI service is not initialized.");
			return E_FAIL;
		}

		HRESULT hr;
		IWbemClassObject* pClass = NULL;
		IWbemClassObject* pInParams = NULL;
		IWbemClassObject* pOutParams = NULL;

		_bstr_t className = "PowerSwitchInterface";

		// Get the class definition
		hr = m_pWbemSvc->GetObject(className, 0, NULL, &pClass, NULL);
		if (FAILED(hr))
		{
			WPPTrace(TRACE_LEVEL_ERROR, WMI, L"Error in SetPowerMode: Failed to get class definition.");
			return hr;
		}

		// 获取SetPowerMode方法的参数
		hr = pClass->GetMethod(_bstr_t(L"SetPowerMode"), 0, &pInParams, NULL);
		if (FAILED(hr))
		{
			WPPTrace(TRACE_LEVEL_ERROR, WMI, L"Error in SetPowerMode: Failed to get method parameters.");
			pClass->Release();
			return hr;
		}

		// 设置PowerMode参数
		VARIANT var;
		var.vt = VT_UI1;  // BYTE
		var.bVal = modeValue;
		hr = pInParams->Put(_bstr_t(L"PowerMode"), 0, &var, CIM_UINT8);
		if (FAILED(hr))
		{
			WPPTrace(TRACE_LEVEL_ERROR, WMI, L"Error in SetPowerMode: Failed to set PowerMode parameter.");
			pClass->Release();
			if (pInParams)
				pInParams->Release();
			return hr;
		}

		// 调用SetPowerMode方法
		_bstr_t objectPath = "PowerSwitchInterface.InstanceName='ACPI\\PNP0C14\\IP3POWERSWITCH_0'";
		hr = m_pWbemSvc->ExecMethod(objectPath, _bstr_t(L"SetPowerMode"), 0, NULL, pInParams, &pOutParams, NULL);
		if (FAILED(hr))
		{
			WPPTrace(TRACE_LEVEL_ERROR, WMI, L"Error in SetPowerMode: Failed to invoke method.");
			pClass->Release();
			if (pInParams)
				pInParams->Release();
			return hr;
		}

		VARIANT varResultStatus;
		hr = pOutParams->Get(_bstr_t(L"ResultStatus"), 0, &varResultStatus, NULL, NULL);
		if (FAILED(hr))
		{
			WPPTrace(TRACE_LEVEL_ERROR, WMI, L"Error in SetPowerMode: Failed to get ResultStatus from output parameters.");
			if (pClass) pClass->Release();
			if (pInParams) pInParams->Release();
			if (pOutParams) pOutParams->Release();
			return hr;
		}

		if (varResultStatus.vt == VT_I4)  // assuming ResultStatus is an integer
		{
			WPPTrace(TRACE_LEVEL_INFORMATION, WMI, L"ResultStatus in SetPowerMode: %d", varResultStatus.intVal);
		}

		// Cleanup
		if (pClass) pClass->Release();
		if (pInParams) pInParams->Release();
		if (pOutParams) pOutParams->Release();

		return S_OK;
	}
}
