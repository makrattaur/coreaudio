using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Runtime.InteropServices;

namespace CoreAudioApi.COMInterop.Session
{
    [Guid("bfb7ff88-7239-4fc9-8fa2-07c950be9c6d"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IAudioSessionControl2
    {
        // IAudioSessionControl

        void GetState([Out] out AudioSessionState pRetVal);
        void GetDisplayName([Out, MarshalAs(UnmanagedType.LPWStr)] out string pRetVal);
        void SetDisplayName([Out, MarshalAs(UnmanagedType.LPWStr)] string Value, ref Guid EventContext);
        void GetIconPath([Out, MarshalAs(UnmanagedType.LPWStr)] out string pRetVal);
        void SetIconPath([Out, MarshalAs(UnmanagedType.LPWStr)] string Value, ref Guid EventContext);
        void GetGroupingParam([Out] out Guid pRetVal);
        void SetGroupingParam([In] ref Guid Override, [In] ref Guid EventContext);
        void RegisterAudioSessionNotification([In] IAudioSessionEvents NewNotifications);
        void UnregisterAudioSessionNotification([In] IAudioSessionEvents NewNotifications);

        // IAudioSessionControl2

        void GetSessionIdentifier([Out, MarshalAs(UnmanagedType.LPWStr)] out string pRetVal);
        void GetSessionInstanceIdentifier([Out, MarshalAs(UnmanagedType.LPWStr)] out string pRetVal);
        void GetProcessId([Out] out uint pRetVal);
        [PreserveSig] int IsSystemSoundsSession();
        void SetDuckingPreference([In, MarshalAs(UnmanagedType.Bool)] bool optOut);
    }
}
