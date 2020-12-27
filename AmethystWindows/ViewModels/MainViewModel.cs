using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Threading;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmethystWindows.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private List<DesktopWindow> desktopWindows;

        public List<DesktopWindow> DesktopWindows
        {
            get
            {
                return desktopWindows;
            }
            set
            {
                DispatcherHelper.CheckBeginInvokeOnUI(() => {
                    Set(() => DesktopWindows, ref desktopWindows, value);
                });
            }
        }

        private int padding;

        public int Padding
        {
            get
            {
                return padding;
            }
            set
            {
                DispatcherHelper.CheckBeginInvokeOnUI(() => {
                    Set(() => Padding, ref padding, value);
                });
            }
        }

        public MainViewModel()
        {
            desktopWindows = new List<DesktopWindow>();
            padding = 5;
        }
    }
}
