using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreAudioApi
{
    public delegate void VolumeChangedEventHandler(float newMasterVolume, float[] newChannelVolumes, bool newMute, Guid eventContext);

    static public class IVolumeAdjustableExtensions
    {
        public static void GetChannelVolumes(this IVolumeAdjustable iva, float[] volumes)
        {
            int count = iva.GetChannelCount();
            if (volumes.Length < count)
                throw new ArgumentException("The volumes array is too small to contain all channnels.");

            for (int i = 0; i < count; i++)
            {
                volumes[i] = iva.GetChannelVolume(i);
            }
        }

        public static float[] GetChannelVolumes(this IVolumeAdjustable iva)
        {
            float[] volumes = new float[iva.GetChannelCount()];
            GetChannelVolumes(iva, volumes);

            return volumes;
        }
    }

    public interface IVolumeAdjustable
    {
        int GetChannelCount();
        void SetMasterVolume(float volume);
        float GetMasterVolume();
        void SetChannelVolume(int channelIndex, float volume);
        float GetChannelVolume(int channelIndex);
        void SetMute(bool mute);
        bool GetMute();

        event VolumeChangedEventHandler OnVolumeChanged;
    }
}
