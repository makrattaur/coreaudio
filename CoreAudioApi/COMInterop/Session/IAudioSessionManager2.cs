using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Runtime.InteropServices;

namespace CoreAudioApi.COMInterop.Session
{
    [Guid("77AA99A0-1BD6-484F-8BC7-2C654C9A9B6F"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IAudioSessionManager2
    {
        // IAudioSessionManager

        void GetAudioSessionControl([In] ref Guid AudioSessionGuid, [In] uint StreamFlags, [Out] out IAudioSessionControl SessionControl);
        void GetSimpleAudioVolume([In] ref Guid AudioSessionGuid, [In] uint StreamFlags, [Out] out ISimpleAudioVolume AudioVolume);

        // IAudioSessionManager2

        void GetSessionEnumerator([Out] out IAudioSessionEnumerator SessionEnum);
        void RegisterSessionNotification([In] IAudioSessionNotification SessionNotification);
        void UnregisterSessionNotification([In] IAudioSessionNotification SessionNotification);
        void RegisterDuckNotification([In, MarshalAs(UnmanagedType.LPWStr)] string sessionID, [In] IAudioVolumeDuckNotification duckNotification);
        void UnregisterDuckNotification([In] IAudioVolumeDuckNotification duckNotification);
    }
}
