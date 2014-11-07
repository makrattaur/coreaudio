using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Threading;
using CoreAudioApi;

namespace VolumeMixer
{
    public class PeakPollerThread
    {
        Thread thread;
        volatile bool running = true;
        volatile bool stopped = false;

        List<ManagedDevice> devices;

        public List<ManagedDevice> Devices
        {
            get { return devices; }
            set { devices = value; }
        }

        public event Action<Action> OnCallNeeded;

        public PeakPollerThread()
        {
            thread = new Thread(ThreadProc);
            thread.Name = "Peak Poller Thread";
        }

        public void Start()
        {
            thread.Start();
        }

        public void Stop()
        {
            if (!stopped)
            {
                running = false;
                thread.Join();
            }
        }

        void ThreadProc()
        {
            while (running)
            {
                Thread.Sleep(50);

                //continue;
                
                //bool first = true;
                foreach (var manDev in devices)
                {
                    OnCallNeeded(() =>
                    {
                        //manDev.Item1.Level = first ? 1.0f : manDev.Item2.GetMasterLevel();
                        //first = false;
                        manDev.ViewModel.Level = manDev.Device.GetMasterLevel();
                    });

                    foreach (var manSess in manDev.Sessions)
                    {
                        OnCallNeeded(() =>
                        {
                            manSess.ViewModel.Level = manSess.Session.GetMasterLevel();
                        });
                    }
                }
            }

            stopped = true;
        }
    }
}
