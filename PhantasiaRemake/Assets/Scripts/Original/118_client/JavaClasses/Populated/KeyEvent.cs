//import java.awt.JavaPanel;
//import java.awt.JavaButton;
//import java.awt.TextField;
//import java.awt.TextArea;
//import java.awt.List;
//import java.awt.BorderLayout;
//import java.io.DataInputStream;
//import java.awt.Color;
//import java.awt.event.*;
using System;
using UnityEngine;

public class KeyEvent
{
    public const int VK_F1 = 1;
    public const int VK_F2 = 2;
    public const int VK_F3 = 3;
    public const int VK_F4 = 4;
    public const int VK_F5 = 5;
    public const int VK_F6 = 6;
    public const int VK_F7 = 7;
    public const int VK_F8 = 8;
    public const int VK_1 = 9;
    public const int VK_2 = 10;
    public const int VK_3 = 11;
    public const int VK_4 = 12;
    public const int VK_5 = 13;
    public const int VK_6 = 14;
    public const int VK_7 = 15;
    public const int VK_8 = 16;
    public const int VK_NUMPAD1 = 17;
    public const int VK_NUMPAD2 = 18;
    public const int VK_NUMPAD3 = 19;
    public const int VK_NUMPAD4 = 20;
    public const int VK_NUMPAD5 = 21;
    public const int VK_NUMPAD6 = 22;
    public const int VK_NUMPAD7 = 23;
    public const int VK_NUMPAD8 = 24;
    public const int VK_NUMPAD9 = 25;
    public const int VK_SPACE = 26;

    public const int VK_RETURN = 27;

    int currentKeyCode;

    internal void setCurrentKeyCode(int key)
    {
        currentKeyCode = key;
    }

    internal int getKeyCode()
    {
        return currentKeyCode;
    }
}