
using System;
using System.Windows.Forms;
using Vanara.PInvoke;

namespace AmethystWindowsSystray
{
    partial class TransparentForm : Form
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        public event EventHandler AmethystSysTrayReconnect;
        public event EventHandler<int> AmethystSystrayHotKey;

        protected override void SetVisibleCore(bool value)
        {
            if (!this.IsHandleCreated)
            {
                CreateHandle();
            }
            value = false;
            base.SetVisibleCore(value);
        }

        protected override void WndProc(ref Message m)
        {
            uint messageReconnect = User32.RegisterWindowMessage("AMETHYSTSYSTRAY_RECONNECT");

            if (m.Msg == messageReconnect)
            {
                // Handle custom message
                AmethystSysTrayReconnect.Invoke(this, null);
            }

            if (m.Msg == (uint)User32.WindowMessage.WM_HOTKEY)
            {
                if (m.WParam.ToInt32() == 0x20)
                {
                    AmethystSystrayHotKey.Invoke(this, 0x20);
                }
                if (m.WParam.ToInt32() == 0x0D)
                {
                    AmethystSystrayHotKey.Invoke(this, 0x0D);
                }
                if (m.WParam.ToInt32() == 0x48)
                {
                    AmethystSystrayHotKey.Invoke(this, 0x48);
                }
                if (m.WParam.ToInt32() == 0x4A)
                {
                    AmethystSystrayHotKey.Invoke(this, 0x4A);
                }
                if (m.WParam.ToInt32() == 0x4B)
                {
                    AmethystSystrayHotKey.Invoke(this, 0x4B);
                }
                if (m.WParam.ToInt32() == 0x4C)
                {
                    AmethystSystrayHotKey.Invoke(this, 0x4C);
                }
                if (m.WParam.ToInt32() == 0x0)
                {
                    AmethystSystrayHotKey.Invoke(this, 0x0);
                }
                if (m.WParam.ToInt32() == 0x1)
                {
                    AmethystSystrayHotKey.Invoke(this, 0x1);
                }
            }

            base.WndProc(ref m);
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Text = "Form1";
        }

        #endregion
    }
}