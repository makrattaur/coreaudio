using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CoreAudioApi.COMInterop;
using System.Runtime.InteropServices;

namespace CoreAudioApi.COMWrappers
{
    internal delegate void AudioEndpointVolumeCallback(AudioVolumeNotificationData data);

    internal class AudioEndpointVolume : IDisposable
    {
        IAudioEndpointVolume volumeCtl;
        CIAudioEndpointVolumeCallback callback;

        internal AudioEndpointVolume(IAudioEndpointVolume volumeCtl)
        {
            this.volumeCtl = volumeCtl;
            ChangeGuid = Guid.Empty;

            callback = new CIAudioEndpointVolumeCallback(this);
            volumeCtl.RegisterControlChangeNotify(callback);
        }

        ~AudioEndpointVolume()
        {
            Dispose();
        }

        public event AudioEndpointVolumeCallback OnVolumeChange;
        public Guid ChangeGuid { get; set; }

        public int ChannelCount
        {
            get 
            {
                uint channelCount;
                volumeCtl.GetChannelCount(out channelCount);

                return (int)channelCount;
            }
        }

        public float GetMasterVolume()
        {
            float masterVolume;
            volumeCtl.GetMasterVolumeLevelScalar(out masterVolume);

            return masterVolume;
        }

        public float GetChannelVolume(int channelIndex)
        {
            if (channelIndex < 0 && channelIndex >= ChannelCount)
                throw new ArgumentException("Index out of bounds.");

            float channelVolume;
            volumeCtl.GetChannelVolumeLevelScalar((uint)channelIndex, out channelVolume);

            return channelVolume;
        }

        public void SetMasterVolume(float level, Guid changeGuid)
        {
            volumeCtl.SetMasterVolumeLevelScalar(level, ref changeGuid);
        }

        public void SetMasterVolume(float level)
        {
            SetMasterVolume(level, ChangeGuid);
        }

        public void SetChannelVolume(int channelIndex, float level, Guid changeGuid)
        {
            if (channelIndex < 0 && channelIndex >= ChannelCount)
                throw new ArgumentException("Index out of bounds.");

            volumeCtl.SetChannelVolumeLevelScalar((uint)channelIndex, level, ref changeGuid);
        }

        public void SetChannelVolume(int channelIndex, float level)
        {
            SetChannelVolume(channelIndex, level, ChangeGuid);
        }

        public bool GetMute()
        {
            bool mute;
            volumeCtl.GetMute(out mute);

            return mute;
        }

        public void SetMute(bool state, Guid changeGuid)
        {
            volumeCtl.SetMute(state, ref changeGuid);
        }

        public void SetMute(bool state)
        {
            SetMute(state, ChangeGuid);
        }

        public EndpointHardwareSupport QueryHardwareSupport()
        {
            EndpointHardwareSupport support;
            volumeCtl.QueryHardwareSupport(out support);

            return support;
        }

        internal void FireEvent(AudioVolumeNotificationData data)
        {
            AudioEndpointVolumeCallback del = OnVolumeChange;
            if (del != null)
            {
                del(data);
            }
        }

        public void Dispose()
        {
            if (callback != null)
            {
                volumeCtl.UnregisterControlChangeNotify(callback);
                callback = null;
            }
        }
    }

    internal class CIAudioEndpointVolumeCallback : IAudioEndpointVolumeCallback
    {
        AudioEndpointVolume parent;

        public CIAudioEndpointVolumeCallback(AudioEndpointVolume parent)
        {
            this.parent = parent;
        }

        static AudioVolumeNotificationData GetClassFromPointer(IntPtr ptr)
        {
            AUDIO_VOLUME_NOTIFICATION_DATA data = (AUDIO_VOLUME_NOTIFICATION_DATA)Marshal.PtrToStructure(ptr, typeof(AUDIO_VOLUME_NOTIFICATION_DATA));

            IntPtr offset = Marshal.OffsetOf(typeof(AUDIO_VOLUME_NOTIFICATION_DATA), "afChannelVolumes");
            float[] volumes = new float[data.nChannels];
            IntPtr firstFloat = IntPtr.Add(ptr, (int)offset);
            Marshal.Copy(firstFloat, volumes, 0, volumes.Length);

            return new AudioVolumeNotificationData()
            {
                guidEventContext = data.guidEventContext,
                bMuted = data.bMuted,
                fMasterVolume = data.fMasterVolume,
                nChannels = data.nChannels,
                afChannelVolumes = volumes
            };
        }

        public void OnNotify(IntPtr pNotify)
        {
            var data = GetClassFromPointer(pNotify);
            parent.FireEvent(data);
        }
    }
}
