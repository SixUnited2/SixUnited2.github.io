// WmiPowerModeSwitcherCpp.cpp : This file contains the 'main' function. Program execution begins and ends there.
//
#include <iostream>
#include <Windows.h>
#include <wbemidl.h>
#include <comdef.h>

#include <iostream>

#pragma once

#include <string>
#include <vector>
#include <WbemIdl.h> 
#include <WbemIdl.h>
#include <comutil.h>
#pragma comment( lib,"winmm.lib" )
#pragma comment(lib,"WbemUuid.lib")  
#pragma comment(lib,"comsuppw.lib")


class CWmiInfo
{
public:
	CWmiInfo(const std::string& rootPath = "root\\wmi");
	~CWmiInfo();

	// return -1 if failed
	int GetPowerMode();
	HRESULT SetPowerMode(BYTE modeValue);

private:
	std::vector<std::string> GetSingleItemInfo(const std::string& class_name, const std::string& class_member);
	void VariantToString(const LPVARIANT, std::string&) const;//��Variant���͵ı���ת��ΪCString

	HRESULT InitWmi();    //��ʼ��WMI  
	HRESULT ReleaseWmi(); //�ͷ�  

	IWbemServices* m_pWbemSvc;
	IWbemLocator* m_pWbemLoc;
	std::string m_rootPath;
};

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
		//���ý��̵İ�ȫ���𣬣�����COM���ʱ�ڳ�ʼ��COM֮��Ҫ����CoInitializeSecurity���ý��̰�ȫ���𣬷���ᱻϵͳʶ��Ϊ������  
		hr = CoInitializeSecurity(NULL,
			-1,
			NULL,
			NULL,
			RPC_C_AUTHN_LEVEL_PKT,
			RPC_C_IMP_LEVEL_IMPERSONATE,
			NULL,
			EOAC_NONE,
			NULL);

		//��������һ��WMI�����ռ�����  
		//����һ��CLSID_WbemLocator����  
		hr = CoCreateInstance(CLSID_WbemLocator,
			0,
			CLSCTX_INPROC_SERVER,
			IID_IWbemLocator,
			(LPVOID*)&m_pWbemLoc);

		if (SUCCEEDED(hr))
		{
			//ʹ��m_pWbemLoc���ӵ�"root\cimv2"������m_pWbemSvc��ָ��  
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
				//��������WMI���ӵİ�ȫ��  
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
			//��ʼ��vtPropֵ  
			VariantInit(&vtProp);

			IWbemClassObject* pclsobj;
			while (penumclsobj)
			{
				//���شӵ�ǰλ����ĵ�һ������pclsobj��
				ULONG uReturn = 0;
				hr = penumclsobj->Next(WBEM_INFINITE, 1, &pclsobj, &uReturn);
				if (0 == uReturn)
				{
					break;
				}

				//�ҳ�ClassMember��ʶ�ĳ�Ա����ֵ,�����浽vtProp������  
				hr = pclsobj->Get(bstr_t(class_member.c_str()), 0, &vtProp, 0, 0);
				if (SUCCEEDED(hr))
				{
					std::string val;
					VariantToString(&vtProp, val);
					VariantClear(&vtProp);//���vtProp  
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
			chRetValue = "��";
		else
			chRetValue = "��";
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

int CWmiInfo::GetPowerMode()
{
	if (NULL == m_pWbemSvc)
	{
		std::cout << "Error in GetPowerMode: WbemSvc is NULL." << std::endl;
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
		std::cout << "Error in GetPowerMode: Failed to get class definition." << std::endl;
		return -2;  // Error code indicating failure to get class definition
	}

	// Get the method from the class definition
	hr = pClass->GetMethod(_bstr_t(L"GetPowerMode"), 0, &pInParamsDefinition, NULL);
	if (FAILED(hr))
	{
		std::cout << "Error in GetPowerMode: Failed to get method from class definition." << std::endl;
		pClass->Release();
		return -3;  // Error code indicating failure to get method from class
	}

	_bstr_t objectPath = "PowerSwitchInterface.InstanceName='ACPI\\PNP0C14\\IP3POWERSWITCH_0'";
	hr = m_pWbemSvc->ExecMethod(objectPath, _bstr_t(L"GetPowerMode"), 0, NULL, pInParamsDefinition, &pOutParams, NULL);
	if (FAILED(hr))
	{
		std::cout << "Error in GetPowerMode: Failed to execute method." << std::endl;
		pClass->Release();
		if (pInParamsDefinition)
			pInParamsDefinition->Release();
		return -4;  // Error code indicating failure to execute method
	}

	VARIANT vtProp;
	hr = pOutParams->Get(_bstr_t(L"CurrentPowerMode"), 0, &vtProp, 0, 0);
	if (FAILED(hr) || vtProp.vt != VT_UI1) // Assuming the value type is VT_UI1 (BYTE)
	{
		std::cout << "Error in GetPowerMode: Failed to get CurrentPowerMode." << std::endl;
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
		std::cout << "Error in SetPowerMode: WMI service is not initialized." << std::endl;
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
		std::cout << "Error in SetPowerMode: Failed to get class definition." << std::endl;
		return hr;
	}

	// ��ȡSetPowerMode�����Ĳ���
	hr = pClass->GetMethod(_bstr_t(L"SetPowerMode"), 0, &pInParams, NULL);
	if (FAILED(hr))
	{
		std::cout << "Error in SetPowerMode: Failed to get method parameters." << std::endl;
		pClass->Release();
		return hr;
	}

	// ����PowerMode����
	VARIANT var;
	var.vt = VT_UI1;  // BYTE
	var.bVal = modeValue;
	hr = pInParams->Put(_bstr_t(L"PowerMode"), 0, &var, CIM_UINT8);
	if (FAILED(hr))
	{
		std::cout << "Error in SetPowerMode: Failed to set PowerMode parameter." << std::endl;
		pClass->Release();
		if (pInParams)
			pInParams->Release();
		return hr;
	}

	// ����SetPowerMode����
	_bstr_t objectPath = "PowerSwitchInterface.InstanceName='ACPI\\PNP0C14\\IP3POWERSWITCH_0'";
	hr = m_pWbemSvc->ExecMethod(objectPath, _bstr_t(L"SetPowerMode"), 0, NULL, pInParams, &pOutParams, NULL);
	if (FAILED(hr))
	{
		std::cout << "Error in SetPowerMode: Failed to invoke method." << std::endl;
		pClass->Release();
		if (pInParams)
			pInParams->Release();
		return hr;
	}

	VARIANT varResultStatus;
	hr = pOutParams->Get(_bstr_t(L"ResultStatus"), 0, &varResultStatus, NULL, NULL);
	if (FAILED(hr))
	{
		std::cout << "Error in SetPowerMode: Failed to get ResultStatus from output parameters." << std::endl;
		if (pClass) pClass->Release();
		if (pInParams) pInParams->Release();
		if (pOutParams) pOutParams->Release();
		return hr;
	}

	if (varResultStatus.vt == VT_I4)  // assuming ResultStatus is an integer
	{
		std::cout << "ResultStatus in SetPowerMode: " << varResultStatus.intVal << std::endl;
	}

	// Cleanup
	if (pClass) pClass->Release();
	if (pInParams) pInParams->Release();
	if (pOutParams) pOutParams->Release();

	return S_OK;
}

int main() {
	CWmiInfo wmiInfo;

	while (1) {
		std::cout << "Choose command [get|set]: ";
		std::string command;
		std::cin >> command;

		if (command == "get") {
			BYTE mode = wmiInfo.GetPowerMode();
			switch (mode) {
			case 0x00: std::cout << "Balance mode" << std::endl; break;
			case 0x01: std::cout << "Performance mode" << std::endl; break;
			case 0x02: std::cout << "Quiet mode" << std::endl; break;
			case 0x03: std::cout << "Super mode" << std::endl; break;
			default: std::cout << "Unknown mode" << std::endl; break;
			}
		}
		else if (command == "set") {
			std::cout << "Enter mode value (0 for Balance, 1 for Performance, 2 for Quiet, 3 for Super): ";
			int modeValue;
			std::cin >> modeValue;

			if (modeValue >= 0 && modeValue <= 3) {
				wmiInfo.SetPowerMode((BYTE)modeValue);
				std::cout << "Power mode set." << std::endl;
			}
			else {
				std::cout << "Invalid mode value." << std::endl;
			}
		}
		else {
			std::cout << "Invalid command." << std::endl;
		}
	}

	CoUninitialize();
	return 0;
}
