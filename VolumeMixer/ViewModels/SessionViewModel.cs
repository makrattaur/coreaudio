using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Drawing;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace VolumeMixer.ViewModels
{
    public class SessionViewModel : INotifyPropertyChanged
    {
        private string name;
        private float level;
        private float volume;
        private bool muted;
        private Bitmap icon;

        public string Name
        {
            get { return name; }
            set { CheckAndFireChange(ref value, ref name); }
        }

        public float Level
        {
            get { return level; }
            set { CheckAndFireChange(ref value, ref level); }
        }

        public float Volume
        {
            get { return volume; }
            set { CheckAndFireChange(ref value, ref volume); }
        }

        public bool Muted
        {
            get { return muted; }
            set { CheckAndFireChange(ref value, ref muted); }
        }

        public Bitmap Icon
        {
            get { return icon; }
            set { CheckAndFireChange(ref value, ref icon); }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void CheckAndFireChange<T>(ref T newValue, ref T field, [CallerMemberName] string fieldName = null)
        {
            if (Comparer<T>.Default.Compare(newValue, field) != 0)
            {
                field = newValue;

                PropertyChangedEventHandler del = PropertyChanged;
                if (del != null)
                {
                    del(this, new PropertyChangedEventArgs(fieldName));
                }
            }
        }
    }
}
