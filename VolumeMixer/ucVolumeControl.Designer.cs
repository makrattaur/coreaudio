namespace VolumeMixer
{
    partial class ucVolumeControl
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ucVolumeControl));
            this.tbVolume = new System.Windows.Forms.TrackBar();
            this.lblName = new System.Windows.Forms.Label();
            this.btnAction = new System.Windows.Forms.Button();
            this.ttpSliderValue = new System.Windows.Forms.ToolTip(this.components);
            this.ilStates = new System.Windows.Forms.ImageList(this.components);
            this.pbIcon = new System.Windows.Forms.PictureBox();
            this.chkMuted = new VolumeMixer.ucImageCheckbox();
            this.guLevel = new VolumeMixer.ucGauge();
            ((System.ComponentModel.ISupportInitialize)(this.tbVolume)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbIcon)).BeginInit();
            this.SuspendLayout();
            // 
            // tbVolume
            // 
            this.tbVolume.AutoSize = false;
            this.tbVolume.Location = new System.Drawing.Point(52, 96);
            this.tbVolume.Maximum = 100;
            this.tbVolume.Name = "tbVolume";
            this.tbVolume.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.tbVolume.Size = new System.Drawing.Size(25, 136);
            this.tbVolume.TabIndex = 3;
            this.tbVolume.TickStyle = System.Windows.Forms.TickStyle.None;
            this.tbVolume.Value = 50;
            this.tbVolume.Scroll += new System.EventHandler(this.tbVolume_Scroll);
            // 
            // lblName
            // 
            this.lblName.AutoEllipsis = true;
            this.lblName.Location = new System.Drawing.Point(3, 57);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(94, 30);
            this.lblName.TabIndex = 1;
            this.lblName.Text = "label1";
            this.lblName.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnAction
            // 
            this.btnAction.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAction.Location = new System.Drawing.Point(3, 3);
            this.btnAction.Name = "btnAction";
            this.btnAction.Size = new System.Drawing.Size(32, 32);
            this.btnAction.TabIndex = 0;
            this.btnAction.Text = "IA";
            this.btnAction.UseVisualStyleBackColor = true;
            this.btnAction.Visible = false;
            // 
            // ilStates
            // 
            this.ilStates.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ilStates.ImageStream")));
            this.ilStates.TransparentColor = System.Drawing.Color.Transparent;
            this.ilStates.Images.SetKeyName(0, "spkr_norm.ico");
            this.ilStates.Images.SetKeyName(1, "spkr_muted.ico");
            // 
            // pbIcon
            // 
            this.pbIcon.Location = new System.Drawing.Point(34, 22);
            this.pbIcon.Name = "pbIcon";
            this.pbIcon.Size = new System.Drawing.Size(32, 32);
            this.pbIcon.TabIndex = 5;
            this.pbIcon.TabStop = false;
            // 
            // chkMuted
            // 
            this.chkMuted.Appearance = System.Windows.Forms.Appearance.Button;
            this.chkMuted.ImageList = this.ilStates;
            this.chkMuted.Location = new System.Drawing.Point(34, 234);
            this.chkMuted.Name = "chkMuted";
            this.chkMuted.Size = new System.Drawing.Size(32, 32);
            this.chkMuted.TabIndex = 4;
            this.chkMuted.Text = "M";
            this.chkMuted.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.chkMuted.UseVisualStyleBackColor = true;
            // 
            // guLevel
            // 
            this.guLevel.BackColor = System.Drawing.SystemColors.Control;
            this.guLevel.Location = new System.Drawing.Point(36, 104);
            this.guLevel.Name = "guLevel";
            this.guLevel.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.guLevel.Size = new System.Drawing.Size(10, 120);
            this.guLevel.TabIndex = 2;
            // 
            // ucVolumeControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.Controls.Add(this.pbIcon);
            this.Controls.Add(this.btnAction);
            this.Controls.Add(this.lblName);
            this.Controls.Add(this.chkMuted);
            this.Controls.Add(this.guLevel);
            this.Controls.Add(this.tbVolume);
            this.Name = "ucVolumeControl";
            this.Size = new System.Drawing.Size(100, 275);
            ((System.ComponentModel.ISupportInitialize)(this.tbVolume)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbIcon)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TrackBar tbVolume;
        private ucGauge guLevel;
        private ucImageCheckbox chkMuted;
        private System.Windows.Forms.Label lblName;
        private System.Windows.Forms.Button btnAction;
        private System.Windows.Forms.ToolTip ttpSliderValue;
        private System.Windows.Forms.ImageList ilStates;
        private System.Windows.Forms.PictureBox pbIcon;
    }
}
