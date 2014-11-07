using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VolumeMixer
{
    public partial class frmVolumeMixer : Form
    {
        ObjectManager objMan = new ObjectManager();
        BindingList<ViewModels.DeviceViewModel> deviceList = new BindingList<ViewModels.DeviceViewModel>();

        public frmVolumeMixer()
        {
            InitializeComponent();
        }

        private void frmVolumeMixer_Load(object sender, EventArgs e)
        {
            deviceBindingSource.DataSource = deviceList;
            objMan.TargetDeviceList = deviceList;
            objMan.OnCallNeeded += objMan_OnCallNeeded;
            objMan.Init();
        }

        void objMan_OnCallNeeded(Action obj)
        {
            this.BeginInvoke(obj);
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }

            base.Dispose(disposing);
        }

        private void frmVolumeMixer_FormClosing(object sender, FormClosingEventArgs e)
        {
            tmrUpdate.Stop();
            objMan.DeInit();
        }

        private void tmrUpdate_Tick(object sender, EventArgs e)
        {
            objMan.UpdateState();
        }

        private void tmrUpdateSecond_Tick(object sender, EventArgs e)
        {
            objMan.UpdateStatePerSecond();
        }

    }
}
