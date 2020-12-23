using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Threading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmethystWindows.ViewModels
{
    class MainViewModel : ViewModelBase
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

        public MainViewModel()
        {
            desktopWindows = new List<DesktopWindow>();
        }
    }
}
