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

        private int marginTop;

        public int MarginTop
        {
            get
            {
                return marginTop;
            }
            set
            {
                DispatcherHelper.CheckBeginInvokeOnUI(() => {
                    Set(() => MarginTop, ref marginTop, value);
                });
            }
        }

        private int marginBottom;

        public int MarginBottom
        {
            get
            {
                return marginBottom;
            }
            set
            {
                DispatcherHelper.CheckBeginInvokeOnUI(() => {
                    Set(() => MarginBottom, ref marginBottom, value);
                });
            }
        }

        private int marginLeft;

        public int MarginLeft
        {
            get
            {
                return marginLeft;
            }
            set
            {
                DispatcherHelper.CheckBeginInvokeOnUI(() => {
                    Set(() => MarginLeft, ref marginLeft, value);
                });
            }
        }

        private int marginRight;

        public int MarginRight
        {
            get
            {
                return marginRight;
            }
            set
            {
                DispatcherHelper.CheckBeginInvokeOnUI(() => {
                    Set(() => MarginRight, ref marginRight, value);
                });
            }
        }

        private int layoutPadding;

        public int LayoutPadding
        {
            get
            {
                return layoutPadding;
            }
            set
            {
                DispatcherHelper.CheckBeginInvokeOnUI(() => {
                    Set(() => LayoutPadding, ref layoutPadding, value);
                });
            }
        }

        private List<Filter> filters;

        public List<Filter> Filters
        {
            get
            {
                return filters;
            }
            set
            {
                DispatcherHelper.CheckBeginInvokeOnUI(() => {
                    Set(() => Filters, ref filters, value);
                });
            }
        }

        public MainViewModel()
        {
            desktopWindows = new List<DesktopWindow>();
            filters = new List<Filter>();
            padding = 0;
            layoutPadding = 5;
        }
    }
}
