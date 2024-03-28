using System;
using System.Collections.Generic;
using System.Text;

namespace yz.gaming.accessoryapp.Api
{
    public enum KeyCode
    {
        //KEYBOARD DATA,键盘功能码
        KEYB_ErrorRollOver = 0x01,
        KEYB_A = 0x04,
        KEYB_B = 0x05,
        KEYB_C = 0x06,
        KEYB_D = 0x07,
        KEYB_E = 0x08,
        KEYB_F = 0x09,
        KEYB_G = 0x0A,
        KEYB_H = 0x0B,
        KEYB_I = 0x0C,
        KEYB_J = 0x0D,
        KEYB_K = 0x0E,
        KEYB_L = 0x0F,
        KEYB_M = 0x10,
        KEYB_N = 0x11,
        KEYB_O = 0x12,
        KEYB_P = 0x13,
        KEYB_Q = 0x14,
        KEYB_R = 0x15,
        KEYB_S = 0x16,
        KEYB_T = 0x17,
        KEYB_U = 0x18,
        KEYB_V = 0x19,
        KEYB_W = 0x1A,
        KEYB_X = 0x1B,
        KEYB_Y = 0x1C,
        KEYB_Z = 0x1D,

        KEYB_1 = 0x1E,
        KEYB_2 = 0x1F,
        KEYB_3 = 0x20,
        KEYB_4 = 0x21,
        KEYB_5 = 0x22,
        KEYB_6 = 0x23,
        KEYB_7 = 0x24,
        KEYB_8 = 0x25,
        KEYB_9 = 0x26,
        KEYB_0 = 0x27,

        KEYB_ENTER = 0x28,             // ENTER键   
        KEYB_ESC = 0x29,               // ESC
        KEYB_BACK = 0x2A,              // 退格键
        KEYB_TAB = 0x2B,               // Table键  
        KEYB_SPACEBAR = 0x2C,          // 空格键
        KEYB_MINUS = 0x2D,             // - and _  
        KEYB_EQUAL = 0x2E,             // + and = 
        KEYB_L_BRACKET = 0x2F,         // { and [ 
        KEYB_R_BRACKET = 0x30,         // } and ]   
        KEYB_SLASH = 0x31,             // \ and | 
        KEYB_POUND_SIGN = 0x32,        // # 
        KEYB_SEMICOLON = 0x33,         // ; and :
        KEYB_QUOTATION = 0x34,         // ' and "
        KEYB_TILDE = 0x35,             // ~ and `
        KEYB_LESS_THAN = 0x36,         // , and <
        KEYB_GREATER_THAN = 0x37,      // . and >
        KEYB_QUESTION_MARK = 0x38,     // / and ?         
        KEYB_CAPSLOCK = 0x39,          // CAPSLOCK键

        KEYB_F1 = 0x3A,
        KEYB_F2 = 0x3B,
        KEYB_F3 = 0x3C,
        KEYB_F4 = 0x3D,
        KEYB_F5 = 0x3E,
        KEYB_F6 = 0x3F,
        KEYB_F7 = 0x40,
        KEYB_F8 = 0x41,
        KEYB_F9 = 0x42,
        KEYB_F10 = 0x43,
        KEYB_F11 = 0x44,
        KEYB_F12 = 0x45,
        KEYB_PRINTSCREEN = 0x46,
        KEYB_SCROLLLOCK = 0x47,
        KEYB_PAUSE = 0x48,
        KEYB_INSERT = 0x49,
        KEYB_HOME = 0x4A,
        KEYB_PAGEUP = 0x4B,
        KEYB_DELETE = 0x4C,
        KEYB_END = 0x4D,
        KEYB_DOWN = 0x4E,
        KEYB_RIGHTARROW = 0x4F,
        KEYB_LEFTARROW = 0x50,
        KEYB_DOWNARROW = 0x51,
        KEYB_UPARROW = 0x52,

        KEYP_NUM0 = 0x62,
        KEYP_NUM1 = 0x59,
        KEYP_NUM2 = 0x5A,
        KEYP_NUM3 = 0x5B,
        KEYP_NUM4 = 0x5C,
        KEYP_NUM5 = 0x5D,
        KEYP_NUM6 = 0x5E,
        KEYP_NUM7 = 0x5F,
        KEYP_NUM8 = 0x60,
        KEYP_NUM9 = 0x61,

        KEYB_L_CTRL = 0xE0,
        KEYB_L_SHIFT = 0xE1,
        KEYB_L_ALT = 0xE2,
        KEYB_L_WIN = 0xE3,
        KEYB_R_CTRL = 0xE4,
        KEYB_R_SHIFT = 0xE5,
        KEYB_R_ALT = 0xE6,
        KEYB_R_WIN = 0xE7
    }
}
