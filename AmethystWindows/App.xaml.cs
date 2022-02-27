using AmethystWindows.Settings;
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
        internal static DesktopWindowsManager.Hooks hooks = null;
        private static bool firstActivation = true;

        public App()
        {
            InitializeComponent();
        }

        private void App_Activated(object sender, EventArgs e)
        {
            if (firstActivation) {
                DWM = new DesktopWindowsManager.DesktopWindowsManager();
                Debug.WriteLine($"getting settings");
                Debug.WriteLine($"setting hooks");
                hooks = new DesktopWindowsManager.Hooks(DWM);
                hooks.setWindowsHook();
                hooks.setKeyboardHook(new WindowInteropHelper(App.Current.MainWindow).Handle, DWM.mainWindowViewModel.Hotkeys);
                Debug.WriteLine($"getting windows");
                DWM.CollectWindows();

                Debug.WriteLine($"setting virtual desktop change listener");
                VirtualDesktop.CurrentChanged += VirtualDesktop_CurrentChanged;

                Debug.WriteLine($"setting virtual desktops");
                int virtualDesktopsExisting = VirtualDesktop.GetDesktops().Count();
                int virtualDesktopsToCreate = MySettings.Instance.VirtualDesktops - virtualDesktopsExisting;

                if (virtualDesktopsExisting < MySettings.Instance.VirtualDesktops)
                {
                    for (int i = 1; i <= virtualDesktopsToCreate; i++)
                    {
                        VirtualDesktop.Create();
                    }
                }

                firstActivation = false;
            }
        }

        private void VirtualDesktop_CurrentChanged(object sender, VirtualDesktopChangedEventArgs e)
        {
            DWM.CollectWindows();
            DWM.Draw();
        }
    }
}
