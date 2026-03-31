using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class GlobalMouseHook : MonoBehaviour
{
    private delegate IntPtr LowLevelMouseProc(int nCode, IntPtr wParam, IntPtr lParam);

    [DllImport("user32.dll")]
    private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelMouseProc lpfn, IntPtr hMod, uint dwThreadId);

    [DllImport("user32.dll")]
    private static extern bool UnhookWindowsHookEx(IntPtr hhk);

    [DllImport("user32.dll")]
    private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

    [DllImport("kernel32.dll")]
    private static extern IntPtr GetModuleHandle(string lpModuleName);

    private const int WH_MOUSE_LL = 14;
    private const int WM_LBUTTONDOWN = 0x0201;
    private const int WM_LBUTTONUP = 0x0202;

    public static event Action<Vector2> OnMouseDown;
    public static event Action<Vector2> OnMouseUp;

    private IntPtr hookId = IntPtr.Zero;
    private LowLevelMouseProc proc;

    private readonly Queue<MouseEventData> eventQueue = new Queue<MouseEventData>();

    private enum MouseEventType
    {
        Down,
        Up
    }

    private struct MouseEventData
    {
        public MouseEventType type;
        public Vector2 position;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct POINT
    {
        public int x;
        public int y;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct MSLLHOOKSTRUCT
    {
        public POINT pt;
        public uint mouseData;
        public uint flags;
        public uint time;
        public IntPtr dwExtraInfo;
    }

    void OnEnable()
    {
        proc = HookCallback;
        hookId = SetWindowsHookEx(WH_MOUSE_LL, proc, GetModuleHandle(null), 0);
    }

    void OnDisable()
    {
        if (hookId != IntPtr.Zero)
        {
            UnhookWindowsHookEx(hookId);
            hookId = IntPtr.Zero;
        }
    }

    private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
    {
        if (nCode >= 0)
        {
            MSLLHOOKSTRUCT info = Marshal.PtrToStructure<MSLLHOOKSTRUCT>(lParam);
            Vector2 pos = new Vector2(info.pt.x, info.pt.y);

            lock (eventQueue)
            {
                if (wParam == (IntPtr)WM_LBUTTONDOWN)
                {
                    eventQueue.Enqueue(new MouseEventData
                    {
                        type = MouseEventType.Down,
                        position = pos
                    });
                }
                else if (wParam == (IntPtr)WM_LBUTTONUP)
                {
                    eventQueue.Enqueue(new MouseEventData
                    {
                        type = MouseEventType.Up,
                        position = pos
                    });
                }
            }
        }

        return CallNextHookEx(hookId, nCode, wParam, lParam);
    }

    void Update()
    {
        lock (eventQueue)
        {
            while (eventQueue.Count > 0)
            {
                MouseEventData e = eventQueue.Dequeue();

                if (e.type == MouseEventType.Down)
                    OnMouseDown?.Invoke(e.position);
                else
                    OnMouseUp?.Invoke(e.position);
            }
        }
    }
}