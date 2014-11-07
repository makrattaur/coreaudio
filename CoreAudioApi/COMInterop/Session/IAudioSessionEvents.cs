using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Runtime.InteropServices;

namespace CoreAudioApi.COMInterop.Session
{
    [Guid("24918ACC-64B3-37C1-8CA9-74A66E9957A8"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IAudioSessionEvents
    {
        void OnDisplayNameChanged([In, MarshalAs(UnmanagedType.LPWStr)] string NewDisplayName, [In] ref Guid EventContext);
        void OnIconPathChanged([In, MarshalAs(UnmanagedType.LPWStr)] string NewIconPath, [In] ref Guid EventContext);
        void OnSimpleVolumeChanged([In] float NewVolume, [In, MarshalAs(UnmanagedType.Bool)] bool NewMute, [In] ref Guid EventContext);
        void OnChannelVolumeChanged([In] uint ChannelCount, [In, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)] float[] NewChannelVolumeArray, [In] uint ChangedChannel, [In] ref Guid EventContext);
        void OnGroupingParamChanged([In] ref Guid NewGroupingParam, [In] ref Guid EventContext);
        void OnStateChanged([In] AudioSessionState NewState);
        void OnSessionDisconnected([In] AudioSessionDisconnectReason DisconnectReason);
    }
}
