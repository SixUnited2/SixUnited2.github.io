
// Sample For Calling interfaceDlg.h: 头文件
//

#pragma once
#include "CUpdateManager.h"
// CSampleForCallinginterfaceDlg 对话框
class CSampleForCallinginterfaceDlg : public CDialogEx
{
// 构造
public:
	CSampleForCallinginterfaceDlg(CWnd* pParent = nullptr);	// 标准构造函数

// 对话框数据
#ifdef AFX_DESIGN_TIME
	enum { IDD = IDD_SAMPLE_FOR_CALLING_INTERFACE_DIALOG };
#endif

	protected:
	virtual void DoDataExchange(CDataExchange* pDX);	// DDX/DDV 支持


// 实现
protected:
	HICON m_hIcon;

	// 生成的消息映射函数
	virtual BOOL OnInitDialog();
	afx_msg void OnSysCommand(UINT nID, LPARAM lParam);
	afx_msg void OnPaint();
	afx_msg HCURSOR OnQueryDragIcon();
	DECLARE_MESSAGE_MAP()
public:
	afx_msg void OnBnClickedButton1();

	CWinThread* updateThread;

	CProgressCtrl m_Progress;


	afx_msg void OnDestroy();
private:
	HANDLE OpenUsbDevice(USHORT vid, USHORT pid, USHORT usage, USHORT usagePage);
	bool bootLoadUpdateDevice(HANDLE hdev);
	USHORT getFirmwareVersion(HANDLE hdev);
	CUpdateManager m_upMgr;
};
