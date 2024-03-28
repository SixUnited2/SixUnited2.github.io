
// Sample For Calling interfaceDlg.cpp: 实现文件
//

#include "pch.h"
#include "framework.h"
#include "Sample For Calling interface.h"
#include "Sample For Calling interfaceDlg.h"
#include "afxdialogex.h"
#include "HidUtil.h"
#ifdef _DEBUG
#define new DEBUG_NEW
#endif

#ifdef _AMD64_
	#ifdef _DEBUG
		#pragma comment(lib,".\\Lib\\x64\\CUpdateManager.lib")
	#else
		#pragma comment(lib,".\\Lib\\x64\\CUpdateManager.lib")
	#endif
#else
	#ifdef _DEBUG
		#pragma comment(lib,".\\Lib\\x86\\debug\\CUpdateManager.lib")
	#else
		#pragma comment(lib,".\\Lib\\x86\\release\\CUpdateManager.lib")
	#endif
#endif

// 用于应用程序“关于”菜单项的 CAboutDlg 对话框

class CAboutDlg : public CDialogEx
{
public:
	CAboutDlg();

// 对话框数据
#ifdef AFX_DESIGN_TIME
	enum { IDD = IDD_ABOUTBOX };
#endif

	protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV 支持

// 实现
protected:
	DECLARE_MESSAGE_MAP()
};

CAboutDlg::CAboutDlg() : CDialogEx(IDD_ABOUTBOX)
{
}

void CAboutDlg::DoDataExchange(CDataExchange* pDX)
{
	CDialogEx::DoDataExchange(pDX);
}

BEGIN_MESSAGE_MAP(CAboutDlg, CDialogEx)
END_MESSAGE_MAP()


// CSampleForCallinginterfaceDlg 对话框
#define Thread_End 1111
//线程函数
UINT UpdateThread(LPVOID params)
{
	CUpdateManager* pUpdate = (CUpdateManager*)params;
	pUpdate->startUpdate();
	return Thread_End;
}

CSampleForCallinginterfaceDlg::CSampleForCallinginterfaceDlg(CWnd* pParent /*=nullptr*/)
	: CDialogEx(IDD_SAMPLE_FOR_CALLING_INTERFACE_DIALOG, pParent)
{
	m_hIcon = AfxGetApp()->LoadIcon(IDR_MAINFRAME);
}

void CSampleForCallinginterfaceDlg::DoDataExchange(CDataExchange* pDX)
{
	CDialogEx::DoDataExchange(pDX);
	DDX_Control(pDX, IDC_PROGRESS1, m_Progress);
}

BEGIN_MESSAGE_MAP(CSampleForCallinginterfaceDlg, CDialogEx)
	ON_WM_SYSCOMMAND()
	ON_WM_PAINT()
	ON_WM_QUERYDRAGICON()
	ON_BN_CLICKED(IDC_BUTTON1, &CSampleForCallinginterfaceDlg::OnBnClickedButton1)
	ON_WM_DESTROY()
END_MESSAGE_MAP()


// CSampleForCallinginterfaceDlg 消息处理程序

BOOL CSampleForCallinginterfaceDlg::OnInitDialog()
{
	CDialogEx::OnInitDialog();

	// 将“关于...”菜单项添加到系统菜单中。

	// IDM_ABOUTBOX 必须在系统命令范围内。
	ASSERT((IDM_ABOUTBOX & 0xFFF0) == IDM_ABOUTBOX);
	ASSERT(IDM_ABOUTBOX < 0xF000);

	CMenu* pSysMenu = GetSystemMenu(FALSE);
	if (pSysMenu != nullptr)
	{
		BOOL bNameValid;
		CString strAboutMenu;
		bNameValid = strAboutMenu.LoadString(IDS_ABOUTBOX);
		ASSERT(bNameValid);
		if (!strAboutMenu.IsEmpty())
		{
			pSysMenu->AppendMenu(MF_SEPARATOR);
			pSysMenu->AppendMenu(MF_STRING, IDM_ABOUTBOX, strAboutMenu);
		}
	}

	// 设置此对话框的图标。  当应用程序主窗口不是对话框时，框架将自动
	//  执行此操作
	SetIcon(m_hIcon, TRUE);			// 设置大图标
	SetIcon(m_hIcon, FALSE);		// 设置小图标

	// TODO: 在此添加额外的初始化代码
	updateThread = NULL;
	m_Progress.SetRange(0, 100);
	m_Progress.SetPos(0);

	return TRUE;  // 除非将焦点设置到控件，否则返回 TRUE
}

void CSampleForCallinginterfaceDlg::OnSysCommand(UINT nID, LPARAM lParam)
{
	if ((nID & 0xFFF0) == IDM_ABOUTBOX)
	{
		CAboutDlg dlgAbout;
		dlgAbout.DoModal();
	}
	else
	{
		CDialogEx::OnSysCommand(nID, lParam);
	}
}

// 如果向对话框添加最小化按钮，则需要下面的代码
//  来绘制该图标。  对于使用文档/视图模型的 MFC 应用程序，
//  这将由框架自动完成。

void CSampleForCallinginterfaceDlg::OnPaint()
{
	if (IsIconic())
	{
		CPaintDC dc(this); // 用于绘制的设备上下文

		SendMessage(WM_ICONERASEBKGND, reinterpret_cast<WPARAM>(dc.GetSafeHdc()), 0);

		// 使图标在工作区矩形中居中
		int cxIcon = GetSystemMetrics(SM_CXICON);
		int cyIcon = GetSystemMetrics(SM_CYICON);
		CRect rect;
		GetClientRect(&rect);
		int x = (rect.Width() - cxIcon + 1) / 2;
		int y = (rect.Height() - cyIcon + 1) / 2;

		// 绘制图标
		dc.DrawIcon(x, y, m_hIcon);
	}
	else
	{
		CDialogEx::OnPaint();
	}
}

//当用户拖动最小化窗口时系统调用此函数取得光标
//显示。
HCURSOR CSampleForCallinginterfaceDlg::OnQueryDragIcon()
{
	return static_cast<HCURSOR>(m_hIcon);
}

//升级进度回调函数实现
void upDateProgress(void* caller, int value)
{
	//do something 
	((CSampleForCallinginterfaceDlg*)caller)->m_Progress.SetPos(value);
	//TRACE(_T("upDataProgress:%d"),value);
}

//升级状态回调函数实现
void upDateResultStatus(void* caller, int status)
{
	//do something
	if (status == 0) {
		::MessageBox(((CSampleForCallinginterfaceDlg*)caller)->m_hWnd, _T("升级成功。"), _T(""), 0);
	}
	else {
		CString strStatus;
		strStatus.Format(_T("升级失败! status:%d"), status);
		::MessageBox(((CSampleForCallinginterfaceDlg*)caller)->m_hWnd, strStatus, _T(""), 0);
	}
}


void CSampleForCallinginterfaceDlg::OnBnClickedButton1()
{
	// TODO: 在此添加控件通知处理程序代码
	USHORT version=0;
	HANDLE hdev = OpenUsbDevice(CUpdateManager::Vid, CUpdateManager::Gp_Pid,0x0001,0xff00);
	if (hdev != NULL) {
		//获聀固件（游戏控制器）版本号
		version = getFirmwareVersion(hdev);
	}

	char szFilePath[MAX_PATH + 1];
	GetModuleFileNameA(NULL, szFilePath, MAX_PATH);
	CStringA Dir = szFilePath;
	int last = Dir.ReverseFind('\\');
	Dir = Dir.Left(last);//Dir为exe文件的绝对路径
	Dir = Dir + "\\UpdatePkg\\updataPkg.hjz";
	char* upDateFilePath= Dir.GetBuffer(Dir.GetLength());
	Dir.ReleaseBuffer();
	m_upMgr.initParam(upDateFilePath, this, upDateProgress, upDateResultStatus);
	//获取升级包内升级文件版本号
	USHORT fileVer = m_upMgr.getDeviceVersionInUpdatePackage();
	if (fileVer >= version) //比较是否有更新的版本须要升级
	{
		if (hdev != NULL) {

			UCHAR RdBuf[65];
			memset(RdBuf, 0, 65);
			bool ret_val;
			//BootLoad 进升级设备
			ret_val = bootLoadUpdateDevice(hdev);
			if (!ret_val) {
				bootLoadUpdateDevice(hdev);
			}
			CloseHandle(hdev);
			hdev = NULL;
			Sleep(1000);

		}
		else {
			//在升级失败情况下设备一直处在升级设备无需进入BootLoad,由动态库枚举并打开设备进行升级
			//在升级失败情况，version=0 可设置为0,必须进入升级否则设备不能正常使用
		}

		updateThread = AfxBeginThread(UpdateThread, &m_upMgr, 0, 0, 0);
	}

}

HANDLE CSampleForCallinginterfaceDlg::OpenUsbDevice(USHORT vid, USHORT pid, USHORT usage, USHORT usagePage)
{
	DWORD Instance = 0;
	//BOOLEAN ret_val;
	GUID HidGuid;
	HIDD_ATTRIBUTES HidAttr;
	HidD_GetHidGuid(&HidGuid);//open device
	HANDLE hDevice = NULL;
	while (1) {
		hDevice = GetDeviceViaInterface(&HidGuid, Instance++, NULL);	// keyboard mouse等HID设备不不能打开
		if (hDevice == NULL) {
			return 0;
		}
		PHIDP_PREPARSED_DATA pp_data = NULL;
		HIDP_CAPS caps;
		BOOLEAN res;
		NTSTATUS nt_res;
		bool isNeedDevice = false;
		if (HidD_GetAttributes(hDevice, &HidAttr) == TRUE) {	//Get usb hid attributes
			if ((HidAttr.VendorID == vid) && (HidAttr.ProductID == pid)) {
				res = HidD_GetPreparsedData(hDevice, &pp_data);
				if (res) {
					nt_res = HidP_GetCaps(pp_data, &caps);
					if (nt_res == HIDP_STATUS_SUCCESS) {
						if (usage == caps.Usage && usagePage == caps.UsagePage) {
							isNeedDevice = true;
							HidD_FreePreparsedData(pp_data);
							break;
						}
					}
					HidD_FreePreparsedData(pp_data);
				}
				break;
			}
			if (!isNeedDevice) {
				CloseHandle(hDevice);
				hDevice = NULL;
			}
		}
	}
	return hDevice;
}
bool CSampleForCallinginterfaceDlg::bootLoadUpdateDevice(HANDLE hdev){

	bool res = false;
	UCHAR RdBuf[65];
	memset(RdBuf, 0, 65);

	res = HidD_GetIndexedString(hdev, 0xFE, &RdBuf, (0x10 - 2 + 1));
	if (!res) {
		return res;
	}
	res = HidD_GetIndexedString(hdev, 0xFE, &RdBuf, (0x10 - 2 + 2));
	if (!res) {
		return res;
	}
	res = HidD_GetIndexedString(hdev, 0xFE, &RdBuf, (0x10 - 2 + 3));
	if (!res) {
		return res;
	}
	res = HidD_GetIndexedString(hdev, 0xFE, &RdBuf, (0x10 - 2 + 4));
	if (!res) {
		return res;
	}
	//ret_val = HidD_GetIndexedString(hDevice,0xFE,&RdBuf,(0x10-2+5));
	//if(!ret_val){
	//	CloseHandle(hDevice); 
	//	hDevice = NULL;
	//	return 0;
	//}
	res = true;
	return res;
}
USHORT CSampleForCallinginterfaceDlg::getFirmwareVersion(HANDLE hdev) {

	USHORT res = -1;
	UCHAR RdBuf[65];
	memset(RdBuf, 0, 65);

	res = HidD_GetIndexedString(hdev, 0xFE, &RdBuf, (0x10 - 2));
	if (!res) {
		return -1;
	}
	return RdBuf[0]<<8| RdBuf[1];
}
void CSampleForCallinginterfaceDlg::OnDestroy()
{
	CDialogEx::OnDestroy();
	// TODO: 在此处添加消息处理程序代码
	if (updateThread != NULL) {
		DWORD exitcode=0;
		if (GetExitCodeThread(updateThread->m_hThread, &exitcode)) {
			if (exitcode == STILL_ACTIVE) {
				//m_upMgr.stopUpdate();

				DWORD res = WaitForSingleObject(updateThread->m_hThread, 1000);
				if (res == WAIT_TIMEOUT) {
					DWORD exitcode;
					GetExitCodeThread(updateThread->m_hThread, &exitcode);
					TerminateThread(updateThread->m_hThread, Thread_End);
					CloseHandle(updateThread->m_hThread);
				}
			}
		}
	}
	else {
	}
}
