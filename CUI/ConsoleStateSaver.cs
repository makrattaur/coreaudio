using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CUI
{
    public class ConsoleStateSaver : IDisposable
    {
        ConsoleColor oldBackgroundColor;
        ConsoleColor oldForegroundColor;
        bool oldCursorVisibility;
        int oldCursorLeft;
        int oldCursorTop;
        int oldBufferLeft;
        int oldBufferTop;

        public ConsoleColor OldBackgroundColor
        {
            get
            {
                return oldBackgroundColor;
            }
        }

        public ConsoleColor OldForegroundColor
        {
            get
            {
                return oldForegroundColor;
            }
        }

        public ConsoleStateSaver()
        {
            oldBackgroundColor = Console.BackgroundColor;
            oldForegroundColor = Console.ForegroundColor;
            oldCursorVisibility = Console.CursorVisible;

            oldCursorLeft = Console.CursorLeft;
            oldCursorTop = Console.CursorTop;

            oldBufferLeft = Console.WindowLeft;
            oldBufferTop = Console.WindowTop;
        }

        public void Dispose()
        {
            Console.BackgroundColor = oldBackgroundColor;
            Console.ForegroundColor = oldForegroundColor;
            Console.CursorVisible = oldCursorVisibility;

            Console.CursorLeft = oldCursorLeft;
            Console.CursorTop = oldCursorTop;

            Console.WindowLeft = oldBufferLeft;
            Console.WindowTop = oldBufferTop;
        }
    }
}
