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
    public partial class ucVolumeControlArray : UserControl
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

        private CurrencyManager dataManager;
        private ListChangedEventHandler listChanged;
        private BindingSource parentBinding;
        private EventHandler currentChanged;
        private EventHandler dataSourceInitialized;

        bool waitingForInit = false;

        private void tryDataBinding()
        {
            if (this.DataSource == null || base.BindingContext == null)
                return;

            ISupportInitializeNotification isin = (DataSource as ISupportInitializeNotification);

            if (isin != null)
            {
                if (waitingForInit)
                {
                    isin.Initialized -= dataSourceInitialized;
                    waitingForInit = false;
                }
                else if (!isin.IsInitialized)
                {
                    waitingForInit = true;
                    isin.Initialized += dataSourceInitialized;
                    return;
                }
            }

            BindingSource bs;
            try
            {
                bs = (BindingSource)this.dataSource;
            }
            catch (InvalidCastException)
            {
                return;
            }

            CurrencyManager cm;
            try
            {
                var bindingBase = base.BindingContext[this.DataSource, this.DataMember];
                cm = (CurrencyManager)bindingBase;
            }
            catch (ArgumentException)
            {
                return;
            }

            if (parentBinding != bs)
            {
                if (parentBinding != null)
                {
                    parentBinding.CurrentChanged -= currentChanged;
                }
                parentBinding = bs;

                ClearData();
                if (parentBinding != null)
                {
                    parentBinding.CurrentChanged += currentChanged;
                    if (dataManager != null)
                    {
                        UpdateData();
                    }
                }
            }

            if (this.dataManager != cm)
            {
                if (this.dataManager != null)
                {
                    dataManager.ListChanged -= listChanged;
                }
                this.dataManager = cm;

                ClearData();
                if (this.dataManager != null)
                {
                    dataManager.ListChanged += listChanged;
                    UpdateData();
                }
            }
        }

        void isin_Initialized(object sender, EventArgs e)
        {
            ISupportInitializeNotification isin = DataSource as ISupportInitializeNotification;
            if (isin != null)
            {
                isin.Initialized -= dataSourceInitialized;
            }

            waitingForInit = false;
            tryDataBinding();
        }

        bool updateOnNextReset = false;

        void parentBinding_CurrentChanged(object sender, EventArgs e)
        {
            updateOnNextReset = true;
        }

        void dataManager_ListChanged(object sender, ListChangedEventArgs e)
        {
            if (e.ListChangedType == ListChangedType.ItemAdded)
            {
                CreateSubControl(dataManager.List[e.NewIndex]);
            }
            else if (e.ListChangedType == ListChangedType.ItemDeleted)
            {
                //var obj = dataManager.List[e.NewIndex];
                //var ctl = tlpVolCtlArray.Controls.Cast<ucVolumeControl>().Where(vc => vc.DataSource == obj).SingleOrDefault();
                ucVolumeControl ctl = null;
                if (e.NewIndex >= 0 && e.NewIndex < tlpVolCtlArray.Controls.Count)
                {
                    ctl = tlpVolCtlArray.Controls[e.NewIndex] as ucVolumeControl;
                }
                if (ctl != null)
                {
                    tlpVolCtlArray.Controls.Remove(ctl);
                    ctl.DataSource = null;
                    ForceScrollReadjust();
                }
            }

            if (updateOnNextReset && e.ListChangedType == ListChangedType.Reset)
            {
                ClearData();
                UpdateData();

                updateOnNextReset = false;
            }
        }

        void ClearData()
        {
            foreach (ucVolumeControl ctl in tlpVolCtlArray.Controls)
            {
                ctl.DataSource = null;
            }

            tlpVolCtlArray.Controls.Clear();
        }

        void UpdateData()
        {
            foreach (var item in dataManager.List)
            {
                CreateSubControl(item);
            }

            ForceScrollReadjust();
        }

        void ForceScrollReadjust()
        {
            tlpVolCtlArray.AutoScroll = false;
            tlpVolCtlArray.AutoScroll = true;
        }

        void CreateSubControl(object dataSource)
        {
            var subCtrl = new ucVolumeControl();
            subCtrl.DataSource = dataSource;
            subCtrl.Margin = new Padding(3, 0, 3, 0);
            tlpVolCtlArray.Controls.Add(subCtrl);
        }

        public ucVolumeControlArray()
        {
            InitializeComponent();

            listChanged = new ListChangedEventHandler(dataManager_ListChanged);
            currentChanged = new EventHandler(parentBinding_CurrentChanged);
            dataSourceInitialized = new EventHandler(isin_Initialized);

            tlpVolCtlArray.Padding = new Padding(0, 0, 0, SystemInformation.HorizontalScrollBarHeight);
        }
    }
}
