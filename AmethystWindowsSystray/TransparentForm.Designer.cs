
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
                if (m.WParam.ToInt32() == 0x11)
                {
                    AmethystSystrayHotKey.Invoke(this, 0x11);
                }
                if (m.WParam.ToInt32() == 0x12)
                {
                    AmethystSystrayHotKey.Invoke(this, 0x12);
                }
                if (m.WParam.ToInt32() == 0x13)
                {
                    AmethystSystrayHotKey.Invoke(this, 0x13);
                }
                if (m.WParam.ToInt32() == 0x14)
                {
                    AmethystSystrayHotKey.Invoke(this, 0x14);
                }
                if (m.WParam.ToInt32() == 0x15)
                {
                    AmethystSystrayHotKey.Invoke(this, 0x15);
                }
                if (m.WParam.ToInt32() == 0x16)
                {
                    AmethystSystrayHotKey.Invoke(this, 0x16);
                }
                if (m.WParam.ToInt32() == 0x17)
                {
                    AmethystSystrayHotKey.Invoke(this, 0x17);
                }
                if (m.WParam.ToInt32() == 0x18)
                {
                    AmethystSystrayHotKey.Invoke(this, 0x18);
                }
                if (m.WParam.ToInt32() == 0x19)
                {
                    AmethystSystrayHotKey.Invoke(this, 0x19);
                }
                if (m.WParam.ToInt32() == 0x21)
                {
                    AmethystSystrayHotKey.Invoke(this, 0x21);
                }
                if (m.WParam.ToInt32() == 0x22)
                {
                    AmethystSystrayHotKey.Invoke(this, 0x22);
                }
                if (m.WParam.ToInt32() == 0x23)
                {
                    AmethystSystrayHotKey.Invoke(this, 0x23);
                }
                if (m.WParam.ToInt32() == 0x24)
                {
                    AmethystSystrayHotKey.Invoke(this, 0x24);
                }
                if (m.WParam.ToInt32() == 0x25)
                {
                    AmethystSystrayHotKey.Invoke(this, 0x25);
                }
                if (m.WParam.ToInt32() == 0x26)
                {
                    AmethystSystrayHotKey.Invoke(this, 0x26);
                }
                if (m.WParam.ToInt32() == 0x27)
                {
                    AmethystSystrayHotKey.Invoke(this, 0x27);
                }
                if (m.WParam.ToInt32() == 0x31)
                {
                    AmethystSystrayHotKey.Invoke(this, 0x31);
                }
                if (m.WParam.ToInt32() == 0x1)
                {
                    AmethystSystrayHotKey.Invoke(this, 0x1);
                }
                if (m.WParam.ToInt32() == 0x2)
                {
                    AmethystSystrayHotKey.Invoke(this, 0x2);
                }
                if (m.WParam.ToInt32() == 0x3)
                {
                    AmethystSystrayHotKey.Invoke(this, 0x3);
                }
                if (m.WParam.ToInt32() == 0x4)
                {
                    AmethystSystrayHotKey.Invoke(this, 0x4);
                }
                if (m.WParam.ToInt32() == 0x5)
                {
                    AmethystSystrayHotKey.Invoke(this, 0x5);
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