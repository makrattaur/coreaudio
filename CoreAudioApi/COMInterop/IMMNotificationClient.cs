using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Runtime.InteropServices;

namespace CoreAudioApi.COMInterop
{
    [Guid("7991EEC9-7E89-4D85-8390-6C703CEC60C0"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IMMNotificationClient
    {
        void OnDeviceStateChanged([In, MarshalAs(UnmanagedType.LPWStr)] string pwstrDeviceId, [In] uint dwNewState);
        void OnDeviceAdded([In, MarshalAs(UnmanagedType.LPWStr)] string pwstrDeviceId);
        void OnDeviceRemoved([In, MarshalAs(UnmanagedType.LPWStr)] string pwstrDeviceId);
        void OnDefaultDeviceChanged([In] EDataFlow flow, [In] ERole role, [In, MarshalAs(UnmanagedType.LPWStr)] string pwstrDefaultDeviceId);
        void OnPropertyValueChanged([In, MarshalAs(UnmanagedType.LPWStr)] string pwstrDeviceId, [In] PROPERTYKEY key);        
    }
}
