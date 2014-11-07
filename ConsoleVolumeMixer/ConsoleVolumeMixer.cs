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
    public class ConsoleVolumeMixer
    {
        enum CurrentFocus
        {
            SessionList,
            Device,
            DeviceSelection
        }

        CUI.Buffer screenBuffer;

        MMDeviceEnumerator enumerator;
        MMDevice device;

        AudioSessionManager sessMgr;
        List<AudioSession> sessions;
        ConcurrentQueue<AudioSession> sessionsPendingRemoval;

        int sessionIndex = 0;
        int sessionWindow = 0;
        int maxSessions;

        CurrentFocus focusedElement = CurrentFocus.SessionList;

        class Point
        {
            public Point(int x, int y)
            {
                X = x;
                Y = y;
            }

            public int X { get; set; }
            public int Y { get; set; }
        }

        class DrawMeterData
        {
            public enum DrawStateEnum
            { 
                Normal, Focused, UnfocusedActive
            }

            public DrawMeterData()
            {

            }

            public DrawMeterData(DrawMeterData other)
            {
                this.Title = other.Title;
                this.Volume = other.Volume;
                this.Level = other.Level;
                this.Mute = other.Mute;
                this.DrawState = other.DrawState;
                this.Position = other.Position;
                this.Size = other.Size;
            }

            public string Title { get; set; }
            public float Volume { get; set; }
            public float Level { get; set; }
            public bool Mute { get; set; }
            public DrawStateEnum DrawState { get; set; }
            public Point Position { get; set; }
            public Point Size { get; set; }

            public static DrawMeterData FromControllable<T>(T obj) where T : ILevelMonitorable, IVolumeAdjustable
            {
                return new DrawMeterData()
                {
                    Volume = obj.GetMasterVolume(),
                    Level = obj.GetMasterLevel(),
                    Mute = obj.GetMute()
                };
            }
        }

        void DrawMeter(CUI.Buffer b, DrawMeterData dmd)
        {
            int px = dmd.Position.X;
            int py = dmd.Position.Y;
            int w = dmd.Size.X;
            int h = dmd.Size.Y;

            ConsoleColor rectColor;
            switch (dmd.DrawState)
            {
                case DrawMeterData.DrawStateEnum.Normal:
                {
                    rectColor = ConsoleColor.Cyan;
                    break;
                }
                case DrawMeterData.DrawStateEnum.Focused:
                {
                    rectColor = ConsoleColor.Red;
                    break;
                }
                case DrawMeterData.DrawStateEnum.UnfocusedActive:
                {
                    rectColor = ConsoleColor.DarkRed;
                    break;
                }
                default:
                {
                    rectColor = ConsoleColor.Cyan;
                    break;
                }
            }
            b.DrawTitledRectangle(dmd.Title, px, py, w, h, ConsoleColor.White, rectColor, ConsoleColor.Blue);

            b.DrawText("Lvl", px + 2, py + 2, ConsoleColor.White, ConsoleColor.Blue);
            b.DrawText("Vol", px + 6, py + 2, ConsoleColor.White, ConsoleColor.Blue);

            b.DrawRectangle(px + 2, py + 4, 3, h - 10, ConsoleColor.Cyan, ConsoleColor.Blue);
            //for (int i = 0; i < h - 12; i++)
            //{
            //    b.SetCharacter(px + 3, py + 5 + i, '\u2588', ConsoleColor.Green, ConsoleColor.Blue);
            //}
            int lvlCharCount = (int)Math.Round(dmd.Level * (h - 12));
            for (int i = 0; i < lvlCharCount; i++)
            {
                b.SetCharacter(px + 3, py + h - 8 - i, '\u2588', ConsoleColor.Green, ConsoleColor.Blue);
            }

            b.DrawRectangle(px + 6, py + 4, 3, h - 10, ConsoleColor.Cyan, ConsoleColor.Blue);
            int volCharCount = (int)Math.Round(dmd.Volume * (h - 12));
            //for (int i = 0; i < h - 12; i++)
            //{
            //    b.SetCharacter(px + 7, py + 5 + i, '\u2588', ConsoleColor.Gray, ConsoleColor.Blue);
            //}
            for (int i = 0; i < volCharCount; i++)
            {
                b.SetCharacter(px + 7, py + h - 8 - i, '\u2588', ConsoleColor.Gray, ConsoleColor.Blue);
            }
#if false
            b.SetCharacter(px + 6, py + h - 8 - volCharCount, '\u253c', ConsoleColor.Cyan, ConsoleColor.Blue);
            b.SetCharacter(px + 7, py + h - 8 - volCharCount, '\u2500', ConsoleColor.Cyan, ConsoleColor.Blue);
            b.SetCharacter(px + 8, py + h - 8 - volCharCount, '\u2524', ConsoleColor.Cyan, ConsoleColor.Blue);

            b.SetCharacter(px + 2, py + h - 8 - volCharCount, '\u251c', ConsoleColor.Cyan, ConsoleColor.Blue);
            b.SetCharacter(px + 3, py + h - 8 - volCharCount, '\u2500', ConsoleColor.Cyan, ConsoleColor.Blue);
            b.SetCharacter(px + 4, py + h - 8 - volCharCount, '\u253c', ConsoleColor.Cyan, ConsoleColor.Blue);

            b.SetCharacter(px + 5, py + h - 8 - volCharCount, '\u2500', ConsoleColor.Cyan, ConsoleColor.Blue);
#endif
            b.DrawText((dmd.Level * 100).ToString("##0").PadLeft(3), px + 2, py + h - 5, ConsoleColor.White, ConsoleColor.Blue);
            b.DrawText((dmd.Volume * 100).ToString("##0").PadLeft(3), px + 6, py + h - 5, ConsoleColor.White, ConsoleColor.Blue);

            b.DrawText("[", px + 3, py + h - 3, ConsoleColor.White, ConsoleColor.Blue);
            if (dmd.Mute)
                b.DrawText("M", px + 5, py + h - 3, ConsoleColor.Red, ConsoleColor.Blue);
            b.DrawText("]", px + 7, py + h - 3, ConsoleColor.White, ConsoleColor.Blue);
        }


        void RegisterRemovalThreadSafe(AudioSession sess)
        {
            sess.OnSessionDisconnection += (reason) =>
            {
                //Console.WriteLine("Session with PID " + sess.ProcessId + " disconnected.");
                sessionsPendingRemoval.Enqueue(sess);
            };
            sess.OnStateChanged += (state) =>
            {
                if (state == AudioSessionState.Expired)
                {
                    //Console.WriteLine("Session with PID " + sess.ProcessId + " expired.");
                    sessionsPendingRemoval.Enqueue(sess);
                }
            };

            int pid = sess.ProcessId;
            Process p = Process.GetProcesses().Where(proc => proc.Id == pid).SingleOrDefault();
            if (p != null)
            {
                bool hasExited;

                try
                {
                    hasExited = p.HasExited;
                }
                catch (System.ComponentModel.Win32Exception)
                {
                    //Console.WriteLine("Could not attach exit handler for pid " + p.Id + ", no perm.");
                    return;
                }

                if (!hasExited)
                {
                    p.EnableRaisingEvents = true;
                    p.Exited += (s, e) =>
                    {
                        //Console.WriteLine("Process with PID " + sess.ProcessId + " exited.");
                        sessionsPendingRemoval.Enqueue(sess);
                    };
                }
            }
            else
            {
                //Console.WriteLine("Could not attach exit handler for pid " + pid + ", cannot find pid.");
            }
        }

        void RemovePendingSessions()
        {
            AudioSession sessionToRemove;
            while (sessionsPendingRemoval.TryDequeue(out sessionToRemove))
            {
                sessions.Remove(sessionToRemove);
                sessionToRemove.Dispose();
            }
        }

        void Init()
        {
            screenBuffer = new CUI.Buffer(Console.WindowWidth - 1, Console.WindowHeight);

            enumerator = new MMDeviceEnumerator();
            device = enumerator.GetDefaultAudioEndpoint(EDataFlow.eRender, ERole.eConsole);

            sessMgr = device.SessionManager;
            sessions = new List<AudioSession>(sessMgr.GetSessions());
            sessionsPendingRemoval = new ConcurrentQueue<AudioSession>();
            for (int i = 0; i < sessions.Count; i++)
            {
                AudioSession sess = sessions[i];
                RegisterRemovalThreadSafe(sess);
            }
            sessMgr.OnSessionCreated += (sess) =>
            {
                sessions.Add(sess);
                RegisterRemovalThreadSafe(sess);
            };

            maxSessions = screenBuffer.Width / 11 - 1;
        }

        void UpdateSessionScroll()
        {
            if (sessionIndex < 0)
                sessionIndex = 0;

            if (sessionIndex >= sessions.Count)
                sessionIndex = sessions.Count - 1;

            if (sessionIndex - sessionWindow >= maxSessions)
            {
                sessionWindow++;
            }

            if (sessionIndex - sessionWindow < 0)
            {
                sessionWindow--;
            }
        }

        void SwitchToNextElement()
        {
            switch (focusedElement)
            {
                case CurrentFocus.SessionList:
                {
                    focusedElement = CurrentFocus.Device;
                    break;
                }
                case CurrentFocus.Device:
                {
                    focusedElement = CurrentFocus.DeviceSelection;
                    break;
                }
                case CurrentFocus.DeviceSelection:
                {
                    focusedElement = CurrentFocus.SessionList;
                    break;
                }
                default:
                {
                    break;
                }
            }
        }

        public static T Clamp<T>(T val, T min, T max) where T : IComparable<T>
        {
            if (val.CompareTo(min) < 0) return min;
            else if (val.CompareTo(max) > 0) return max;
            else return val;
        }

        void CheckElementChange(ConsoleKeyInfo cki, IVolumeAdjustable iva)
        {
            if (cki.Key == ConsoleKey.Spacebar)
            {
                iva.SetMute(!iva.GetMute());
            }
            else if (cki.Key == ConsoleKey.DownArrow)
            {
                float currentVolume = iva.GetMasterVolume();
                iva.SetMasterVolume(Clamp(currentVolume - 0.01f, 0.0f, 1.0f));
            }
            else if (cki.Key == ConsoleKey.UpArrow)
            {
                float currentVolume = iva.GetMasterVolume();
                iva.SetMasterVolume(Clamp(currentVolume + 0.01f, 0.0f, 1.0f));
            }
        }

        void OnKeyPress(ConsoleKeyInfo cki)
        {
            switch(focusedElement)
            {
                case CurrentFocus.SessionList:
                {
                    if (cki.Key == ConsoleKey.LeftArrow)
                    {
                        sessionIndex--;
                    }
                    else if (cki.Key == ConsoleKey.RightArrow)
                    {
                        sessionIndex++;
                    }

                    UpdateSessionScroll();

                    CheckElementChange(cki, sessions[sessionIndex]);

                    break;
                }
                case CurrentFocus.Device:
                {
                    CheckElementChange(cki, device);

                    break;
                }
                default:
                {
                    break;
                }
            }

            if (cki.Key == ConsoleKey.Tab)
            {
                SwitchToNextElement();
            }
        }

        void Update()
        {
            if (sessionsPendingRemoval.Count > 0)
            {
                RemovePendingSessions();
            }

            if (Console.KeyAvailable)
            {
                ConsoleKeyInfo cki = Console.ReadKey(true);

                OnKeyPress(cki);
            }

            switch (focusedElement)
            {
                case CurrentFocus.SessionList:
                {
                    UpdateSessionScroll();

                    break;
                }
                default:
                {
                    break;
                }
            }
        }

        void Draw()
        {
            screenBuffer.Clear(ConsoleColor.Blue);
            screenBuffer.DrawText("Current device: ", 0, 1, ConsoleColor.White, ConsoleColor.Blue);
            screenBuffer.DrawRectangle(16, 0, screenBuffer.Width - 17, 3, focusedElement == CurrentFocus.DeviceSelection ? ConsoleColor.Red : ConsoleColor.White, ConsoleColor.Blue);
            screenBuffer.DrawText(device.Name, 18, 1, ConsoleColor.White, ConsoleColor.Blue);

            DrawMeter(screenBuffer, new DrawMeterData(DrawMeterData.FromControllable(device))
            {
                Title = "Master",
                DrawState = focusedElement == CurrentFocus.Device ? DrawMeterData.DrawStateEnum.Focused : DrawMeterData.DrawStateEnum.Normal,
                Position = new Point(0, 3),
                Size = new Point(11, screenBuffer.Height - 3)
            });

            for (int i = sessionWindow; i < Math.Min(sessions.Count, sessionWindow + maxSessions); i++)
            {
                var session = sessions[i];
                DrawMeterData.DrawStateEnum drawState;
                if (focusedElement == CurrentFocus.SessionList)
                {
                    drawState = sessionIndex == i ? DrawMeterData.DrawStateEnum.Focused : DrawMeterData.DrawStateEnum.Normal;
                }
                else
                {
                    drawState = sessionIndex == i ? DrawMeterData.DrawStateEnum.UnfocusedActive : DrawMeterData.DrawStateEnum.Normal;
                }

                DrawMeter(screenBuffer, new DrawMeterData(DrawMeterData.FromControllable(session))
                {
                    Title = "Session",
                    DrawState = drawState,
                    Position = new Point(11 * (1 + (i - sessionWindow)), 3),
                    Size = new Point(11, screenBuffer.Height - 5)
                });
            }
            screenBuffer.DrawText("Current session: " + CoreAudioUtilities.GetSndVolDisplayName(sessions[sessionIndex]), 11, screenBuffer.Height - 1, ConsoleColor.White, ConsoleColor.Blue);

            screenBuffer.SetCharacter(11, screenBuffer.Height - 2, '\u2190', ConsoleColor.White, ConsoleColor.Blue);
            screenBuffer.SetCharacter(11 + maxSessions * 11 - 1, screenBuffer.Height - 2, '\u2192', ConsoleColor.White, ConsoleColor.Blue);
            int dist = maxSessions * 11 - 2;
            int chunkSize = Math.Max((int)Math.Round(dist / (double)sessions.Count), 1);
            for (int i = 0; i < dist; i++)
            {
                screenBuffer.SetCharacter(12 + i, screenBuffer.Height - 2, '\u2592', ConsoleColor.White, ConsoleColor.Blue);
            }

            int drawChunkSize = chunkSize;
            if ((sessionIndex + 1) == sessions.Count)
            {
                drawChunkSize = dist - sessionIndex * chunkSize;
            }
            for (int i = 0; i < drawChunkSize; i++)
            {
                screenBuffer.SetCharacter(12 + sessionIndex * chunkSize + i, screenBuffer.Height - 2, '\u2588', ConsoleColor.White, ConsoleColor.Blue);
            }
        }

        void MainLoop()
        {
            using (var css = new ConsoleStateSaver())
            {
                Console.CursorVisible = false;
                ConsoleScreen cs = new ConsoleScreen();

                bool exit = false;
                Console.CancelKeyPress += (s, e) => { e.Cancel = true; exit = true; };

                cs.InitClear(ConsoleColor.Blue);

                while (!exit)
                {
                    Update();
                    Draw();
                    cs.PresentScreenBuffer(screenBuffer);
                    System.Threading.Thread.Sleep(50);
                }

                cs.EndClear(css);
            }
        }

        public void Run()
        {
            Init();
            MainLoop();
        }
    }
}
