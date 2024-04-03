#pragma once
#include <windows.h>

namespace YZ::Gaming::Com
{
    constexpr auto REPORTID_1 = 0x01;
    constexpr auto REPORTID_2 = 0x02;
    constexpr auto REPORTID_3 = 0x03;

    constexpr auto PROFILENUMBER = 66;
    constexpr auto MAP_KEY_MAX = 20;
    constexpr auto MAP_STEP_MAX = 20;

    //0->select物理键
    //1->L2物理键
    //2->L1物理键
    //3->L3D_UP物理键
    //4->L3D_DOWN物理键
    //5->L3D_LEFT物理键
    //6->L3D_RIGHT物理键
    //7->L3物理键
    //8->DPAD_UP物理键
    //9->DPAD-DOWN物理键
    //10->DPAD-LEFT物理键
    //11->DPAD-RIGHT物理键
    //12->start物理键
    //13->R2物理键
    //14->R1物理键
    //15->A物理键
    //16->B物理键
    //17->X物理键
    //18->Y物理键
    //19->R3物理键
    typedef union
    {
        UINT32 data[PROFILENUMBER];      //PROFILENUMBER: 66
        struct
        {
            struct   //ensure 32bit(4 bytes) every struct
            {
                UINT32 light_type : 3;          //0:单色常亮，1:单色呼吸，2:RGB呼吸，3:RGB循环，4:RGB波浪
                UINT32 light_rgb : 24;          //R:8 G:8 B:8
                UINT32 light_intensity : 4;     //0:关闭，1~5档
                UINT32 reserve : 1;
            } config_base_index;

            struct   //ensure 32bit(4 bytes) every struct
            {
                UINT32 motor_intensity : 3;  //0:关闭，1:弱，2:强
                UINT32 controller_mode : 1;  //0:手柄，1键鼠
                UINT32 ns_position : 1;      //0:XBOX按键布局 1：NS按键布局
                UINT32 reserve : 27;
            } config_base_index1;

            struct   //ensure 32bit(4 bytes) every struct
            {
                UINT32 A_turbo : 1;     //0:关闭   1:开启
                UINT32 B_turbo : 1;     //0:关闭   1:开启
                UINT32 X_turbo : 1;     //0:关闭   1:开启
                UINT32 Y_turbo : 1;     //0:关闭   1:开启
                UINT32 L1_turbo : 1;    //0:关闭   1:开启
                UINT32 R1_turbo : 1;    //0:关闭   1:开启
                UINT32 turbo_open : 1;  //0:关闭   1:开启
                UINT32 reserve : 25;
            } config_turbo;

            struct   //ensure 32bit(4 bytes) every struct
            {
                UINT32  Lstick_deadzone_value : 8;      //0~20 （中心死区%）
                UINT32  Rstick_deadzone_value : 8;      //0~20 （中心死区%）
                UINT32  left_stick_sensitivity : 3;     //0~5
                UINT32  right_stick_sensitivity : 3;    //0~5
                UINT32  left_trigger_sensitivity : 3;   //0~5
                UINT32  right_trigger_sensitivity : 3;  //0~5
                UINT32  enter_stick_cal : 1;            //0:退出摇杆校准 1:进入摇杆校准
                UINT32  stick_cal_sucess : 1;           //0:摇杆校准失败 1:摇杆校准成功
                UINT32  enter_trigger_cal : 1;          //0:退出扳机校准 1:进入扳机校准
                UINT32  trigger_cal_sucess : 1;         //0:扳机校准失败 1:扳机校准成功 
            } config_stick_trigger;

            struct
            {
                UINT32 mapping : 8;  //按键映射键码
                UINT32 reserve : 24;
            } mk_mode_keydata[MAP_KEY_MAX];  //MAP_KEY_MAX->20

            struct
            {
                UINT32 mapping : 8;                     //映射键码
                UINT32 map_controller_or_keyboard : 1;  //1->映射成手柄键码  0->映射成键盘、鼠标键码
                UINT32 next_step : 8;                   //下一步骤
                UINT32 reserved : 15;                   //预留
                UINT32 mappingtime : 16;                //映射值持续时间，单位为ms
                UINT32 intervaltime_with_next_step : 16;//与下一步映射值触发时的相隔时间,单位为ms
            } bk_macro_step[MAP_STEP_MAX];              //暂且定义有20个步骤空间

            struct
            {
                UINT32 start_step : 8;                  //该套宏的起始步
                UINT32 end_step : 8;                    //该套宏的结束步
                UINT32 cycleflag : 1;                   //该套宏的循环执行标志
                UINT32 cycleIntervaltime : 15;          //宏步骤执行完后到重新执行前的相隔时间，单位为ms
            } bk_macro[BACK_KEY_NUMBER];                //背键数量->2，暂且定义一个背键存一套宏。如背键M1存放一套有5个步骤
                                                        //的宏，包含步骤0~步骤4，背键M2可以存放一套跟M1相同的宏（步骤0~步骤4），也可以重新设一套，包含步骤5~步骤n（n最大为19，一共20个步骤空间）。
        };
    } Profile;

    enum class ProfileCmd : BYTE
    {
        R_INFO = 0x10,			//预读Profile命令，下位机应答相关信息，版本号等
        R_PF_DATA = 0x11,		//读完整Profile数据
        R_V_DATA = 0x12,		//校验读取的完整Profile数据
        W_INFO = 0x20,			//预读写Profile命令，下位机应答相关信息，版本号等
        W_PF_DATA = 0x21,		//写完整Profile数据
        W_V_DATA = 0x22,		//校验写完整Profile数据
        W_S_DATA = 0x23     	//保存写完整Profile数据（通知下位机保存到Flash中）
    };
}
