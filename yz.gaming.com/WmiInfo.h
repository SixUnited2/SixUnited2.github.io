#pragma once

namespace YZ::Gaming::Com
{
	class CWmiInfo
	{
	public:
		CWmiInfo(const string& rootPath = "root\\wmi");
		~CWmiInfo();

		// return -1 if failed
		int GetPowerMode();
		HRESULT SetPowerMode(BYTE modeValue);

	private:
		vector<string> GetSingleItemInfo(const std::string& class_name, const std::string& class_member);
		void VariantToString(const LPVARIANT, std::string&) const;//将Variant类型的变量转换为CString

		HRESULT InitWmi();    //初始化WMI  
		HRESULT ReleaseWmi(); //释放  

		IWbemServices* m_pWbemSvc;
		IWbemLocator* m_pWbemLoc;
		string m_rootPath;
	};
}
