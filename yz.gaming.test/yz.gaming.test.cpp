// yz.gaming.test.cpp : This file contains the 'main' function. Program execution begins and ends there.
//

#include "pch.h"
#include <iostream>
#include <vector>
#include "..\yz.gaming.com\comimpl.h"

int main()
{    
    auto f = [](ProfileDeviceEvent deviceEvent, HPROFILEDEVICE hProfileDevice, WPARAM wParam, LPARAM lParam, LPVOID pContext)
    {
        HPROFILEDEVICE* phProfileDevice = reinterpret_cast<HPROFILEDEVICE*>(pContext);

        switch (deviceEvent)
        {
        case ProfileDeviceEvent::DeviceAdded:
            {
                *phProfileDevice = hProfileDevice;

                PROFILEDATA profileData;
                ZeroMemory(&profileData, sizeof(profileData));
                YZGamingCom_ReadProfileData(hProfileDevice, &profileData);

                profileData.keys[KEYCODE::A] = KEYCODE::B;
                profileData.lightRGB[0] = 1;
                profileData.lightRGB[1] = 1;
                profileData.lStickDeadZoneValue = 20;
                profileData.rStickDeadZoneValue = 20;
                YZGamingCom_WriteProfileData(hProfileDevice, &profileData);

                std::cout << "Added " << hProfileDevice << std::endl;
            }
            break;

        case ProfileDeviceEvent::DeviceRemoved:
            std::cout << "Removed " << hProfileDevice << std::endl;
            break;

        case ProfileDeviceEvent::KeyPressed:
            switch (wParam)
            {
            case KEYCODE::DPAD_UP:
                std::cout << "DPAD_UP: ";
                break;
            case KEYCODE::DPAD_DOWN:
                std::cout << "DPAD_DOWN: ";
                break;
            case KEYCODE::DPAD_LEFT:
                std::cout << "DPAD_LEFT: ";
                break;
            case KEYCODE::DPAD_RIGHT:
                std::cout << "DPAD_RIGHT: ";
                break;
            case KEYCODE::Start:
                std::cout << "Start: ";
                break;
            case KEYCODE::Back:
                std::cout << "Back: ";
                break;
            case KEYCODE::L1:
                std::cout << "L1: ";
                break;
            case KEYCODE::R1:
                std::cout << "R1: ";
                break;
            case KEYCODE::L3:
                std::cout << "L3: ";
                break;
            case KEYCODE::R3:
                std::cout << "R3: ";
                break;
            case KEYCODE::A:
                std::cout << "A: ";
                break;
            case KEYCODE::B:
                std::cout << "B: ";
                break;
            case KEYCODE::X:
                std::cout << "X: ";
                break;
            case KEYCODE::Y:
                std::cout << "Y: ";
                break;
            case KEYCODE::Quick:
                std::cout << "Quick: ";
                break;
            }

            std::cout << (lParam ? "Down" : "Up") << std::endl;
            break;

        case ProfileDeviceEvent::TriggerPressed:
            {
                Trigger t = static_cast<Trigger>(wParam);
                BYTE v = static_cast<BYTE>(lParam);
                switch (t)
                {
                case Trigger::Left:
                    std::cout << "Left trigger: ";
                    break;
                case Trigger::Right:
                    std::cout << "Right trigger: ";
                    break;
                }

                std::cout << v << std::endl;
            }
            break;

        case ProfileDeviceEvent::ThumbPressed:
            {
                Thumb t = static_cast<Thumb>(wParam);
                INT v = static_cast<CHAR>(lParam);
                switch (t)
                {
                case Thumb::LX:
                    std::cout << "ThumbLX: ";
                    break;
                case Thumb::RX:
                    std::cout << "ThumbRX: ";
                    break;
                case Thumb::LY:
                    std::cout << "ThumbLY: ";
                    break;
                case Thumb::RY:
                    std::cout << "ThumbRY: ";
                    break;
                }

                std::cout << v << std::endl;
            }
            break;
        }
    };

    HPROFILEDEVICE profileDevice{ nullptr };
    DWORD dwRetVal = YZGamingCom_Initialize(f, &profileDevice);

    getchar();

    YZGamingCom_Uninitialize();
    return 0;
}
