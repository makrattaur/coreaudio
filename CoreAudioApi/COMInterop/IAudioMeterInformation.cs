using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Runtime.InteropServices;

namespace CoreAudioApi.COMInterop
{
    [Guid("C02216F6-8C67-4B5B-9D00-D008E73E0064"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IAudioMeterInformation
    {
        void GetPeakValue([Out] out float pfPeak);
        void GetMeteringChannelCount([Out] out uint pnChannelCount);
        void GetChannelsPeakValues([In] uint u32ChannelCount, [Out] IntPtr afPeakValues);
        void QueryHardwareSupport([Out] out EndpointHardwareSupport pdwHardwareSupportMask);
    }
}
