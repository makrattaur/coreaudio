using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreAudioApi.COMWrappers
{
    internal class AudioVolumeNotificationData
    {
        internal AudioVolumeNotificationData()
        {

        }

        public Guid guidEventContext { get; internal set; }
        public bool bMuted { get; internal set; }
        public float fMasterVolume { get; internal set; }
        public uint nChannels { get; internal set; }
        public float[] afChannelVolumes { get; internal set; }
    }
}
