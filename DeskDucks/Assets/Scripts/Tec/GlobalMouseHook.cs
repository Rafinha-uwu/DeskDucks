using System;
using System.Runtime.InteropServices;
using UnityEngine;

public class GlobalMouseHook : MonoBehaviour
{
    private delegate IntPtr LowLevelMouseProc(int nCode, IntPtr wParam, IntPtr lParam);

    [DllImport("user32.dll")]
    private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelMouseProc lpfn,
        IntPtr hMod, uint dwThreadId);

    [DllImport("user32.dll")]
    private static extern bool UnhookWindowsHookEx(IntPtr hhk);

    [DllImport("user32.dll")]
    private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode,
        IntPtr wParam, IntPtr lParam);

    [DllImport("kernel32.dll")]
    private static extern IntPtr GetModuleHandle(string lpModuleName);

    [StructLayout(LayoutKind.Sequential)]
    private struct MSLLHOOKSTRUCT
    {
        public POINT pt;
        public uint mouseData, flags, time;
        public IntPtr dwExtraInfo;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct POINT { public int x, y; }

    const int WH_MOUSE_LL = 14;
    const int WM_LBUTTONDOWN = 0x0201;

    public static event Action<Vector2> OnGlobalMouseClick;

    private IntPtr _hookId = IntPtr.Zero;
    private LowLevelMouseProc _proc;

    private Vector2 _pendingClick;
    private volatile bool _hasClick;

    void OnEnable()
    {
        _proc = HookCallback;
        _hookId = SetWindowsHookEx(WH_MOUSE_LL, _proc, GetModuleHandle(null), 0);
    }

    void OnDisable()
    {
        UnhookWindowsHookEx(_hookId);
        _hookId = IntPtr.Zero;
    }

    private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
    {
        if (nCode >= 0 && wParam == (IntPtr)WM_LBUTTONDOWN)
        {
            MSLLHOOKSTRUCT info = Marshal.PtrToStructure<MSLLHOOKSTRUCT>(lParam);
            _pendingClick = new Vector2(info.pt.x, info.pt.y);
            _hasClick = true;
        }
        return CallNextHookEx(_hookId, nCode, wParam, lParam);
    }

    void Update()
    {
        if (_hasClick)
        {
            _hasClick = false;
            OnGlobalMouseClick?.Invoke(_pendingClick);
        }
    }
}