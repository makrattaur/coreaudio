using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CoreAudioApi.COMInterop;
using CoreAudioApi.COMInterop.Session;
using CoreAudioApi.COMWrappers;

using System.Collections.ObjectModel;

namespace CoreAudioApi
{
    public class MMDevice : IDisposable, IVolumeAdjustable, ILevelMonitorable
    {
        IMMDevice device;
        Dictionary<string, object> properties = new Dictionary<string,object>();
        ReadOnlyDictionary<string, object> readOnlyProperties;
        AudioEndpointVolume volumeCtl;
        AudioMeterInformation meter;

        internal MMDevice(IMMDevice device)
        {
            this.device = device;

            BuildPropertiesDictionary();

            volumeCtl = Activate<IAudioEndpointVolume, AudioEndpointVolume>();
            meter = Activate<IAudioMeterInformation, AudioMeterInformation>();
            SessionManager = Activate<IAudioSessionManager2, AudioSessionManager>();

            volumeCtl.OnVolumeChange += volumeCtl_OnVolumeChange;
        }

        private void BuildPropertiesDictionary()
        {
            IPropertyStore props;
            device.OpenPropertyStore(StgmAccess.STGM_READ, out props);

            uint propCount;
            props.GetCount(out propCount);
            for (uint j = 0; j < propCount; j++)
            {
                PROPERTYKEY key;
                props.GetAt(j, out key);

                string keyName;
                keyName = PropVariantKeys.GetFriendlyName(key);
                if (keyName == null)
                    continue;

                PROPVARIANT value = new PROPVARIANT();
                try
                {
                    props.GetValue(ref key, out value);

                    object valueObj = null;
                    try
                    {
                        valueObj = value.Value;
                    }
                    catch (NotSupportedException)
                    {
                        continue;
                    }

                    properties.Add(keyName, valueObj);
                }
                finally
                {
                    value.Clear();
                }
            }

            readOnlyProperties = new ReadOnlyDictionary<string, object>(properties);
        }

        public DeviceState State
        {
            get
            {
                uint state;
                device.GetState(out state);

                return (DeviceState)state;
            }
        }

        public string Id
        {
            get
            {
                string id;
                device.GetId(out id);

                return id;
            }
        }

        public string AdapterName
        {
            get
            {
                return (string)properties["DEVPKEY_DeviceInterface_FriendlyName"];
            }
        }

        public string Description
        {
            get
            {
                return (string)properties["DEVPKEY_Device_DeviceDesc"];
            }
        }

        public string Name
        {
            get
            {
                return (string)properties["DEVPKEY_Device_FriendlyName"];
            }
        }

        public IDictionary<string, object> Properties
        {
            get
            {
                return readOnlyProperties;
            }
        }

        public Guid ChangeGuid { get; set; }

        public AudioSessionManager SessionManager { get; private set; }

        private static T CreateInstance<T>(params object[] args)
        {
            return (T)Activator.CreateInstance(typeof(T),
                System.Reflection.BindingFlags.NonPublic |
                System.Reflection.BindingFlags.Instance,
                null, args, null);
        }

        public int ChannelCount
        {
            get
            {
                return volumeCtl.ChannelCount;
            }
        }

        private TWrapper Activate<TInterface, TWrapper>()
        {
            TInterface comInterface;
            Guid guid = typeof(TInterface).GUID;
            object obj;
            device.Activate(ref guid, CLSCTX.CLSCTX_ALL, IntPtr.Zero, out obj);
            comInterface = (TInterface)obj;
            return CreateInstance<TWrapper>(comInterface);
        }

        public int GetChannelCount()
        {
            return volumeCtl.ChannelCount;
        }

        public void SetMasterVolume(float volume)
        {
            SetMasterVolume(volume, ChangeGuid);
        }

        public void SetMasterVolume(float volume, Guid eventContext)
        {
            volumeCtl.SetMasterVolume(volume, eventContext);
        }

        public float GetMasterVolume()
        {
            return volumeCtl.GetMasterVolume();
        }

        public void SetChannelVolume(int channelIndex, float volume, Guid eventContext)
        {
            if (channelIndex >= GetChannelCount())
                throw new IndexOutOfRangeException();

            volumeCtl.SetChannelVolume(channelIndex, volume, eventContext);
        }

        public void SetChannelVolume(int channelIndex, float volume)
        {
            SetChannelVolume(channelIndex, volume, ChangeGuid);
        }

        public float GetChannelVolume(int channelIndex)
        {
            if (channelIndex >= GetChannelCount())
                throw new IndexOutOfRangeException();

            return volumeCtl.GetChannelVolume(channelIndex);
        }

        public void SetMute(bool mute)
        {
            SetMute(mute, ChangeGuid);
        }

        public void SetMute(bool mute, Guid eventContext)
        {
            volumeCtl.SetMute(mute, eventContext);
        }

        public bool GetMute()
        {
            return volumeCtl.GetMute();
        }

        public event VolumeChangedEventHandler OnVolumeChanged;

        void volumeCtl_OnVolumeChange(AudioVolumeNotificationData data)
        {
            VolumeChangedEventHandler del = OnVolumeChanged;
            if (del != null)
                del(data.fMasterVolume, data.afChannelVolumes, data.bMuted, data.guidEventContext);
        }

        public void Dispose()
        {
            if (volumeCtl != null)
            {
                volumeCtl.Dispose();
                volumeCtl = null;
            }

            if (meter != null)
            {
                meter.Dispose();
                meter = null;
            }

            if (SessionManager != null)
            {
                SessionManager.Dispose();
                SessionManager = null;
            }
        }

        float[] peaks = new float[8];
        private void UpdatePeaks()
        {
            meter.GetChannelPeaks(peaks);
        }

        public float GetMasterLevel()
        {
            UpdatePeaks();
            return peaks.Take(GetChannelCount()).Average();
        }

        public float GetChannelLevel(int channelIndex)
        {
            if (channelIndex >= GetChannelCount())
                throw new IndexOutOfRangeException();

            UpdatePeaks();
            return peaks[channelIndex];
        }

        public EndpointHardwareSupport QueryHardwareSupport()
        {
            return volumeCtl.QueryHardwareSupport();
        }
    }
}
