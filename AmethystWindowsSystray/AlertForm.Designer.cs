
using System.Drawing;
using System.Windows.Forms;
using Vanara.PInvoke;

namespace AmethystWindowsSystray
{
    partial class AlertForm
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.AlertFormImage = new System.Windows.Forms.PictureBox();
            this.AlertFormLabel = new System.Windows.Forms.Label();
            this.AlertFormTimer = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.AlertFormImage)).BeginInit();
            this.SuspendLayout();
            // 
            // AlertFormImage
            // 
            this.AlertFormImage.Image = global::AmethystWindowsSystray.Properties.Resources.Square44x44Logo1;
            this.AlertFormImage.InitialImage = global::AmethystWindowsSystray.Properties.Resources.Square44x44Logo_scale_200;
            this.AlertFormImage.Location = new System.Drawing.Point(12, 12);
            this.AlertFormImage.Name = "AlertFormImage";
            this.AlertFormImage.Size = new System.Drawing.Size(91, 90);
            this.AlertFormImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.AlertFormImage.TabIndex = 0;
            this.AlertFormImage.TabStop = false;
            // 
            // AlertFormLabel
            // 
            this.AlertFormLabel.AutoSize = true;
            this.AlertFormLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 13.875F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.AlertFormLabel.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.AlertFormLabel.Location = new System.Drawing.Point(109, 31);
            this.AlertFormLabel.Name = "AlertFormLabel";
            this.AlertFormLabel.Size = new System.Drawing.Size(118, 42);
            this.AlertFormLabel.TabIndex = 1;
            this.AlertFormLabel.Text = "label1";
            // 
            // AlertFormTimer
            // 
            this.AlertFormTimer.Tick += new System.EventHandler(this.AlertFormTimer_Tick);
            // 
            // AlertForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(324, 114);
            this.Controls.Add(this.AlertFormLabel);
            this.Controls.Add(this.AlertFormImage);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "AlertForm";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.AlertFormImage)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox AlertFormImage;
        private System.Windows.Forms.Label AlertFormLabel;
        private System.Windows.Forms.Timer AlertFormTimer;

        private enmAction action;
        private int x, y;

        public void showAlert(string msg, int x, int width, int y, int height)
        {
            this.Opacity = 0.0;
            this.StartPosition = FormStartPosition.Manual;
            string fname;

            for (int i = 1; i < 10; i++)
            {
                fname = "alert" + i.ToString();
                AlertForm frm = (AlertForm)Application.OpenForms[fname];

                if (frm == null)
                {
                    this.Name = fname;
                    this.x = x + width / 2 - this.Width / 2;
                    this.y = y + height / 2 - this.Height / 2;
                    this.Location = new Point(this.x, this.y);
                    break;
                }

            }

            this.BackColor = Color.SlateGray;
            this.AlertFormLabel.Text = msg;

            this.Show();
            User32.SetForegroundWindow(this.Handle);
            this.action = enmAction.start;
            this.AlertFormTimer.Interval = 1;
            this.AlertFormTimer.Start();
        }
    }
}