using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Runtime.InteropServices;

namespace VolumeMixer
{
    public class NativeMethods
    {
        public const int WM_GETICON = 0x7f;
        public const int ICON_BIG = 1;
        public const int ICON_SMALL = 0;
        public const int ICON_SMALL2 = 2;

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        [DllImport("user32.dll", CharSet = CharSet.Unicode, EntryPoint = "SendMessageW")]
        public static extern IntPtr SendMessageIntPtr(IntPtr hWnd, int Msg, int wParam, int lParam);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        public static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        public static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        public const int GWL_STYLE = -16;

        public const int TBS_TOOLTIPS = 0x0100;

        [DllImport("shell32.dll", CharSet = CharSet.Unicode)]
        public static extern int ExtractIconEx(string lpszFile, int nIconIndex, out IntPtr phiconLarge, out IntPtr phiconSmall, int nIcons);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        public static extern bool DestroyIcon(IntPtr hIcon);

        [DllImport("Kernel32.dll", CharSet = CharSet.Unicode)]
        public static extern IntPtr LoadLibrary(string lpFileName);

        [DllImport("Kernel32.dll", CharSet = CharSet.Unicode)]
        public static extern bool FreeLibrary(IntPtr hModule);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        public static extern int LoadString(IntPtr hInstance, int uID, StringBuilder lpBuffer, int nBufferMax);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool IsWow64Process([In] IntPtr process, [Out] out bool wow64Process);

        public const int SMTO_ABORTIFHUNG = 0x2;
        public const int SMTO_BLOCK = 0x1;
        public const int SMTO_NORMAL = 0x0;
        public const int SMTO_NOTIMEOUTIFNOTHUNG = 0x8;
        public const int SMTO_ERRORONEXIT = 0x20;

        [DllImport("user32.dll", CharSet = CharSet.Unicode, EntryPoint = "SendMessageTimeoutW")]
        public static extern int SendMessageTimeoutIntPtr(IntPtr hWnd, int Msg, int wParam, int lParam, int fuFlags, int uTimeout, out IntPtr lpdwResult);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        public static extern int SendMessageTimeout(IntPtr hWnd, int Msg, int wParam, int lParam, int fuFlags, int uTimeout, out int lpdwResult);

        public const int GW_CHILD = 5;
        public const int GW_ENABLEDPOPUP = 6;
        public const int GW_HWNDFIRST = 0;
        public const int GW_HWNDLAST = 1;
        public const int GW_HWNDNEXT = 2;
        public const int GW_HWNDPREV = 3;
        public const int GW_OWNER = 4;

        [DllImport("user32.dll")]
        public static extern IntPtr GetWindow(IntPtr hWnd, int uCmd);

        [DllImport("user32.dll")]
        public static extern int GetWindowThreadProcessId(IntPtr hWnd, out int lpdwProcessId);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        public static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        public static extern int GetWindowTextLength(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern bool IsWindowVisible(IntPtr hWnd);

        public delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

        [DllImport("user32.dll")]
        public static extern bool EnumWindows(EnumWindowsProc lpEnumFunc, IntPtr lParam);

        public const int GCLP_MENUNAME = -8;
        public const int GCLP_HBRBACKGROUND = -10;
        public const int GCLP_HCURSOR = -12;
        public const int GCLP_HICON = -14;
        public const int GCLP_HMODULE = -16;
        public const int GCL_CBWNDEXTRA = -18;
        public const int GCL_CBCLSEXTRA = -20;
        public const int GCLP_WNDPROC = -24;
        public const int GCL_STYLE = -26;
        public const int GCLP_HICONSM = -34;
        public const int GCW_ATOM = -3;

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        public static extern int GetClassLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll", CharSet = CharSet.Unicode, EntryPoint = "GetClassLongW")]
        public static extern IntPtr GetClassLongIntPtr(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        public static extern IntPtr GetClassLongPtr(IntPtr hWnd, int nIndex);
    }
}
