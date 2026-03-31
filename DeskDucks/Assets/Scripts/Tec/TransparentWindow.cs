using System;
using System.Runtime.InteropServices;
using UnityEngine;

public class TransparentWindow : MonoBehaviour
{
    [DllImport("user32.dll")]
    private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

    [DllImport("user32.dll")]
    private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

    [DllImport("user32.dll")]
    private static extern int SetWindowLong(IntPtr hWnd, int nIndex, uint dwNewLong);

    [DllImport("dwmapi.dll")]
    private static extern int DwmExtendFrameIntoClientArea(IntPtr hWnd, ref MARGINS pMargins);

    [StructLayout(LayoutKind.Sequential)]
    public struct MARGINS
    {
        public int cxLeftWidth;
        public int cxRightWidth;
        public int cyTopHeight;
        public int cyBottomHeight;
    }

    const int GWL_EXSTYLE = -20;
    const uint WS_EX_LAYERED = 0x00080000;
    const uint WS_EX_TRANSPARENT = 0x00000020;

    private IntPtr hwnd;
    private uint originalStyle;

    void Start()
    {
        hwnd = FindWindow(null, Application.productName);

        // ? GET CURRENT STYLE (IMPORTANT)
        originalStyle = (uint)GetWindowLong(hwnd, GWL_EXSTYLE);

        // Ensure layered is enabled but KEEP everything else
        SetWindowLong(hwnd, GWL_EXSTYLE, originalStyle | WS_EX_LAYERED);

        // Extend frame (transparency)
        MARGINS margins = new MARGINS { cxLeftWidth = -1 };
        DwmExtendFrameIntoClientArea(hwnd, ref margins);
    }

    public void SetClickThrough(bool value)
    {
        if (value)
        {
            SetWindowLong(hwnd, GWL_EXSTYLE, originalStyle | WS_EX_LAYERED | WS_EX_TRANSPARENT);
        }
        else
        {
            SetWindowLong(hwnd, GWL_EXSTYLE, originalStyle | WS_EX_LAYERED);
        }
    }
}