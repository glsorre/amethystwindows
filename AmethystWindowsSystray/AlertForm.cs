﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AmethystWindowsSystray
{
    public partial class AlertForm : Form
    {
        public AlertForm()
        {
            InitializeComponent();
        }

        public enum enmAction
        {
            wait,
            start,
            close
        }

        private void AlertFormTimer_Tick(object sender, EventArgs e)
        {
            switch (this.action)
            {
                case enmAction.wait:
                    AlertFormTimer.Interval = 1500;
                    action = enmAction.close;
                    break;
                case enmAction.start:
                    AlertFormTimer.Interval = 1;
                    this.Opacity += 0.1;
                    if (this.Opacity == 1.0)
                    {
                        action = enmAction.wait;
                    }
                    break;
                case enmAction.close:
                    AlertFormTimer.Interval = 1;
                    this.Opacity -= 0.1;
                    if (base.Opacity == 0.0)
                    {
                        base.Close();
                    }
                    break;
            }
        }
    }
}
