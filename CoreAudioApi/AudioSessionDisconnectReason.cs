using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreAudioApi
{
    public enum AudioSessionDisconnectReason
    {
        DeviceRemoval = 0,
        ServerShutdown = 1,
        FormatChanged = 2,
        SessionLogoff = 3,
        SessionDisconnected = 4,
        ExclusiveModeOverride = 5
    }
}
