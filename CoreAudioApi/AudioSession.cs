using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CoreAudioApi.COMInterop;
using CoreAudioApi.COMInterop.Session;
using CoreAudioApi.COMWrappers;

namespace CoreAudioApi
{
    public delegate void DisplayNameChangedEventHandler(string newDisplayName, Guid eventContext);
    public delegate void IconPathChangedEventHandler(string newIconPath, Guid eventContext);
    //public delegate void VolumeChangedEventHandler(float newVolume, bool newMute, Guid eventContext);
    public delegate void GroupingParamChangedEventHandler(Guid newGroupingParam, Guid eventContext);
    public delegate void StateChangedEventHandler(AudioSessionState newState);
    public delegate void SessionDisconnectionEventHandler(AudioSessionDisconnectReason disconnectReason);

    internal class CIAudioSessionEventsCallback : IAudioSessionEvents
    {
        AudioSession parent;

        public CIAudioSessionEventsCallback(AudioSession parent)
        {
            this.parent = parent;
        }

        public void OnDisplayNameChanged(string NewDisplayName, ref Guid EventContext)
        {
            parent.FireDisplayNameChanged(NewDisplayName, EventContext);
        }

        public void OnIconPathChanged(string NewIconPath, ref Guid EventContext)
        {
            parent.FireIconPathChanged(NewIconPath, EventContext);
        }

        public void OnSimpleVolumeChanged(float NewVolume, bool NewMute, ref Guid EventContext)
        {
            parent.FireVolumeChanged(NewVolume, NewMute, EventContext);
        }

        public void OnChannelVolumeChanged(uint ChannelCount, float[] NewChannelVolumeArray, uint ChangedChannel, ref Guid EventContext)
        {
            // No impl.
        }

        public void OnGroupingParamChanged(ref Guid NewGroupingParam, ref Guid EventContext)
        {
            parent.FireGroupingParamChanged(NewGroupingParam, EventContext);
        }

        public void OnStateChanged(AudioSessionState NewState)
        {
            parent.FireStateChanged(NewState);
        }

        public void OnSessionDisconnected(AudioSessionDisconnectReason DisconnectReason)
        {
            parent.FireSessionDisconnection(DisconnectReason);
        }
    }

    public class AudioSession : IDisposable, ILevelMonitorable, IVolumeAdjustable
    {
        IAudioSessionControl2 sessCtl;
        AudioMeterInformation meter;
        ISimpleAudioVolume volumeCtl;
        CIAudioSessionEventsCallback callback;

        internal AudioSession(IAudioSessionControl2 sessCtl)
        {
            this.sessCtl = sessCtl;
            ChangeGuid = Guid.Empty;

            meter = new AudioMeterInformation((IAudioMeterInformation)sessCtl);
            volumeCtl = (ISimpleAudioVolume)sessCtl;

            callback = new CIAudioSessionEventsCallback(this);
            sessCtl.RegisterAudioSessionNotification(callback);
        }

        internal AudioSession(IAudioSessionControl sessCtl)
            : this((IAudioSessionControl2)sessCtl)
        {
        }

        ~AudioSession()
        {
            Dispose();
        }

        public event DisplayNameChangedEventHandler OnDisplayNameChanged;
        public event IconPathChangedEventHandler OnIconPathChanged;
        public event VolumeChangedEventHandler OnVolumeChanged;
        public event GroupingParamChangedEventHandler OnGroupingParamChanged;
        public event StateChangedEventHandler OnStateChanged;
        public event SessionDisconnectionEventHandler OnSessionDisconnection;

        public Guid ChangeGuid { get; set; }

        public AudioSessionState State
        {
            get
            {
                AudioSessionState currentState;
                sessCtl.GetState(out currentState);

                return currentState;
            }
        }

        public string Identifier
        {
            get
            {
                string sessIdentifier;
                sessCtl.GetSessionIdentifier(out sessIdentifier);

                return sessIdentifier;
            }
        }

        public string InstanceIdentifier
        {
            get
            {
                string sessInstanceIdentifier;
                sessCtl.GetSessionInstanceIdentifier(out sessInstanceIdentifier);

                return sessInstanceIdentifier;
            }
        }

        public int ProcessId
        {
            get
            {
                uint pid;
                sessCtl.GetProcessId(out pid);

                return (int)pid;
            }
        }

        const int S_OK = 0;
        const int S_FALSE = 1;
        public bool IsSystemSession
        {
            get
            {
                return sessCtl.IsSystemSoundsSession() == S_OK;
            }
        }

        public int GetChannelCount()
        {
            return 1;
        }

        float[] peaks = new float[8];
        public float GetMasterLevel()
        {
            int channelCount = meter.ChannelCount;
            if (channelCount > 0)
            {
                meter.GetChannelPeaks(peaks);
                return peaks.Take(channelCount).Average();
            }
            else
            {
                return 0.0f;
            }
        }

        public float GetChannelLevel(int channelIndex)
        {
            if (channelIndex >= GetChannelCount())
                throw new IndexOutOfRangeException();

            return GetMasterLevel();
        }

        public void SetMasterVolume(float volume, Guid eventContext)
        {
            volumeCtl.SetMasterVolume(volume, ref eventContext);
        }

        public void SetMasterVolume(float volume)
        {
            SetMasterVolume(volume, ChangeGuid);
        }

        public float GetMasterVolume()
        {
            float volume;
            volumeCtl.GetMasterVolume(out volume);

            return volume;
        }

        public void SetChannelVolume(int channelIndex, float volume, Guid eventContext)
        {
            if (channelIndex >= GetChannelCount())
                throw new IndexOutOfRangeException();

            volumeCtl.SetMasterVolume(volume, ref eventContext);
        }

        public void SetChannelVolume(int channelIndex, float volume)
        {
            SetChannelVolume(channelIndex, volume, ChangeGuid);
        }

        public float GetChannelVolume(int channelIndex)
        {
            if (channelIndex >= GetChannelCount())
                throw new IndexOutOfRangeException();

            float volume;
            volumeCtl.GetMasterVolume(out volume);

            return volume;
        }

        public bool GetMute()
        {
            bool volume;
            volumeCtl.GetMute(out volume);

            return volume;
        }

        public void SetMute(bool volume, Guid eventContext)
        {
            volumeCtl.SetMute(volume, ref eventContext);
        }

        public void SetMute(bool volume)
        {
            SetMute(volume, ChangeGuid);
        }

        public string GetDisplayName()
        {
            string name;
            sessCtl.GetDisplayName(out name);

            return name;
        }

        public void SetDisplayName(string name, Guid changeGuid)
        {
            sessCtl.SetDisplayName(name, ref changeGuid);
        }

        public void SetDisplayName(string name)
        {
            SetDisplayName(name, ChangeGuid);
        }

        public string GetIconPath()
        {
            string path;
            sessCtl.GetIconPath(out path);

            return path;
        }

        public void SetIconPath(string path, Guid changeGuid)
        {
            sessCtl.SetIconPath(path, ref changeGuid);
        }

        public void SetIconPath(string path)
        {
            SetIconPath(path, ChangeGuid);
        }

        public Guid GetGroupingParam()
        {
            Guid groupParam;
            sessCtl.GetGroupingParam(out groupParam);

            return groupParam;
        }

        public void SetGroupingParam(Guid groupParam, Guid changeGuid)
        {
            sessCtl.SetGroupingParam(ref groupParam, ref changeGuid);
        }

        public void SetGroupingParam(Guid groupParam)
        {
            SetGroupingParam(groupParam, ChangeGuid);
        }

        public void Dispose()
        {
            meter.Dispose();
            if (callback != null)
            {
                sessCtl.UnregisterAudioSessionNotification(callback);
                callback = null;
            }
        }

        internal void FireDisplayNameChanged(string newDisplayName, Guid eventContext)
        {
            DisplayNameChangedEventHandler del = OnDisplayNameChanged;
            if (del != null)
                del(newDisplayName, eventContext);
        }

        internal void FireIconPathChanged(string newIconPath, Guid eventContext)
        {
            IconPathChangedEventHandler del = OnIconPathChanged;
            if (del != null)
                del(newIconPath, eventContext);
        }

        internal void FireVolumeChanged(float newVolume, bool newMute, Guid eventContext)
        {
            VolumeChangedEventHandler del = OnVolumeChanged;
            if (del != null)
                del(newVolume, new[] { newVolume }, newMute, eventContext);
        }

        internal void FireGroupingParamChanged(Guid newGroupingParam, Guid eventContext)
        {
            GroupingParamChangedEventHandler del = OnGroupingParamChanged;
            if (del != null)
                del(newGroupingParam, eventContext);
        }

        internal void FireStateChanged(AudioSessionState newState)
        {
            StateChangedEventHandler del = OnStateChanged;
            if (del != null)
                del(newState);
        }

        internal void FireSessionDisconnection(AudioSessionDisconnectReason disconnectReason)
        {
            SessionDisconnectionEventHandler del = OnSessionDisconnection;
            if (del != null)
                del(disconnectReason);
        }
    }
}
