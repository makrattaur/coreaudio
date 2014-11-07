using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Runtime.InteropServices;

namespace CoreAudioApi.COMInterop
{
    [Guid("A95664D2-9614-4F35-A746-DE8DB63617E6"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IMMDeviceEnumerator
    {
        void EnumAudioEndpoints([In] EDataFlow dataFlow, [In] DeviceState dwStateMask, [Out] out IMMDeviceCollection ppDevices);
        void GetDefaultAudioEndpoint([In] EDataFlow dataFlow, [In] ERole role, [Out] out IMMDevice ppEndpoint);
        void GetDevice([In, MarshalAs(UnmanagedType.LPWStr)] string pwstrId, [Out] out IMMDevice ppDevice);
        void RegisterEndpointNotificationCallback([In] IMMNotificationClient pClient);
        void UnregisterEndpointNotificationCallback([In] IMMNotificationClient pClient);
    }
}
