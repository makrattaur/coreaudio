using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CoreAudioCommon;

namespace CUI
{
    public class ConsoleScreen
    {
        StringBuilder sb = new StringBuilder();

        public void PresentScreenBuffer(Buffer b)
        {
            ConsoleColor oldFg = Console.BackgroundColor;
            ConsoleColor oldBg = Console.ForegroundColor;
            char c;
            ConsoleColor fg;
            ConsoleColor bg;
            char previous;
            ConsoleColor previousFg;
            ConsoleColor previousBg;

            for (int y = 0; y < b.Height; y++)
            {
                b.GetCharacter(0, y, out previous, out previousFg, out previousBg);
                Console.BackgroundColor = oldBg = previousBg;
                Console.ForegroundColor = oldFg = previousFg;

                for (int x = 0; x < b.Width; x++)
                {
                    b.GetCharacter(x, y, out c, out fg, out bg);

                    if (c == previous && fg == previousFg && bg == previousBg)
                    {
                        sb.Append(c);
                    }
                    else
                    {
                        Console.Write(sb.ToString());

                        if (fg != oldFg)
                        {
                            Console.ForegroundColor = fg;
                            oldFg = fg;
                        }
                        if (bg != oldBg)
                        {
                            Console.BackgroundColor = bg;
                            oldBg = bg;
                        }

                        Console.Write(c);

                        previous = c;
                        previousBg = bg;
                        previousFg = fg;

                        sb.Clear();
                    }
                }
                Console.Write(sb.ToString());
                sb.Clear();
                if(y != b.Height - 1)
                    Console.WriteLine();
            }

            Console.SetCursorPosition(0, Console.CursorTop - (b.Height - 1));
        }

        public void InitClear(ConsoleColor color)
        {
            Console.BackgroundColor = color;
            Console.Write((" ".Repeat(Console.WindowWidth - 1) + "\n").Repeat(Console.WindowHeight));
            Console.SetCursorPosition(0, Console.CursorTop - Console.WindowHeight);
        }

        public void EndClear(ConsoleStateSaver css)
        {
            Console.BackgroundColor = css.OldBackgroundColor;
            Console.ForegroundColor = css.OldForegroundColor;
            Console.Write((" ".Repeat(Console.WindowWidth - 1) + "\n").Repeat(Console.WindowHeight));
            Console.SetCursorPosition(0, Console.CursorTop - Console.WindowHeight);
        }
    }
}
