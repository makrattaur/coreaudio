using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Runtime.InteropServices;

namespace CoreAudioApi.COMInterop.Session
{
    [Guid("F4B1A599-7266-4319-A8CA-E70ACB11E8CD"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IAudioSessionControl
    {
        void GetState([Out] out AudioSessionState pRetVal);
        void GetDisplayName([Out, MarshalAs(UnmanagedType.LPWStr)] out string pRetVal);
        void SetDisplayName([Out, MarshalAs(UnmanagedType.LPWStr)] string Value, ref Guid EventContext);
        void GetIconPath([Out, MarshalAs(UnmanagedType.LPWStr)] out string pRetVal);
        void SetIconPath([Out, MarshalAs(UnmanagedType.LPWStr)] string Value, ref Guid EventContext);
        void GetGroupingParam([Out] out Guid pRetVal);
        void SetGroupingParam([In] ref Guid Override, [In] ref Guid EventContext);
        void RegisterAudioSessionNotification([In] IAudioSessionEvents NewNotifications);
        void UnregisterAudioSessionNotification([In] IAudioSessionEvents NewNotifications);
    }
}
