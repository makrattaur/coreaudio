using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CoreAudioApi;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Runtime.InteropServices;
using CUI;
using CoreAudioCommon;

namespace ConsoleVolumeMixer
{
    class Program
    {
        static void Test04(string[] args)
        {
            CUI.Buffer b = new CUI.Buffer(Console.WindowWidth - 1, Console.WindowHeight - 1);

            using (var css = new ConsoleStateSaver())
            {
                Console.CursorVisible = false;
                ConsoleScreen cs = new ConsoleScreen();

                bool exit = false;
                Console.CancelKeyPress += (s, e) => { e.Cancel = true; exit = true; };

                cs.InitClear(ConsoleColor.Blue);

                while (!exit)
                {

                    b.Clear(ConsoleColor.Blue);
                    // ¦  99 100 ¦

                    b.SetCharacter(0, 0, '\u2502', ConsoleColor.Cyan, ConsoleColor.Blue);
                    b.DrawText("         ", 1, 0, ConsoleColor.White, ConsoleColor.Blue);
                    b.SetCharacter(11, 0, '\u2502', ConsoleColor.Cyan, ConsoleColor.Blue);

                    b.SetCharacter(0, 1, '\u2502', ConsoleColor.Cyan, ConsoleColor.Blue);
                    b.DrawText("  99 100 ", 1, 1, ConsoleColor.White, ConsoleColor.Blue);
                    b.SetCharacter(11, 1, '\u2502', ConsoleColor.Cyan, ConsoleColor.Blue);

                    cs.PresentScreenBuffer(b);
                    System.Threading.Thread.Sleep(50);
                }

                cs.EndClear(css);
            }
        }


        static void Main(string[] args)
        {
            //Test03(args);
            ConsoleVolumeMixer app = new ConsoleVolumeMixer();
            app.Run();
        }
    }
}
