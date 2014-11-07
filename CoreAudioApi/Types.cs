using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreAudioApi
{
    public enum EDataFlow
    {
        eRender,
        eCapture,
        eAll,
        EDataFlow_enum_count
    }

    public enum ERole
    {
        eConsole,
        eMultimedia,
        eCommunications,
        ERole_enum_count
    }

    [Flags]
    public enum DeviceState : uint
    {
        DEVICE_STATE_ACTIVE = 0x00000001,
        DEVICE_STATE_DISABLED = 0x00000002,
        DEVICE_STATE_NOTPRESENT = 0x00000004,
        DEVICE_STATE_UNPLUGGED = 0x00000008,
        DEVICE_STATEMASK_ALL = 0x0000000F
    }

    [Flags]
    public enum EndpointHardwareSupport : uint
    {
        ENDPOINT_HARDWARE_SUPPORT_VOLUME = 0x00000001,
        ENDPOINT_HARDWARE_SUPPORT_MUTE = 0x00000002,
        ENDPOINT_HARDWARE_SUPPORT_METER = 0x00000004
    }
}
