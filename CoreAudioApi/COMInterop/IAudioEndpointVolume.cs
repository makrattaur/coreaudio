using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Runtime.InteropServices;

namespace CoreAudioApi.COMInterop
{
    [Guid("5CDF2C82-841E-4546-9722-0CF74078229A"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IAudioEndpointVolume
    {
        void RegisterControlChangeNotify([In] IAudioEndpointVolumeCallback pNotify);
        void UnregisterControlChangeNotify([In] IAudioEndpointVolumeCallback pNotify);
        void GetChannelCount([Out] out uint pnChannelCount);
        void SetMasterVolumeLevel([In] float fLevelDB, [In] ref Guid pguidEventContext);
        void SetMasterVolumeLevelScalar([In] float fLevel, [In] ref Guid pguidEventContext);
        void GetMasterVolumeLevel([Out] out float pfLevelDB);
        void GetMasterVolumeLevelScalar([Out] out float pfLevel);
        void SetChannelVolumeLevel([In] uint nChannel, float fLevelDB, [In] ref Guid pguidEventContext);
        void SetChannelVolumeLevelScalar([In] uint nChannel, float fLevel, [In] ref Guid pguidEventContext);
        void GetChannelVolumeLevel([In] uint nChannel, [Out] out float pfLevelDB);
        void GetChannelVolumeLevelScalar([In] uint nChannel, [Out] out float pfLevel);
        void SetMute([In, MarshalAs(UnmanagedType.Bool)] bool bMute, [In] ref Guid pguidEventContext);
        void GetMute([Out, MarshalAs(UnmanagedType.Bool)] out bool pbMute);
        void GetVolumeStepInfo([Out] out uint pnStep, [Out] out uint pnStepCount);
        void VolumeStepUp([In] ref Guid pguidEventContext);
        void VolumeStepDown([In] ref Guid pguidEventContext);
        void QueryHardwareSupport([Out] out EndpointHardwareSupport pdwHardwareSupportMask);
        void GetVolumeRange([Out] out float pflVolumeMindB, [Out] out float pflVolumeMaxdB, [Out] out float pflVolumeIncrementdB);

    }
}
