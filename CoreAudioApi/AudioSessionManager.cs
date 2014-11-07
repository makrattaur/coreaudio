using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CoreAudioApi.COMInterop.Session;

namespace CoreAudioApi
{
    public delegate void SessionCreatedEventHandler(AudioSession newSession);

    internal class CIAudioSessionNotificationCallback : IAudioSessionNotification
    {
        AudioSessionManager parent;

        public CIAudioSessionNotificationCallback(AudioSessionManager parent)
        {
            this.parent = parent;
        }

        public void OnSessionCreated(IAudioSessionControl NewSession)
        {
            parent.FireSessionCreated(NewSession);
        }
    }

    public class AudioSessionManager : IDisposable
    {
        IAudioSessionManager2 sessMgr;
        CIAudioSessionNotificationCallback callback;

        internal AudioSessionManager(IAudioSessionManager2 sessMgr)
        {
            this.sessMgr = sessMgr;

            callback = new CIAudioSessionNotificationCallback(this);
            sessMgr.RegisterSessionNotification(callback);
        }

        ~AudioSessionManager()
        {
            Dispose();
        }

        public event SessionCreatedEventHandler OnSessionCreated;

        public IEnumerable<AudioSession> GetSessions()
        {
            List<AudioSession> sessions = new List<AudioSession>();

            IAudioSessionEnumerator enumerator;
            sessMgr.GetSessionEnumerator(out enumerator);
            int count;
            enumerator.GetCount(out count);
            for (int i = 0; i < count; i++)
            {
                IAudioSessionControl sessCtl;
                enumerator.GetSession(i, out sessCtl);
                sessions.Add(new AudioSession(sessCtl));
            }

            return sessions;
        }

        public void Dispose()
        {
            if (callback != null)
            {
                sessMgr.UnregisterSessionNotification(callback);
                callback = null;
            }
        }

        internal void FireSessionCreated(IAudioSessionControl sessCtl)
        {
            SessionCreatedEventHandler del = OnSessionCreated;
            if (del != null)
                del(new AudioSession(sessCtl));
        }
    }
}
