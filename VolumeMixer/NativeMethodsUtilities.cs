using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Drawing;

namespace VolumeMixer
{
    public class NativeMethodsUtilities
    {
        public static Icon ExtractIcon(string file, int index, bool largeIcon)
        {
            IntPtr large;
            IntPtr small;

            int count = NativeMethods.ExtractIconEx(file, index, out large, out small, 1);
            // 2 icons extracted (small + large)

            if ((largeIcon ? large : small) == IntPtr.Zero)
            {
                return null;
            }

            Icon icon = Icon.FromHandle(largeIcon ? large : small);
            Icon copy = (Icon)icon.Clone();
            if (large != null)
            {
                NativeMethods.DestroyIcon(large);
            }

            if (small != null)
            {
                NativeMethods.DestroyIcon(small);
            }

            icon.Dispose();

            return copy;
        }

        public static void ExtractModuleAndResourceId(string str, out string modulePath, out int resId)
        {
            int atIndex = str.IndexOf('@');
            int commaIndex = str.IndexOf(',');

            modulePath = str.Substring(atIndex + 1, commaIndex - atIndex - 1).Trim();
            resId = int.Parse(str.Substring(commaIndex + 1));
        }

        public static string ExtractString(string file, int index)
        {
            IntPtr module = NativeMethods.LoadLibrary(file);
            if (module == IntPtr.Zero)
            {
                return null;
            }

            StringBuilder sb = new StringBuilder(512);
            int len = NativeMethods.LoadString(module, index, sb, sb.Capacity - 1);

            NativeMethods.FreeLibrary(module);

            return sb.ToString(0, len);
        }

        public static string GetWindowText(IntPtr window)
        {
            int len = NativeMethods.GetWindowTextLength(window);
            if (len == 0)
            {
                return null;
            }

            StringBuilder sb = new StringBuilder(len + 1);
            int ret = NativeMethods.GetWindowText(window, sb, sb.Capacity);
            if (ret == 0)
            {
                return null;
            }

            return sb.ToString();
        }
    }
}
