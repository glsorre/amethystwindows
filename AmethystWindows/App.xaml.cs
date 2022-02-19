using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Interop;
using WindowsDesktop;

namespace AmethystWindows
{

    public partial class App : Application
    {
        internal static DesktopWindowsManager.DesktopWindowsManager DWM = null;
        private DesktopWindowsManager.Hooks hooks = null;
        private bool firtsActivation = true;

        public App()
        {
            InitializeComponent();

        }

        private void App_Activated(object sender, EventArgs e)
        {
            if (firtsActivation) { 
                DWM = new DesktopWindowsManager.DesktopWindowsManager();
                Debug.WriteLine($"getting settings");
                DWM.LoadLayouts();
                DWM.LoadFactors();
                Debug.WriteLine($"setting hooks");
                hooks = new DesktopWindowsManager.Hooks(DWM);
                hooks.setWindowsHook();
                hooks.setKeyboardHook(new WindowInteropHelper(MainWindow).Handle);
                Debug.WriteLine($"getting windows");
                DWM.CollectWindows();
                Debug.WriteLine($"drawing");
                DWM.Draw();

                Debug.WriteLine($"setting virtual desktop change listener");
                VirtualDesktop.CurrentChanged += VirtualDesktop_CurrentChanged;

                Debug.WriteLine($"setting virtual desktops");
                int virtualDesktopsExisting = VirtualDesktop.GetDesktops().Count();
                int virtualDesktopsToCreate = Settings.MySettings.Instance.VirtualDesktops - virtualDesktopsExisting;

                if (virtualDesktopsExisting < Settings.MySettings.Instance.VirtualDesktops)
                {
                    for (int i = 1; i <= virtualDesktopsToCreate; i++)
                    {
                        VirtualDesktop.Create();
                    }
                }

                firtsActivation = false;
            }
        }

        private void VirtualDesktop_CurrentChanged(object sender, VirtualDesktopChangedEventArgs e)
        {
            DWM.CollectWindows();
            DWM.Draw();
        }
    }
}
