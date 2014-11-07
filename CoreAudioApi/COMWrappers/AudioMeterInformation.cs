using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CoreAudioApi.COMInterop;
using System.Runtime.InteropServices;

namespace CoreAudioApi.COMWrappers
{
    internal class AudioMeterInformation : IDisposable
    {
        IAudioMeterInformation meter;
        IntPtr buffer;

        internal AudioMeterInformation(IAudioMeterInformation meter)
        {
            this.meter = meter;

            buffer = Marshal.AllocCoTaskMem(sizeof(float) * ChannelCount);
        }

        ~AudioMeterInformation()
        {
            Dispose();
        }

        public int ChannelCount
        {
            get
            {
                uint channelCount;
                meter.GetMeteringChannelCount(out channelCount);

                return (int)channelCount;
            }
        }

        public float GetMasterPeak()
        {
            float masterPeak;
            meter.GetPeakValue(out masterPeak);

            return masterPeak;
        }

        public float[] GetChannelPeaks()
        {
            float[] peaks = new float[ChannelCount];
            GetChannelPeaks(peaks);

            return peaks;
        }

        public void GetChannelPeaks(float[] peaks)
        {
            if (peaks == null)
                throw new ArgumentNullException("peaks");

            int channelCount = ChannelCount;
            if(peaks.Length < channelCount)
                throw new ArgumentException("Peaks array too small.");

            meter.GetChannelsPeakValues((uint)channelCount, buffer);
            Marshal.Copy(buffer, peaks, 0, peaks.Length);
        }

        public EndpointHardwareSupport QueryHardwareSupport()
        {
            EndpointHardwareSupport support;
            meter.QueryHardwareSupport(out support);

            return support;
        }

        public void Dispose()
        {
            if (buffer != IntPtr.Zero)
            {
                Marshal.FreeCoTaskMem(buffer);
                buffer = IntPtr.Zero;
            }
        }
    }
}
