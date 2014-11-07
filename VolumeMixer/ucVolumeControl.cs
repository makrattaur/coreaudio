using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VolumeMixer
{
    [ComplexBindingProperties("DataSource", "DataMember")]
    public partial class ucVolumeControl : UserControl
    {
        private object dataSource;

        [TypeConverter("System.Windows.Forms.Design.DataSourceConverter, System.Design")]
        [Category("Data")]
        [DefaultValue(null)]
        public object DataSource
        {
            get
            {
                return this.dataSource;
            }
            set
            {
                if (this.dataSource != value)
                {
                    this.dataSource = value;
                    tryDataBinding();
                }
            }
        }

        private string dataMember;

        [Category("Data")]
        [Editor("System.Windows.Forms.Design.DataMemberListEditor, System.Design", "System.Drawing.Design.UITypeEditor, System.Drawing")]
        [DefaultValue("")]
        public string DataMember
        {
            get
            {
                return this.dataMember;
            }
            set
            {
                if (this.dataMember != value)
                {
                    this.dataMember = value;
                    tryDataBinding();
                }
            }
        }

        private BindingManagerBase dataManager;
        private EventHandler currentItemChanged;

        private void tryDataBinding()
        {
            if (this.DataSource == null || base.BindingContext == null)
                return;

            BindingManagerBase cm;
            try
            {
                cm = base.BindingContext[this.DataSource, this.DataMember];
            }
            catch (ArgumentException)
            {
                return;
            }

            if (this.dataManager != cm)
            {
                if (this.dataManager != null)
                {
                    dataManager.CurrentChanged -= currentItemChanged;
                }
                this.dataManager = cm;
                if (this.dataManager != null)
                {
                    dataManager.CurrentChanged += currentItemChanged;
                    if (dataManager.Count == 1 && dataManager is PropertyManager)
                    {
                        bm_CurrentChanged(this, EventArgs.Empty);
                    }
                }
            }
        }

        void bm_CurrentChanged(object sender, EventArgs e)
        {
            ClearAndSetBinding(guLevel, new Binding("NormalizedFloatValue", dataManager.Current, "Level")
            {
                DataSourceUpdateMode = DataSourceUpdateMode.Never
            });

            var volumeBinding = new Binding("Value", dataManager.Current, "Volume");
            volumeBinding.DataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged;
            volumeBinding.Format += volBindFormat;
            volumeBinding.Parse += volBindParse;
            ClearAndSetBinding(tbVolume, volumeBinding);

            ClearAndSetBinding(lblName, new Binding("Text", dataManager.Current, "Name")
            {
                DataSourceUpdateMode = DataSourceUpdateMode.Never
            });

            ClearAndSetBinding(chkMuted, new Binding("Checked", dataManager.Current, "Muted")
            {
                DataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged
            });

            ClearAndSetBinding(pbIcon, new Binding("Image", dataManager.Current, "Icon")
            {
                DataSourceUpdateMode = DataSourceUpdateMode.Never
            });
        }

        static void ClearAndSetBinding(Control ctl, Binding binding)
        {
            ctl.DataBindings.Clear();
            ctl.DataBindings.Add(binding);
        }

        void volumeBinding_Parse(object sender, ConvertEventArgs e)
        {
            e.Value = ((int)e.Value) / 100.0f;
        }

        void volumeBinding_Format(object sender, ConvertEventArgs e)
        {
            e.Value = (int)Math.Round(((float)e.Value) * 100.0f);
        }

        ConvertEventHandler volBindParse;
        ConvertEventHandler volBindFormat;

        public ucVolumeControl()
        {
            InitializeComponent();
            currentItemChanged = new EventHandler(bm_CurrentChanged);

            volBindParse = new ConvertEventHandler(volumeBinding_Parse);
            volBindFormat = new ConvertEventHandler(volumeBinding_Format);

            //int newLong = NativeMethods.GetWindowLong(tbVolume.Handle, NativeMethods.GWL_STYLE) | NativeMethods.TBS_TOOLTIPS;
            //NativeMethods.SetWindowLong(tbVolume.Handle, NativeMethods.GWL_STYLE, newLong);
        }

        protected override Size DefaultSize
        {
            get
            {
                return new Size(100, 275);
            }
        }

        private void tbVolume_Scroll(object sender, EventArgs e)
        {
            ttpSliderValue.SetToolTip(tbVolume, tbVolume.Value.ToString());
        }
    }
}
