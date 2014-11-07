using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Runtime.InteropServices;

namespace CoreAudioApi.COMInterop.Session
{
    [Guid("C3B284D4-6D39-4359-B3CF-B56DDB3BB39C"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IAudioVolumeDuckNotification
    {
        void OnVolumeDuckNotification([In, MarshalAs(UnmanagedType.LPWStr)] string sessionID, UInt32 countCommunicationSessions);
        void OnVolumeUnduckNotification([In, MarshalAs(UnmanagedType.LPWStr)] string sessionID);
    }
}
