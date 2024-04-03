#ifndef CUPDATEMANAGER_H
#define CUPDATEMANAGER_H

//////////////////
typedef void (*CallBack_Funtion)(void* caller, int value); //函数指针定义

class __declspec(dllimport) CUpdateManager
{

public:

    enum DEV_VID { Vid = 0x2f24 };
    enum DEV_PID {
        Gp_Pid = 0x0135,//编程设备PID
        UpData_Pid = 0x0001, //升级设备PID
    };
    enum GAMEPAD_UPDATEID {
        UpdateId = 0x026B//手柄内手柄芯片升级ID
    };

    int getDeviceVersionInUpdatePackage();
    int getDeviceUpdateIdInUpdatePackage();
    bool initParam(char* filePath = nullptr, void* caller = nullptr, void* updateProgress = nullptr, void* updateResultStatus = nullptr);

    void startUpdate();
    static void stopUpdate();

    ////0升级成功 ,负数表示升级失败，
    //            //注：-1:升级包与设备不匹配 -2：升级包不存在
    //            //-3:Device version Read Error
    //            //-4:Device Rom Erase error
    //            //-5:Device Rom Update error
    //            //-6:Device Rom Erase error
    //            //-7:Device Rom Update error
    //            //-8:Device Rom Write error
    //            //-9:Device Checksum Read error
    //            //-10:Checksum error
    //            //-11:Device Rom Erase error
    //            //-12:Device Rom Update error
    //            //-13:Device Rom Write error
    //            //-14:Device Checksum Read error
    //            //-15:Checksum error
    //            //-16:Device Reset failed
    //            //-17:Accidental device removal

    void ResetUpdateDevice();
};


#endif // CUPDATEMANAGER_H
