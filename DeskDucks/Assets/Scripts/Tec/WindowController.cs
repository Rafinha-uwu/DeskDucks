using System;
using System.Runtime.InteropServices;
using UnityEngine;

public class WindowController : MonoBehaviour
{
    [DllImport("user32.dll")]
    private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

    [DllImport("user32.dll")]
    private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

    [DllImport("user32.dll")]
    private static extern int SetWindowLong(IntPtr hWnd, int nIndex, uint dwNewLong);

    [DllImport("user32.dll")]
    private static extern bool SetWindowPos(
        IntPtr hWnd,
        int hWndInsertAfter,
        int X,
        int Y,
        int cx,
        int cy,
        uint uFlags);

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

    private const int GWL_EXSTYLE = -20;

    private const uint WS_EX_LAYERED = 0x00080000;
    private const uint WS_EX_TRANSPARENT = 0x00000020;

    private const int HWND_TOPMOST = -1;
    private const uint SWP_NOSIZE = 0x0001;
    private const uint SWP_NOMOVE = 0x0002;

    private IntPtr hwnd;
    private uint originalStyle;
    private bool isClickThrough;

    void Start()
    {
        hwnd = FindWindow(null, Application.productName);

        if (hwnd == IntPtr.Zero)
        {
            Debug.LogError("WindowController: Failed to find game window.");
            return;
        }

        originalStyle = (uint)GetWindowLong(hwnd, GWL_EXSTYLE);

        SetWindowLong(hwnd, GWL_EXSTYLE, originalStyle | WS_EX_LAYERED);

        MARGINS margins = new MARGINS { cxLeftWidth = -1 };
        DwmExtendFrameIntoClientArea(hwnd, ref margins);

        SetWindowPos(hwnd, HWND_TOPMOST, 0, 0, 0, 0, SWP_NOSIZE | SWP_NOMOVE);
    }

    public void SetClickThrough(bool value)
    {
        if (hwnd == IntPtr.Zero || isClickThrough == value)
            return;

        isClickThrough = value;

        uint newStyle = value
            ? originalStyle | WS_EX_LAYERED | WS_EX_TRANSPARENT
            : originalStyle | WS_EX_LAYERED;

        SetWindowLong(hwnd, GWL_EXSTYLE, newStyle);
    }
}