using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Runtime.InteropServices;

namespace CoreAudioApi.COMInterop.Session
{
    [Guid("87CE5498-68D6-44E5-9215-6DA47EF883D8"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface ISimpleAudioVolume
    {
        void SetMasterVolume([In] float fLevel, ref Guid EventContext);
        void GetMasterVolume([Out] out float pfLevel);
        void SetMute([In, MarshalAs(UnmanagedType.Bool)] bool bMute, ref Guid EventContext);
        void GetMute([Out, MarshalAs(UnmanagedType.Bool)] out bool pbMute);
    }
}
