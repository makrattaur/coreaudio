using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CoreAudioApi.COMInterop;

namespace CoreAudioApi
{
    public delegate void DeviceStateChangeEventHandler(string deviceId, DeviceState newState);
    public delegate void DeviceAddedEventHandler(string deviceId);
    public delegate void DeviceRemovedEventHandler(string deviceId);
    public delegate void DefaultDeviceChangedEventHandler(EDataFlow flow, ERole role, string deviceId);

    internal class CIMMNotificationClientCallback : IMMNotificationClient
    {
        MMDeviceEnumerator parent;

        public CIMMNotificationClientCallback(MMDeviceEnumerator parent)
        {
            this.parent = parent;
        }

        public void OnDeviceStateChanged(string pwstrDeviceId, uint dwNewState)
        {
            parent.FireDeviceStateChange(pwstrDeviceId, (DeviceState)dwNewState);
        }

        public void OnDeviceAdded(string pwstrDeviceId)
        {
            parent.FireDeviceAdded(pwstrDeviceId);
        }

        public void OnDeviceRemoved(string pwstrDeviceId)
        {
            parent.FireDeviceRemoved(pwstrDeviceId);
        }

        public void OnDefaultDeviceChanged(EDataFlow flow, ERole role, string pwstrDefaultDeviceId)
        {
            parent.FireDefaultDeviceChanged(flow, role, pwstrDefaultDeviceId);
        }

        public void OnPropertyValueChanged(string pwstrDeviceId, PROPERTYKEY key)
        {
            
        }
    }

    public class MMDeviceEnumerator : IDisposable
    {
        IMMDeviceEnumerator enumerator;
        CIMMNotificationClientCallback callback;

        public MMDeviceEnumerator()
        {
            //enumerator = (IMMDeviceEnumerator)(new COMInterop.MMDeviceEnumerator());
            Type comType = Type.GetTypeFromCLSID(new Guid("BCDE0395-E52F-467C-8E3D-C4579291692E"));
            enumerator = (IMMDeviceEnumerator)Activator.CreateInstance(comType);
            callback = new CIMMNotificationClientCallback(this);
            enumerator.RegisterEndpointNotificationCallback(callback);
        }

        ~MMDeviceEnumerator()
        {
            Dispose();
        }

        public event DeviceStateChangeEventHandler OnDeviceStateChanged;
        public event DeviceAddedEventHandler OnDeviceAdded;
        public event DeviceRemovedEventHandler OnDeviceRemoved;
        public event DefaultDeviceChangedEventHandler OnDefaultDeviceChanged;

        public IEnumerable<MMDevice> EnumAudioEndpoints(EDataFlow dataFlow, DeviceState stateMask)
        {
            List<MMDevice> devices = new List<MMDevice>();

            IMMDeviceCollection collection;
            enumerator.EnumAudioEndpoints(dataFlow, stateMask, out collection);
            uint count;
            collection.GetCount(out count);
            for (uint i = 0; i < count; i++)
            {
                IMMDevice device;
                collection.Item(i, out device);
                devices.Add(new MMDevice(device));
            }

            return devices;
        }

        public MMDevice GetDefaultAudioEndpoint(EDataFlow dataFlow, ERole role)
        {
            IMMDevice device;
            enumerator.GetDefaultAudioEndpoint(dataFlow, role, out device);

            return new MMDevice(device);
        }

        public MMDevice GetDevice(string id)
        {
            IMMDevice device;
            enumerator.GetDevice(id, out device);

            return new MMDevice(device);
        }

        internal void FireDeviceStateChange(string deviceId, DeviceState newState)
        {
            DeviceStateChangeEventHandler del = OnDeviceStateChanged;
            if (del != null)
                del(deviceId, newState);
        }

        internal void FireDeviceAdded(string deviceId)
        {
            DeviceAddedEventHandler del = OnDeviceAdded;
            if (del != null)
                del(deviceId);
        }

        internal void FireDeviceRemoved(string deviceId)
        {
            DeviceRemovedEventHandler del = OnDeviceRemoved;
            if (del != null)
                del(deviceId);
        }

        internal void FireDefaultDeviceChanged(EDataFlow flow, ERole role, string deviceId)
        {
            DefaultDeviceChangedEventHandler del = OnDefaultDeviceChanged;
            if (del != null)
                del(flow, role, deviceId);
        }
    
        public void Dispose()
        {
            if (callback != null)
            {
                enumerator.UnregisterEndpointNotificationCallback(callback);
                callback = null;
            }
        }
    }
}
