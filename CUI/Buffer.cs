using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CUI
{
    public class Buffer
    {
        private char[,] characters;
        private ConsoleColor[,] backgroundColors;
        private ConsoleColor[,] foregroundColors;
        private int width;
        private int height;

        public Buffer(int width, int height)
        {
            this.width = width;
            this.height = height;
            characters = new char[width, height];
            backgroundColors = new ConsoleColor[width, height];
            foregroundColors = new ConsoleColor[width, height];
        }

        public int Width
        {
            get
            {
                return width;
            }
        }

        public int Height
        {
            get
            {
                return height;
            }
        }

        public void SetCharacter(int x, int y, char c, ConsoleColor fg, ConsoleColor bg)
        {
            if (x < 0 || x >= width || y < 0 || y >= height)
                return;

            characters[x, y] = c;
            backgroundColors[x, y] = bg;
            foregroundColors[x, y] = fg;
        }

        public void GetCharacter(int x, int y, out char c, out ConsoleColor fg, out ConsoleColor bg)
        {
            if (x < 0 || x >= width || y < 0 || y >= height)
            {
                throw new Exception("Coordinates are out of bounds.");
            }

            c = characters[x, y];
            bg = backgroundColors[x, y];
            fg = foregroundColors[x, y];
        }

        public void DrawRectangle(int px, int py, int w, int h, ConsoleColor fg, ConsoleColor bg)
        {
            if (w < 2 || h < 2)
                throw new Exception("Rectangle too small.");

            for (int i = 0; i < w - 1; i++)
            {
                //SetCharacter(px + i, py, '-', fg, bg);
                //SetCharacter(px + i, py + h - 1, '-', fg, bg);
                SetCharacter(px + i, py, '\u2500', fg, bg);
                SetCharacter(px + i, py + h - 1, '\u2500', fg, bg);
            }

            for (int i = 0; i < h - 1; i++)
            {
                SetCharacter(px, py + i, '\u2502', fg, bg);
                SetCharacter(px + w - 1, py + i, '\u2502', fg, bg);
            }

            for (int y = 1; y < h - 1; y++)
            {
                for (int x = 1; x < w - 1; x++)
                {
                    SetCharacter(px + x, py + y, ' ', fg, bg);
                }
            }

            //SetCharacter(px, py, '+', fg, bg);
            //SetCharacter(px + w - 1, py, '+', fg, bg);
            //SetCharacter(px, py + h - 1, '+', fg, bg);
            //SetCharacter(px + w - 1, py + h - 1, '+', fg, bg);

            SetCharacter(px, py, '\u250c', fg, bg);
            SetCharacter(px + w - 1, py, '\u2510', fg, bg);
            SetCharacter(px, py + h - 1, '\u2514', fg, bg);
            SetCharacter(px + w - 1, py + h - 1, '\u2518', fg, bg);
        }

        public void DrawTitledRectangle(string str, int px, int py, int w, int h, ConsoleColor textFg, ConsoleColor rectFg, ConsoleColor rectBg)
        {
            DrawRectangle(px, py, w, h, rectFg, rectBg);
            DrawText(str, px + 2, py, textFg, rectBg);
        }

        public void DrawText(string str, int px, int py, ConsoleColor fg, ConsoleColor bg)
        {
            for (int i = 0; i < str.Length; i++)
            {
                SetCharacter(px + i, py, str[i], fg, bg);
            }
        }

        public void Clear(ConsoleColor bg)
        {
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    SetCharacter(x, y, ' ', bg, bg);
                }
            }
        }

        public void DumpCharacterBuffer()
        {
            StringBuilder sb = new StringBuilder();
            char c;
            ConsoleColor fg;
            ConsoleColor bg;

            for (int y = 0; y < height; y++)
            {
                sb.Append("{ ");

                for (int x = 0; x < width; x++)
                {
                    sb.Append("'");
                    GetCharacter(x, y, out c, out fg, out bg);
                    sb.Append(c);
                    sb.Append("'");

                    if (x < width - 1)
                    {
                        sb.Append(", ");
                    }
                }

                sb.AppendLine(" }");
            }

            System.Diagnostics.Debug.Write(sb.ToString());
        }

        public void DumpColorBuffer()
        {
            StringBuilder sb = new StringBuilder();
            char c;
            ConsoleColor fg;
            ConsoleColor bg;

            for (int y = 0; y < height; y++)
            {
                sb.Append("{ ");

                for (int x = 0; x < width; x++)
                {
                    sb.Append(" ");
                    GetCharacter(x, y, out c, out fg, out bg);
                    sb.Append(fg.ToString()[0]);
                    sb.Append(bg.ToString()[0]);

                    if (x < width - 1)
                    {
                        sb.Append(", ");
                    }
                }

                sb.AppendLine(" }");
            }

            System.Diagnostics.Debug.Write(sb.ToString());
        }
    }
}
