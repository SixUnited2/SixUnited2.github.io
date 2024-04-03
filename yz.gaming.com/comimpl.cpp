#include "pch.h"
#include "comimpl.h"
#include "profileoperation.h"
#include "inc/wpptrace.h"
#include "comimpl.tmh"

using namespace YZ::Gaming::Com;

namespace
{
    unique_ptr<ProfileOperation> profileOp{ nullptr };
}

YZGAMINGCOM_API
DWORD
YZGamingCom_Initialize(
    PFN_PROFILEDEVICEEVENTHANDLER pfnProfileDeviceEventHandler,
    LPVOID                        lpContext
    )
{
    DWORD dwRetVal = ERROR_SUCCESS;
    
    if (profileOp = make_unique<ProfileOperation>(pfnProfileDeviceEventHandler, lpContext);
        bool{ profileOp })
    {
        if (!profileOp->Initialize())
        {
            dwRetVal = ERROR_OPEN_FAILED;
            WPPTrace(TRACE_LEVEL_ERROR, INIT, L"Failed in initializing the ProfileOperation object");

            profileOp.reset();
        }
        else
        {
            WPPTrace(TRACE_LEVEL_INFORMATION, INIT, L"The ProfileOperation object is initialized");
        }
    }
    else
    {
        dwRetVal = ERROR_CREATE_FAILED;
        WPPTrace(TRACE_LEVEL_ERROR, INIT, L"Failed in instantiating the ProfileOperation object");
    }

    return dwRetVal;
}

YZGAMINGCOM_API
DWORD
YZGamingCom_ReadProfileData(
    HPROFILEDEVICE hProfileDevice,
    LPPROFILEDATA lpProfileData
    )
{
    DWORD dwRetVal = ERROR_SUCCESS;

    if (hProfileDevice == nullptr && lpProfileData == nullptr)
    {
        dwRetVal = ERROR_INVALID_PARAMETER;
        WPPTrace(TRACE_LEVEL_ERROR, PROFILE, L"Either hProfileDevice is null or lpProfileData is null");
        return dwRetVal;
    }

    if (!bool{ profileOp })
    {
        dwRetVal = ERROR_RESOURCE_NOT_AVAILABLE;
        WPPTrace(TRACE_LEVEL_ERROR, PROFILE, L"The profile operation object is not initialized yet");
        return dwRetVal;
    }

    Profile profile{ 0 };
    DWORD fwVersion = 0;
    if (auto profileDevice{ reinterpret_cast<ProfileDevice*>(hProfileDevice) };
        profileDevice->ReadProfileData(0, profile, fwVersion))
    {
        lpProfileData->lightType = static_cast<PROFILEDATA::LIGHTTYPE>(profile.config_base_index.light_type);
        lpProfileData->lightRGB[0] = (profile.config_base_index.light_rgb & 0xff0000) >> 16; // red
        lpProfileData->lightRGB[1] = (profile.config_base_index.light_rgb & 0x00ff00) >> 8; // green
        lpProfileData->lightRGB[2] = (profile.config_base_index.light_rgb & 0x0000ff);      // blue
        lpProfileData->lightIntensity = profile.config_base_index.light_intensity;
        lpProfileData->motorIntensity = profile.config_base_index1.motor_intensity;
        lpProfileData->controllerMode = profile.config_base_index1.controller_mode;
        lpProfileData->nsPosition = profile.config_base_index1.ns_position;
        lpProfileData->turboA = profile.config_turbo.A_turbo;
        lpProfileData->turboB = profile.config_turbo.B_turbo;
        lpProfileData->turboX = profile.config_turbo.X_turbo;
        lpProfileData->turboY = profile.config_turbo.Y_turbo;
        lpProfileData->turboL1 = profile.config_turbo.L1_turbo;
        lpProfileData->turboR1 = profile.config_turbo.R1_turbo;
        lpProfileData->turboOpen = profile.config_turbo.turbo_open;
        lpProfileData->lStickDeadZoneValue = profile.config_stick_trigger.Lstick_deadzone_value;
        lpProfileData->rStickDeadZoneValue = profile.config_stick_trigger.Rstick_deadzone_value;
        lpProfileData->lStickSensitivity = profile.config_stick_trigger.left_stick_sensitivity;
        lpProfileData->rStickSensitivity = profile.config_stick_trigger.right_stick_sensitivity;
        lpProfileData->lTriggerSensitivity = profile.config_stick_trigger.left_trigger_sensitivity;
        lpProfileData->rTriggerSensitivity = profile.config_stick_trigger.right_trigger_sensitivity;
        lpProfileData->stickCalibration = profile.config_stick_trigger.enter_stick_cal;
        lpProfileData->stickCalibrationSucess = profile.config_stick_trigger.stick_cal_sucess;
        lpProfileData->triggerCalibration = profile.config_stick_trigger.enter_trigger_cal;
        lpProfileData->triggerCalibrationSucess = profile.config_stick_trigger.trigger_cal_sucess;
        for (int i = 0; i < KEYCODE::MAX_MAPPING_KEYS; i++)
        {
            lpProfileData->keys[i] = static_cast<KEYCODE>(profile.mk_mode_keydata[i].mapping);
        }
        lpProfileData->fwVersion = fwVersion;

        for (int i = 0; i < BACK_KEY_NUMBER; i++)
        {
            lpProfileData->bkMacro[i].cycleFlag = profile.bk_macro[i].cycleflag;
            lpProfileData->bkMacro[i].cycleInterval = profile.bk_macro[i].cycleIntervaltime;

            auto tempStep = &lpProfileData->bkMacro[i].step;
            auto start = &profile.bk_macro_step[profile.bk_macro[i].start_step];
            auto end = &profile.bk_macro_step[profile.bk_macro[i].end_step];
            for (auto step = start; step <= end; step++)
            {
                PROFILEDATA::BKMACRO::BKMACROSTEP* newStep = nullptr;

                if (step != end)
                {
                    auto next = step + 1;

                    newStep = new PROFILEDATA::BKMACRO::BKMACROSTEP;
                    newStep->key = next->mapping;
                    newStep->interval = next->intervaltime_with_next_step;
                    newStep->type = next->map_controller_or_keyboard;
                    newStep->time = next->mappingtime;
                    newStep->next = nullptr;
                }

                tempStep->key = step->mapping;
                tempStep->interval = step->intervaltime_with_next_step;
                tempStep->time = step->mappingtime;
                tempStep->type = step->map_controller_or_keyboard;
                tempStep->next = newStep;

                tempStep = tempStep->next;
            }
        }

        WPPTrace(TRACE_LEVEL_INFORMATION, PROFILE, L"Succeeded in reading profile data");
    }
    else
    {
        dwRetVal = ERROR_READ_FAULT;
        WPPTrace(TRACE_LEVEL_ERROR, PROFILE, L"Read profile data failed");
    }

    return dwRetVal;
}

VOID YZGamingCom_ReleaseProfileDataForRead(LPPROFILEDATA lpProfileData)
{
    if (lpProfileData != nullptr)
    {
        for (int i = 0; i < BACK_KEY_NUMBER; i++)
        {
            auto step = lpProfileData->bkMacro[i].step.next;
            while (step != nullptr)
            {
                auto s = step;
                step = step->next;
                delete s;
            }
        }
    }
}

DWORD YZGamingCom_WriteProfileData(
    HPROFILEDEVICE hProfileDevice,
    LPPROFILEDATA lpProfileData
    )
{
    DWORD dwRetVal = ERROR_SUCCESS;

    if (hProfileDevice == nullptr && lpProfileData == nullptr)
    {
        dwRetVal = ERROR_INVALID_PARAMETER;
        WPPTrace(TRACE_LEVEL_ERROR, INIT, L"Either hProfileDevice is null or lpProfileData is null");
        return dwRetVal;
    }

    if (!bool{ profileOp })
    {
        dwRetVal = ERROR_RESOURCE_NOT_AVAILABLE;
        WPPTrace(TRACE_LEVEL_ERROR, INIT, L"The profile operation object is not initialized yet");
        return dwRetVal;
    }

    int stepNumber = 0;
    for (int i = 0; i < BACK_KEY_NUMBER; i++)
    {
        for (auto step = &lpProfileData->bkMacro[i].step; step != nullptr; step = step->next)
        {
            stepNumber++;
        }
    }

    if (stepNumber > MAP_STEP_MAX)
    {
        dwRetVal = ERROR_NOT_ENOUGH_QUOTA;
        WPPTrace(TRACE_LEVEL_ERROR, INIT, L"Too many steps %d more than quota %d", stepNumber, MAP_STEP_MAX);
        return dwRetVal;
    }

    Profile profile{ 0 };
    profile.config_base_index.light_type = lpProfileData->lightType;
    profile.config_base_index.light_rgb  = lpProfileData->lightRGB[0] << 16; // red
    profile.config_base_index.light_rgb |= lpProfileData->lightRGB[1] << 8;  // green
    profile.config_base_index.light_rgb |= lpProfileData->lightRGB[2];       // blue
    profile.config_base_index.light_intensity = lpProfileData->lightIntensity;
    profile.config_base_index1.motor_intensity = lpProfileData->motorIntensity;
    profile.config_base_index1.controller_mode = lpProfileData->controllerMode;
    profile.config_base_index1.ns_position = lpProfileData->nsPosition;
    profile.config_turbo.A_turbo = lpProfileData->turboA;
    profile.config_turbo.B_turbo = lpProfileData->turboB;
    profile.config_turbo.X_turbo = lpProfileData->turboX;
    profile.config_turbo.Y_turbo = lpProfileData->turboY;
    profile.config_turbo.L1_turbo = lpProfileData->turboL1;
    profile.config_turbo.R1_turbo = lpProfileData->turboR1;
    profile.config_turbo.turbo_open = lpProfileData->turboOpen;
    profile.config_stick_trigger.Lstick_deadzone_value = lpProfileData->lStickDeadZoneValue;
    profile.config_stick_trigger.Rstick_deadzone_value = lpProfileData->rStickDeadZoneValue;
    profile.config_stick_trigger.left_stick_sensitivity = lpProfileData->lStickSensitivity;
    profile.config_stick_trigger.right_stick_sensitivity = lpProfileData->rStickSensitivity;
    profile.config_stick_trigger.left_trigger_sensitivity = lpProfileData->lTriggerSensitivity;
    profile.config_stick_trigger.right_trigger_sensitivity = lpProfileData->rTriggerSensitivity;
    profile.config_stick_trigger.enter_stick_cal = lpProfileData->stickCalibration;
    profile.config_stick_trigger.stick_cal_sucess = lpProfileData->stickCalibrationSucess;
    profile.config_stick_trigger.enter_trigger_cal = lpProfileData->triggerCalibration;
    profile.config_stick_trigger.trigger_cal_sucess = lpProfileData->triggerCalibrationSucess;
    for (int i = 0; i < MAP_KEY_MAX; i++)
    {
        profile.mk_mode_keydata[i].mapping = lpProfileData->keys[i];
    }

    for (int i = 0, stepIndex = 0; i < BACK_KEY_NUMBER; i++)
    {
        profile.bk_macro[i].cycleflag = lpProfileData->bkMacro[i].cycleFlag;
        profile.bk_macro[i].cycleIntervaltime = lpProfileData->bkMacro[i].cycleInterval;
        profile.bk_macro[i].start_step = stepIndex;

        for (auto step = &lpProfileData->bkMacro[i].step; step != nullptr; step = step->next)
        {
            auto s = &profile.bk_macro_step[stepIndex];
            s->mapping = step->key;
            s->mappingtime = step->time;
            s->intervaltime_with_next_step = step->interval;
            s->map_controller_or_keyboard = step->type;
            s->next_step = step->next == nullptr ? 0 : stepIndex + 1;

            profile.bk_macro[i].end_step = stepIndex;
            ++stepIndex;
        }
    }

    if (auto profileDevice{ reinterpret_cast<ProfileDevice*>(hProfileDevice) };
        profileDevice->WriteProfileData(0, profile))
    {
        WPPTrace(TRACE_LEVEL_INFORMATION, PROFILE, L"Succeeded in writing profile data");
    }
    else
    {
        dwRetVal = ERROR_WRITE_FAULT;
        WPPTrace(TRACE_LEVEL_ERROR, INIT, L"Write profile data failed");
    }

    return dwRetVal;
}

YZGAMINGCOM_API
VOID
YZGamingCom_Uninitialize(
    VOID
    )
{
    if (bool{ profileOp })
    {
        profileOp->Uninitialize();
        profileOp.reset(nullptr);

        WPPTrace(TRACE_LEVEL_INFORMATION, INIT, L"The ProfileOperation object is uninitialized");
    }
}
