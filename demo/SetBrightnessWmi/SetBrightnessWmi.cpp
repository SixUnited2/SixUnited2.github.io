// SetBrightnessWmi.cpp : This file contains the 'main' function. Program execution begins and ends there.
//

#include <iostream>

#pragma once

#include <string>
#include <vector>
#include <WbemIdl.h> 

class CWmiInfo
{
public:
	CWmiInfo(const std::string& rootPath = "root\\wmi");
	~CWmiInfo();

	// return -1 if failed
	int GetBrightness();
	HRESULT SetBrightness(int value);

private:
	std::vector<std::string> GetSingleItemInfo(const std::string& class_name, const std::string& class_member);
	void VariantToString(const LPVARIANT, std::string&) const;//将Variant类型的变量转换为CString

	HRESULT InitWmi();    //初始化WMI  
	HRESULT ReleaseWmi(); //释放  

	IWbemServices* m_pWbemSvc;
	IWbemLocator* m_pWbemLoc;
	std::string m_rootPath;
};

#include <WbemIdl.h>
#include <comutil.h>
#pragma comment( lib,"winmm.lib" )

#define MEMBER_NUM	4
#define CLASS_NUM	10

#pragma comment(lib,"WbemUuid.lib")  
#pragma comment(lib,"comsuppw.lib")

CWmiInfo::CWmiInfo(const std::string& rootPath)
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
		hr = CoInitializeSecurity(NULL,
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
		hr = CoCreateInstance(CLSID_WbemLocator,
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
				hr = CoSetProxyBlanket(m_pWbemSvc,
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
	HRESULT hr = NULL;
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

std::vector<std::string> CWmiInfo::GetSingleItemInfo(const std::string& class_name, const std::string& class_member)
{
	bstr_t query("SELECT * FROM ");
	VARIANT vtProp;
	HRESULT hr;
	std::vector<std::string> result;
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
					std::string val;
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

void CWmiInfo::VariantToString(const LPVARIANT pVar, std::string& chRetValue) const
{
	HRESULT hr;
	switch (pVar->vt)
	{
	case VT_BSTR:
	{
		chRetValue = bstr_t(pVar->bstrVal);
	}
	break;
	case VT_BOOL:
	{
		if (VARIANT_TRUE == pVar->boolVal)
			chRetValue = "是";
		else
			chRetValue = "否";
	}
	break;
	case VT_I4:
	{
		chRetValue = std::to_string(pVar->lVal);
	}
	break;
	case VT_UI1:
	{
		chRetValue = std::to_string(pVar->bVal);
	}
	break;
	case VT_UI4:
	{
		chRetValue = std::to_string(pVar->ulVal);
	}
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

int CWmiInfo::GetBrightness()
{
	std::vector<std::string>&& res = GetSingleItemInfo("WmiMonitorBrightness", "CurrentBrightness");
	if (!res.empty())
	{
		return std::stoi(res[0]);
	}
	return -1;
}

HRESULT CWmiInfo::SetBrightness(int value)
{
	HRESULT hr = WBEM_S_FALSE;

	if (NULL != m_pWbemSvc)
	{
		IEnumWbemClassObject* penumclsobj = nullptr;
		bstr_t query("SELECT * FROM WmiMonitorBrightnessMethods");
		hr = m_pWbemSvc->ExecQuery(bstr_t("WQL"), query,
			WBEM_FLAG_FORWARD_ONLY | WBEM_FLAG_RETURN_IMMEDIATELY,
			0, &penumclsobj);
		if (SUCCEEDED(hr))
		{
			IWbemClassObject* pObj;
			ULONG ulReturned;
			hr = penumclsobj->Next(WBEM_INFINITE, 1, &pObj, &ulReturned);
			if (hr == WBEM_S_NO_ERROR)
			{
				BSTR ClassPath = SysAllocString(L"WmiMonitorBrightnessMethods");
				BSTR MethodName = SysAllocString(L"WmiSetBrightness");
				IWbemClassObject* pClass = NULL;
				IWbemClassObject* pInClass = NULL;
				IWbemClassObject* pInInst = NULL;

				hr = m_pWbemSvc->GetObject(ClassPath, 0, NULL, &pClass, NULL);
				if (hr == WBEM_S_NO_ERROR)
				{
					hr = pClass->GetMethod(MethodName, 0, &pInClass, NULL);
					if (hr == WBEM_S_NO_ERROR)
					{
						hr = pInClass->SpawnInstance(0, &pInInst);
						if (hr == WBEM_S_NO_ERROR)
						{
							BSTR ArgName0 = SysAllocString(L"Timeout");
							BSTR ArgName1 = SysAllocString(L"Brightness");

							VARIANT para1;
							VariantInit(&para1);
							V_VT(&para1) = VT_BSTR;
							V_BSTR(&para1) = SysAllocString(L"0");
							VARIANT para2;
							VariantInit(&para2);
							V_VT(&para2) = VT_BSTR;
							V_BSTR(&para2) = SysAllocString(_bstr_t(std::to_string(value).c_str()));
							VARIANT pathVariable;
							VariantInit(&pathVariable);

							hr = pInInst->Put(ArgName0, 0, &para1, CIM_UINT32);
							if (hr == WBEM_S_NO_ERROR)
							{
								hr = pInInst->Put(ArgName1, 0, &para2, CIM_UINT8);
								if (hr == WBEM_S_NO_ERROR)
								{
									hr = pObj->Get(_bstr_t(L"__PATH"), 0, &pathVariable, NULL, NULL);
									if (hr == WBEM_S_NO_ERROR)
									{
										hr = m_pWbemSvc->ExecMethod(pathVariable.bstrVal, MethodName, 0, NULL, pInInst, NULL, NULL);
									}
								}
							}

							VariantClear(&para1);
							VariantClear(&para2);
							VariantClear(&pathVariable);

							SysFreeString(ArgName0);
							SysFreeString(ArgName1);
						}
					}
				}

				SysFreeString(ClassPath);
				SysFreeString(MethodName);

				if (pClass)	pClass->Release();
				if (pInInst) pInInst->Release();
				if (pInClass) pInClass->Release();
			}
		}
	}

	return hr;
}

int main()
{
	while (1)
	{
		int percent;
		std::cout << "Enter desired brightness(0~100): ";
		std::cin >> percent;

		CWmiInfo wmiInfo;
		std::cout << "Current brightness " << wmiInfo.GetBrightness() << std::endl;
		wmiInfo.SetBrightness(percent);
		std::cout << "Set brightness " << percent << std::endl;
		std::cout << "Current brightness " << wmiInfo.GetBrightness() << std::endl;
	}
}
