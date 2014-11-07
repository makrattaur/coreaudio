using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CoreAudioApi;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Collections.Concurrent;
using System.Threading;

using System.Runtime.InteropServices;

namespace VolumeMixer
{
    public class ManagedSession
    {
        public ManagedSession()
        {
            ViewModel = new ViewModels.SessionViewModel();
        }

        public ViewModels.SessionViewModel ViewModel { get; set; }
        public AudioSession Session { get; set; }
        public Process Process { get; set; }
        public IntPtr CurrentMainWindow { get; set; }
    }

    public class ManagedDevice
    {
        public ManagedDevice()
        {
            Sessions = new List<ManagedSession>();
            ViewModel = new ViewModels.DeviceViewModel();
        }

        public ViewModels.DeviceViewModel ViewModel { get; set; }
        public MMDevice Device { get; set; }

        public List<ManagedSession> Sessions { get; set; }
    }

    public class ObjectManager
    {
        IList<ViewModels.DeviceViewModel> targetDeviceList;

        public IList<ViewModels.DeviceViewModel> TargetDeviceList
        {
            get
            {
                return targetDeviceList;
            }
            set
            {
                targetDeviceList = value;
            }
        }

        public event Action<Action> OnCallNeeded;

        List<ManagedDevice> devices = new List<ManagedDevice>();
        //PeakPollerThread pollThread;

        ConcurrentQueue<Tuple<ManagedDevice, ManagedSession>> sessionsToRemove = new ConcurrentQueue<Tuple<ManagedDevice, ManagedSession>>();
        ConcurrentQueue<Tuple<ManagedDevice, AudioSession>> sessionsToAdd = new ConcurrentQueue<Tuple<ManagedDevice, AudioSession>>();

        //Thread mtaThread;
        //bool threadRunning = true;
        //bool threadExited = false;
        //
        //void mtaThreadProc()
        //{
        //    while (threadRunning)
        //    {
        //        Thread.Sleep(500);
        //    }
        //    threadExited = true;
        //}

        Guid changeGuid;
        public void Init()
        {
            //if (!threadExited)
            //{
            //    mtaThread = new Thread(mtaThreadProc);
            //    mtaThread.ApartmentState = ApartmentState.MTA;
            //    mtaThread.Start();
            //}
            changeGuid = Guid.NewGuid();

            using (var devEnum = new MMDeviceEnumerator())
            {
                foreach (var device in devEnum.EnumAudioEndpoints(EDataFlow.eRender, DeviceState.DEVICE_STATE_ACTIVE))
                {
                    var manDev = new ManagedDevice()
                    {
                        Device = device
                    };
                    devices.Add(manDev);

                    manDev.ViewModel.Name = device.Description;
                    manDev.ViewModel.FullName = device.Name;
                    manDev.ViewModel.Volume = device.GetMasterVolume();
                    manDev.ViewModel.Muted = device.GetMute();

                    if(device.Properties.ContainsKey("DEVPKEY_DeviceClass_IconPath") &&
                        ((device.Properties["DEVPKEY_DeviceClass_IconPath"] as string) != null))
                    {
                        var iconPath = device.Properties["DEVPKEY_DeviceClass_IconPath"] as string;
                        var tokens = iconPath.Split(',');
                        var icon = NativeMethodsUtilities.ExtractIcon(tokens[0], int.Parse(tokens[1]), true);

                        manDev.ViewModel.Icon = icon.ToBitmap();
                        icon.Dispose();
                    }


                    device.ChangeGuid = changeGuid;
                    device.OnVolumeChanged += (float newMasterVolume, float[] newChannelVolumes, bool newMute, Guid eventContext) =>
                    {
                        if (eventContext != manDev.Device.ChangeGuid)
                        {
                            OnCallNeeded(() =>
                            {
                                manDev.ViewModel.Volume = newMasterVolume;
                                manDev.ViewModel.Muted = newMute;
                            });
                        }
                    };

                    manDev.ViewModel.PropertyChanged += (object sender, PropertyChangedEventArgs e) =>
                    {
                        if (e.PropertyName == "Volume")
                        {
                            manDev.Device.SetMasterVolume(manDev.ViewModel.Volume);
                        }

                        if (e.PropertyName == "Muted")
                        {
                            manDev.Device.SetMute(manDev.ViewModel.Muted);
                        }
                    };

                    foreach (var session in device.SessionManager.GetSessions())
                    {
                        AddSessionToManangedDevice(manDev, session, false);
                    }

                    // Place the system session in front.
                    var sysSess = manDev.Sessions.SingleOrDefault(ms => ms.Session.IsSystemSession);
                    if (sysSess != null)
                    {
                        manDev.Sessions.Remove(sysSess);
                        manDev.Sessions.Insert(0, sysSess);
                    }

                    foreach (var manSess in manDev.Sessions)
                    {
                        manDev.ViewModel.GetSessionList().Add(manSess.ViewModel);
                    }

                    device.SessionManager.OnSessionCreated += (AudioSession newSession) =>
                    {
                        //OnCallNeeded(() => 
                        //{
                        //    lock (manDev.SyncRoot)
                        //    {
                        //        var manSess = CreateManagedSession(newSession);
                        //        manDev.Sessions.Add(manSess);
                        //        manDev.ViewModel.GetSessionList().Add(manSess.ViewModel);
                        //    }
                        //});
                        sessionsToAdd.Enqueue(Tuple.Create(manDev, newSession));
                    };

                    targetDeviceList.Add(manDev.ViewModel);
                }
            }

            //pollThread = new PeakPollerThread();
            //pollThread.Devices = devices;
            //pollThread.OnCallNeeded += pollThread_OnCallNeeded;
            //pollThread.Start();
        }

        void AddSessionToManangedDevice(ManagedDevice manDev, AudioSession session)
        {
            AddSessionToManangedDevice(manDev, session, true);
        }

        void AddSessionToManangedDevice(ManagedDevice manDev, AudioSession session, bool addToViewModel)
        {
            System.Threading.ApartmentState aps = System.Threading.Thread.CurrentThread.GetApartmentState(); ;

            var manSess = CreateManagedSession(session);
            manDev.Sessions.Add(manSess);
            RegisterRemovalThreadSafe(manDev, manSess);
            if (addToViewModel)
            {
                manDev.ViewModel.GetSessionList().Add(manSess.ViewModel);
            }
        }

        string GetSessionDisplayName(AudioSession sess, IntPtr mainWindow)
        {
            if (sess.IsSystemSession)
            {
                return "System Sounds";
            }

            var sessName = sess.GetDisplayName();
            if (!string.IsNullOrEmpty(sessName))
            {
                return sessName;
                //return "Own session name";
            }

            Process proc;

            try
            {
                proc = Process.GetProcessById(sess.ProcessId);
            }
            catch (ArgumentException)
            {
                return "!!";
            }

            using (proc)
            {
                bool is32BitProcess;
                NativeMethods.IsWow64Process(proc.Handle, out is32BitProcess);

                if (!is32BitProcess && !Environment.Is64BitProcess)
                {
                    return "<64-bit from 32-bit>";
                }

                //if (proc.MainWindowHandle != IntPtr.Zero)
                //{
                //    return proc.MainWindowTitle;
                //}
                if (mainWindow != IntPtr.Zero)
                {
                    return NativeMethodsUtilities.GetWindowText(mainWindow);
                }
                else
                {
                    return proc.MainModule.FileVersionInfo.FileDescription;
                }
            }
        }

        Bitmap GetSessionIcon(AudioSession sess, IntPtr mainWindow)
        {
            if (sess.IsSystemSession)
            {
                //var path = @"C:\Users\Michael\Desktop\sys_snd.ico";
                var path = @"C:\Users\Michael\Documents\Visual Studio 2012\Projects\CoreAudio\VolumeMixer\sys_snd.ico";
                Icon icon = new Icon(path, new Size(32, 32));
                return ConvertIconToBitmapAndDispose(icon);
            }

            var iconPath = sess.GetIconPath();
            if (!string.IsNullOrEmpty(iconPath))
            {
                if (iconPath.IndexOf(",-") == -1)
                {
                    Icon icon = Icon.ExtractAssociatedIcon(iconPath);
                    return ConvertIconToBitmapAndDispose(icon);
                }
                else
                {
                    string modulePath;
                    int resId;

                    NativeMethodsUtilities.ExtractModuleAndResourceId("@" + iconPath, out modulePath, out resId);
                    var icon = NativeMethodsUtilities.ExtractIcon(modulePath, resId, true);

                    return ConvertIconToBitmapAndDispose(icon);
                }
            }

            Process proc;

            try
            {
                proc = Process.GetProcessById(sess.ProcessId);
            }
            catch (ArgumentException)
            {
                return GetEmptyBitmap();
            }

            using (proc)
            {
                bool is32BitProcess;
                NativeMethods.IsWow64Process(proc.Handle, out is32BitProcess);

                if (!is32BitProcess && !Environment.Is64BitProcess)
                {
                    return GetEmptyBitmap();
                }

                //if (proc.MainWindowHandle != IntPtr.Zero)
                if (mainWindow != IntPtr.Zero)
                {
                    IntPtr rawIcon = NativeMethods.SendMessageIntPtr(mainWindow, NativeMethods.WM_GETICON, NativeMethods.ICON_BIG, 0);
                    if (rawIcon != IntPtr.Zero)
                    {
                        Icon icon = Icon.FromHandle(rawIcon);
                        return ConvertIconToBitmapAndDispose(icon);
                    }
                    else
                    {
                        IntPtr rawClassIcon = NativeMethods.GetClassLongIntPtr(mainWindow, NativeMethods.GCLP_HICON);
                        if (rawClassIcon != IntPtr.Zero)
                        {
                            Icon classIcon = Icon.FromHandle(rawClassIcon);
                            return ConvertIconToBitmapAndDispose(classIcon);
                        }
                    }
                }

                //if (proc.MainWindowHandle == IntPtr.Zero)
                {
                    Icon icon = Icon.ExtractAssociatedIcon(proc.MainModule.FileName);
                    if (icon != null)
                    {
                        return ConvertIconToBitmapAndDispose(icon);
                    }
                    else
                    {
                        return GetEmptyBitmap();
                    }
                }
            }

            //return GetEmptyBitmap();
        }

        Bitmap GetEmptyBitmap()
        {
            return new Bitmap(1, 1);
        }

        Bitmap ConvertIconToBitmapAndDispose(Icon icon)
        {
            Bitmap bitmapIcon = icon.ToBitmap();
            icon.Dispose();

            return bitmapIcon;
        }

        private ManagedSession CreateManagedSession(AudioSession sess)
        {
            var manSess = new ManagedSession()
            {
                Session = sess
            };

            try
            {
                Process proc = Process.GetProcessById(sess.ProcessId);
                //manSess.CurrentMainWindow = proc.MainWindowHandle;
                manSess.CurrentMainWindow = GetTopmostProcessWindow(proc.Id);
            }
            catch (ArgumentException)
            {
                //manSess.CurrentMainWindow = IntPtr.Zero;
            }

            manSess.ViewModel.Name = GetSessionDisplayName(sess, manSess.CurrentMainWindow);
            manSess.ViewModel.Volume = sess.GetMasterVolume();
            manSess.ViewModel.Muted = sess.GetMute();
            manSess.ViewModel.Icon = GetSessionIcon(sess, manSess.CurrentMainWindow);

            sess.ChangeGuid = changeGuid;
            manSess.Session.OnVolumeChanged += (float newMasterVolume, float[] newChannelVolumes, bool newMute, Guid eventContext) =>
            {
                if (eventContext != manSess.Session.ChangeGuid)
                {
                    OnCallNeeded(() =>
                    {
                        manSess.ViewModel.Volume = newMasterVolume;
                        manSess.ViewModel.Muted = newMute;
                    });
                }
            };

            manSess.ViewModel.PropertyChanged += (object sender, PropertyChangedEventArgs e) =>
            {
                if (e.PropertyName == "Volume")
                {
                    manSess.Session.SetMasterVolume(manSess.ViewModel.Volume);
                }

                if (e.PropertyName == "Muted")
                {
                    manSess.Session.SetMute(manSess.ViewModel.Muted);
                }
            };

            return manSess;
        }

        void RegisterRemovalThreadSafe(ManagedDevice manDev, ManagedSession manSess)
        {
            AudioSession sess = manSess.Session;

            if (sess.IsSystemSession)
            {
                return;
            }

            sess.OnSessionDisconnection += (reason) =>
            {
                QueueRemoval(manDev, manSess);
            };
            sess.OnStateChanged += (state) =>
            {
                if (state == AudioSessionState.Expired)
                {
                    QueueRemoval(manDev, manSess);
                }
            };

            int pid = sess.ProcessId;
            Process p;
            try
            {
                p = Process.GetProcessById(pid);
            }
            catch (ArgumentException)
            {
                return;
            }


            if (p != null)
            {
                bool hasExited;

                try
                {
                    hasExited = p.HasExited;
                }
                catch (System.ComponentModel.Win32Exception)
                {
                    return;
                }

                if (!hasExited)
                {
                    manSess.Process = p;
                    p.EnableRaisingEvents = true;
                    p.Exited += (s, e) =>
                    {
                        QueueRemoval(manDev, manSess);
                    };
                }
            }

        }

        void QueueRemoval(ManagedDevice manDev, ManagedSession manSess)
        {
            sessionsToRemove.Enqueue(Tuple.Create(manDev, manSess));
        }


        //void pollThread_OnCallNeeded(Action obj)
        //{
        //    OnCallNeeded(obj);
        //}

        public void DeInit()
        {
            //pollThread.Stop();

            foreach (var manDev in devices)
            {
                manDev.Device.Dispose();
            }

            //if (!threadExited)
            //{
            //    threadRunning = false;
            //    mtaThread.Join();
            //}
        }

        public void UpdateState()
        {
            UpdatePeaks();
            UpdateSessions();
        }

        public void UpdateStatePerSecond()
        {
            UpdateTitles();
        }

        void UpdateTitles()
        {
            foreach (var manDev in devices)
            {
                foreach (var manSess in manDev.Sessions)
                {
                    if (manSess.CurrentMainWindow == IntPtr.Zero)
                    {
                        continue;
                    }

#if false
                    IntPtr newWindow = manSess.Process.MainWindowHandle;
                    if (manSess.CurrentMainWindow == newWindow)
                    {
                        continue;
                    }

                    manSess.CurrentMainWindow = newWindow;
                    manSess.ViewModel.Name = GetSessionDisplayName(manSess.Session);
#endif
                    IntPtr newTop = GetTopmostProcessWindow(manSess.Process.Id);
                    if (newTop != manSess.CurrentMainWindow)
                    {
                        manSess.CurrentMainWindow = newTop;
                        //manSess.ViewModel.Name = NativeMethodsUtilities.GetWindowText(newTop);
                        manSess.ViewModel.Name = GetSessionDisplayName(manSess.Session, manSess.CurrentMainWindow);
                    }
                    else
                    {
#if false
                        string newTitle = NativeMethodsUtilities.GetWindowText(manSess.CurrentMainWindow);
                        if (manSess.ViewModel.Name != newTitle)
                        {
                            manSess.ViewModel.Name = newTitle;
                        }
#endif
                        manSess.ViewModel.Name = GetSessionDisplayName(manSess.Session, manSess.CurrentMainWindow);
                    }
                }
            }
        }

#if false
        static IntPtr GetTopmostProcessWindow(IntPtr window, int pid)
        {
            IntPtr lastWindow = window;

            while (true)
            {
                IntPtr prevWindow = NativeMethods.GetWindow(lastWindow, NativeMethods.GW_HWNDPREV);
                if (prevWindow != IntPtr.Zero)
                {
                    int currentWindowPid;
                    NativeMethods.GetWindowThreadProcessId(prevWindow, out currentWindowPid);
                    if (currentWindowPid != pid || !NativeMethods.IsWindowVisible(prevWindow))
                    {
                        break;
                    }
                    else
                    {
                        lastWindow = prevWindow;
                    }
                }
                else
                {
                    break;
                }
            }

            return lastWindow;
        }
#endif
        static IntPtr GetTopmostProcessWindow(int pid)
        {
            IntPtr topmostWindow = IntPtr.Zero;

            NativeMethods.EnumWindowsProc callback = (IntPtr hWnd, IntPtr lParam) =>
            {
                int currentWindowPid;
                NativeMethods.GetWindowThreadProcessId(hWnd, out currentWindowPid);
                if(currentWindowPid == pid && NativeMethods.IsWindowVisible(hWnd))
                {
                    topmostWindow = hWnd;
                    return false;
                }

                return true;
            };
            GCHandle handle = GCHandle.Alloc(callback);
            NativeMethods.EnumWindows(callback, IntPtr.Zero);
            handle.Free();

            return topmostWindow;
        }

        private void UpdateSessions()
        {
            Tuple<ManagedDevice, AudioSession> sessionToAdd;
            while (sessionsToAdd.TryDequeue(out sessionToAdd))
            {
                AddSessionToManangedDevice(sessionToAdd.Item1, sessionToAdd.Item2);
            }

            Tuple<ManagedDevice, ManagedSession> sessionToRemove;
            while (sessionsToRemove.TryDequeue(out sessionToRemove))
            {
                sessionToRemove.Item1.ViewModel.GetSessionList().Remove(sessionToRemove.Item2.ViewModel);
                sessionToRemove.Item2.Session.Dispose();
                if (sessionToRemove.Item2.Process != null)
                {
                    sessionToRemove.Item2.Process.Dispose();
                }
                sessionToRemove.Item1.Sessions.Remove(sessionToRemove.Item2);
            }
        }

        void UpdatePeaks()
        {
            foreach (var manDev in devices)
            {
                manDev.ViewModel.Level = manDev.Device.GetMasterLevel();

                foreach (var manSess in manDev.Sessions)
                {
                    manSess.ViewModel.Level = manSess.Session.GetMasterLevel();
                }
            }
        }
    }
}
