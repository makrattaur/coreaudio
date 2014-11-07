namespace VolumeMixer
{
    partial class frmVolumeMixer
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.Label lblDeviceName;
            this.deviceBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.ucVolumeControl1 = new VolumeMixer.ucVolumeControl();
            this.ucVolumeControlArray1 = new VolumeMixer.ucVolumeControlArray();
            this.tmrUpdate = new System.Windows.Forms.Timer(this.components);
            this.tmrUpdateSecond = new System.Windows.Forms.Timer(this.components);
            lblDeviceName = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.deviceBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // lblDeviceName
            // 
            lblDeviceName.AutoSize = true;
            lblDeviceName.Location = new System.Drawing.Point(13, 15);
            lblDeviceName.Name = "lblDeviceName";
            lblDeviceName.Size = new System.Drawing.Size(44, 13);
            lblDeviceName.TabIndex = 7;
            lblDeviceName.Text = "Device:";
            // 
            // deviceBindingSource
            // 
            this.deviceBindingSource.AllowNew = false;
            this.deviceBindingSource.DataSource = typeof(VolumeMixer.ViewModels.DeviceViewModel);
            // 
            // comboBox1
            // 
            this.comboBox1.DataSource = this.deviceBindingSource;
            this.comboBox1.DisplayMember = "FullName";
            this.comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(63, 12);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(302, 21);
            this.comboBox1.TabIndex = 11;
            // 
            // ucVolumeControl1
            // 
            this.ucVolumeControl1.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.ucVolumeControl1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.ucVolumeControl1.DataMember = null;
            this.ucVolumeControl1.DataSource = this.deviceBindingSource;
            this.ucVolumeControl1.Location = new System.Drawing.Point(12, 42);
            this.ucVolumeControl1.Name = "ucVolumeControl1";
            this.ucVolumeControl1.Size = new System.Drawing.Size(100, 300);
            this.ucVolumeControl1.TabIndex = 15;
            // 
            // ucVolumeControlArray1
            // 
            this.ucVolumeControlArray1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.ucVolumeControlArray1.DataMember = "Sessions";
            this.ucVolumeControlArray1.DataSource = this.deviceBindingSource;
            this.ucVolumeControlArray1.Location = new System.Drawing.Point(115, 42);
            this.ucVolumeControlArray1.Margin = new System.Windows.Forms.Padding(0);
            this.ucVolumeControlArray1.Name = "ucVolumeControlArray1";
            this.ucVolumeControlArray1.Size = new System.Drawing.Size(479, 300);
            this.ucVolumeControlArray1.TabIndex = 17;
            // 
            // tmrUpdate
            // 
            this.tmrUpdate.Enabled = true;
            this.tmrUpdate.Interval = 50;
            this.tmrUpdate.Tick += new System.EventHandler(this.tmrUpdate_Tick);
            // 
            // tmrUpdateSecond
            // 
            this.tmrUpdateSecond.Enabled = true;
            this.tmrUpdateSecond.Interval = 1000;
            this.tmrUpdateSecond.Tick += new System.EventHandler(this.tmrUpdateSecond_Tick);
            // 
            // frmVolumeMixer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.ClientSize = new System.Drawing.Size(606, 354);
            this.Controls.Add(this.ucVolumeControlArray1);
            this.Controls.Add(this.ucVolumeControl1);
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(lblDeviceName);
            this.Name = "frmVolumeMixer";
            this.Text = "Form1";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmVolumeMixer_FormClosing);
            this.Load += new System.EventHandler(this.frmVolumeMixer_Load);
            ((System.ComponentModel.ISupportInitialize)(this.deviceBindingSource)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.BindingSource deviceBindingSource;
        private System.Windows.Forms.ComboBox comboBox1;
        private ucVolumeControl ucVolumeControl1;
        private ucVolumeControlArray ucVolumeControlArray1;
        private System.Windows.Forms.Timer tmrUpdate;
        private System.Windows.Forms.Timer tmrUpdateSecond;

    }
}

