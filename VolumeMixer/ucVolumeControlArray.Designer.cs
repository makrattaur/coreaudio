namespace VolumeMixer
{
    partial class ucVolumeControlArray
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
            this.tlpVolCtlArray = new System.Windows.Forms.TableLayoutPanel();
            this.SuspendLayout();
            // 
            // tlpVolCtlArray
            // 
            this.tlpVolCtlArray.AutoScroll = true;
            this.tlpVolCtlArray.AutoSize = true;
            this.tlpVolCtlArray.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tlpVolCtlArray.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.tlpVolCtlArray.ColumnCount = 1;
            this.tlpVolCtlArray.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tlpVolCtlArray.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpVolCtlArray.GrowStyle = System.Windows.Forms.TableLayoutPanelGrowStyle.AddColumns;
            this.tlpVolCtlArray.Location = new System.Drawing.Point(0, 0);
            this.tlpVolCtlArray.Margin = new System.Windows.Forms.Padding(0);
            this.tlpVolCtlArray.Name = "tlpVolCtlArray";
            this.tlpVolCtlArray.RowCount = 1;
            this.tlpVolCtlArray.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpVolCtlArray.Size = new System.Drawing.Size(231, 285);
            this.tlpVolCtlArray.TabIndex = 0;
            // 
            // ucVolumeControlArray
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tlpVolCtlArray);
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "ucVolumeControlArray";
            this.Size = new System.Drawing.Size(231, 285);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tlpVolCtlArray;

    }
}
