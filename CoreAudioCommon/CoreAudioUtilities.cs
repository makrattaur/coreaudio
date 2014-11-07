using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CoreAudioApi;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace CoreAudioCommon
{
    public static class StringExtensions
    {
        public static string Repeat(this string str, int count)
        {
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < count; i++)
                sb.Append(str);

            return sb.ToString();
        }
    }

    public class CoreAudioUtilities
    {
        internal static class NativeMethods
        {
            [DllImport("kernel32.dll", SetLastError = true, CallingConvention = CallingConvention.Winapi)]
            [return: MarshalAs(UnmanagedType.Bool)]
            internal static extern bool IsWow64Process([In] IntPtr process, [Out] out bool wow64Process);
        }

        public static string GetSndVolDisplayName(AudioSession sessCtl)
        {
            string name = sessCtl.GetDisplayName();

            if (sessCtl.IsSystemSession)
            {
                return "System Sounds";
            }
            else if (name != "")
            {
                return name;
            }
            else
            {
                int pid = sessCtl.ProcessId;
                Process p = Process.GetProcesses().Where(proc => proc.Id == pid).SingleOrDefault();

                if (p == null)
                    return "<dead process>";

                if (p.HasExited)
                    return "<defunct process>";

                bool is32BitProcess;
                NativeMethods.IsWow64Process(p.Handle, out is32BitProcess);

                if (is32BitProcess || Environment.Is64BitProcess)
                {
                    try
                    {
                        if (p.MainWindowHandle != IntPtr.Zero)
                            return p.MainWindowTitle;
                        else
                            return p.MainModule.FileVersionInfo.FileDescription;
                    }
                    catch
                    {
                        return "*Name not available*";
                    }
                }
                else
                {
                    return "<64-bit process from 32-bit process>";
                }
            }
        }
    }
}
