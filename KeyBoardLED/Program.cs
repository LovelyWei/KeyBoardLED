using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;

namespace KeyBoardLED
{
    class Program
    {
        static bool[] KeyStaut = new bool[3];
        [DllImport("user32.dll")]
        static extern void keybd_event(byte bVk, byte bScan, uint dwFlags,uint dwExtraInfo);
        [DllImport("user32.dll")]
        static extern short GetKeyState(int nVirtKey);

        const uint KEYEVENTF_EXTENDEDKEY = 0x1;
        const uint KEYEVENTF_KEYUP = 0x2;


        public enum VirtualKeys : byte
        {
            VK_NUMLOCK = 0x90, //数字锁定键
            VK_SCROLL = 0x91, //滚动锁定
            VK_CAPITAL = 0x14, //大小写锁定
            VK_A = 62
        }

        public static bool GetState(VirtualKeys Key)
        {
            return (GetKeyState((int)Key) == 1);
        }

        public static void SetState(VirtualKeys Key, bool State)
        {
            if (State != GetState(Key))
            {
                keybd_event((byte)Key, 0x45, KEYEVENTF_EXTENDEDKEY | 0, 0);
                keybd_event((byte)Key, 0x45, KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYUP, 0);
            }
        }

        [DllImport("Kernel32")]
        public static extern bool SetConsoleCtrlHandler(HandlerRoutine Handler, bool Add);

        public delegate bool HandlerRoutine(CtrlTypes CtrlType);

        public enum CtrlTypes
        {
            CTRL_C_EVENT = 0,
            CTRL_BREAK_EVENT,
            CTRL_CLOSE_EVENT,
            CTRL_LOGOFF_EVENT = 5,
            CTRL_SHUTDOWN_EVENT
        }

        static void Main(string[] args)
        {
            KeyStaut[0] = GetState(VirtualKeys.VK_NUMLOCK);
            KeyStaut[1] = GetState(VirtualKeys.VK_CAPITAL);
            KeyStaut[2] = GetState(VirtualKeys.VK_SCROLL);
            SetConsoleCtrlHandler(new HandlerRoutine(ConsoleCtrlCheck), true);
            Thread thread = new Thread(new ThreadStart(Start));
            thread.Start();
        }

        private static bool ConsoleCtrlCheck(CtrlTypes ctrlType)
        {
            SetState(VirtualKeys.VK_NUMLOCK, KeyStaut[0]);
            SetState(VirtualKeys.VK_CAPITAL, KeyStaut[1]);
            SetState(VirtualKeys.VK_SCROLL, KeyStaut[2]);
            return true;
        }

        private static void Start()
        {
            while (true)
            {
                SetState(VirtualKeys.VK_NUMLOCK, true);
                Thread.Sleep(500);
                Thread.Sleep(0);
                SetState(VirtualKeys.VK_CAPITAL, true);
                Thread.Sleep(500);
                Thread.Sleep(0);
                SetState(VirtualKeys.VK_SCROLL, true);
                Thread.Sleep(1000);
                Thread.Sleep(0);
                SetState(VirtualKeys.VK_NUMLOCK, false);
                Thread.Sleep(500);
                Thread.Sleep(0);
                SetState(VirtualKeys.VK_CAPITAL, false);
                Thread.Sleep(500);
                Thread.Sleep(0);
                SetState(VirtualKeys.VK_SCROLL, false);
                Thread.Sleep(1000);
                Thread.Sleep(0);
            }
        }
    }
}
