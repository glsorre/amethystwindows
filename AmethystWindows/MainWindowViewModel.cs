using AmethystWindows.DesktopWindowsManager;
using AmethystWindows.Settings;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using WindowsDesktop;

namespace AmethystWindows
{
    public class ViewModelDesktopWindow
    {
        public string Window { get; set; }
        public string AppName { get; set; }
        public string ClassName { get; set; }
        public string VirtualDesktop { get; set; }
        public string Monitor { get; set; }

        public ViewModelDesktopWindow(string window, string appName, string className, string virtualDesktop, string monitor)
        {
            Window = window;
            AppName = appName;
            ClassName = className;
            VirtualDesktop = virtualDesktop;
            Monitor = monitor;
        }
    }

    public class ViewModelDesktopWindowConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            ViewModelDesktopWindow selectedWindow = value as ViewModelDesktopWindow;

            if (selectedWindow != null)
            {
                return true;
            }

            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    class ConfigurableFiltersEqualityComparer : IEqualityComparer<List<Pair<string, string>>>
    {
        public bool Equals(List<Pair<string, string>> cF1, List<Pair<string, string>> cF2)
        {
            if (cF1 == null && cF2 == null)
                return true;
            else if (cF1 == null || cF2 == null)
                return false;
            else if (cF1.Count == cF2.Count)
                return true;
            else
                return false;
        }

        public int GetHashCode(List<Pair<string, string>> cFx)
        {
            return cFx.GetHashCode();
        }
    }

    public class MainWindowViewModel : ObservableRecipient
    {
        private NotifyIconWrapper.NotifyRequestRecord? _notifyRequest;
        private bool _showInTaskbar;
        private WindowState _windowState;
        private IEnumerable<ViewModelDesktopWindow> _windows;
        private ViewModelDesktopWindow _selectedWindow;

        private int _layoutPadding;
        private int _padding;
        private int _marginTop;
        private int _marginRight;
        private int _marginBottom;
        private int _marginLeft;

        private bool _disabled;

        private List<Pair<string, string>> _configurableFilters;
        private Pair<string, string> _selectedConfigurableFilter;

        public MainWindowViewModel()
        {
            MySettings.Load();
            _padding = MySettings.Instance.Padding;
            _marginTop = MySettings.Instance.MarginTop;
            _marginBottom = MySettings.Instance.MarginBottom;
            _marginLeft = MySettings.Instance.MarginLeft;
            _marginRight = MySettings.Instance.MarginRight;
            _layoutPadding = MySettings.Instance.LayoutPadding;
            _configurableFilters = JsonConvert.DeserializeObject<List<Pair<string, string>>>(MySettings.Instance.Filters);

            LoadedCommand = new RelayCommand(Loaded);
            ClosingCommand = new RelayCommand<CancelEventArgs>(Closing);
            NotifyIconOpenCommand = new RelayCommand(() => { WindowState = WindowState.Normal; });
            NotifyIconExitCommand = new RelayCommand(() => { Application.Current.Shutdown(); });
            UpdateWindowsCommand = new RelayCommand(UpdateWindows);
            FilterAppCommand = new RelayCommand(FilterApp);
            FilterClassWithinAppCommand = new RelayCommand(FilterClassWithinApp);
        }

        public ICommand LoadedCommand { get; }
        public ICommand ClosingCommand { get; }
        public ICommand NotifyIconOpenCommand { get; }
        public ICommand NotifyIconExitCommand { get; }
        public ICommand UpdateWindowsCommand { get; }
        public ICommand FilterAppCommand { get; }
        public ICommand FilterClassWithinAppCommand { get; }

        public WindowState WindowState
        {
            get => _windowState;
            set
            {
                ShowInTaskbar = true;
                SetProperty(ref _windowState, value);
                ShowInTaskbar = value != WindowState.Minimized;
            }
        }

        public bool ShowInTaskbar
        {
            get => _showInTaskbar;
            set => SetProperty(ref _showInTaskbar, value);
        }

        public IEnumerable<ViewModelDesktopWindow> Windows
        {
            get => _windows;
            set => SetProperty(ref _windows, value);
        }

        public ViewModelDesktopWindow SelectedWindow
        {
            get => _selectedWindow;
            set => SetProperty(ref _selectedWindow, value);
        }

        public int LayoutPadding
        {
            get => _layoutPadding;
            set => SetProperty(ref _layoutPadding, value);
        }

        public int Padding
        {
            get => _padding;
            set => SetProperty(ref _padding, value);
        }

        public int MarginTop
        {
            get => _marginTop;
            set => SetProperty(ref _marginTop, value);
        }

        public int MarginRight
        {
            get => _marginRight;
            set => SetProperty(ref _marginRight, value);
        }

        public int MarginBottom
        {
            get => _marginBottom;
            set => SetProperty(ref _marginBottom, value);
        }

        public int MarginLeft
        {
            get => _marginLeft;
            set => SetProperty(ref _marginLeft, value);
        }

        public bool Disabled
        {
            get => _disabled;
            set => SetProperty(ref _disabled, value);
        }

        public List<Pair<string, string>> ConfigurableFilters
        {
            get => _configurableFilters;
            set => SetProperty(ref _configurableFilters, value);
        }

        public Pair<string, string> SelectedConfigurableFilter
        {
            get => _selectedConfigurableFilter;
            set => SetProperty(ref _selectedConfigurableFilter, value);
        }

        public NotifyIconWrapper.NotifyRequestRecord? NotifyRequest
        {
            get => _notifyRequest;
            set => SetProperty(ref _notifyRequest, value);
        }

        public void Notify(string text, string title, int duration)
        {
            NotifyRequest = new NotifyIconWrapper.NotifyRequestRecord
            {
                Title = title,
                Text = text,
                Duration = duration,
            };
        }

        public void UpdateWindows()
        {
            Windows = App.DWM.GetWindowsByVirtualDesktop(VirtualDesktop.Current).Select(window => new ViewModelDesktopWindow(
                window.Window.DangerousGetHandle().ToString(),
                window.AppName,
                window.ClassName,
                window.VirtualDesktop.Id.ToString(),
                window.Monitor.ToString()
                ));
        }

        public void FilterApp()
        {
            ConfigurableFilters = ConfigurableFilters.Concat(new[] { new Pair<string, string>(SelectedWindow.AppName, "*") }).ToList();
        }

        public void FilterClassWithinApp()
        {
            ConfigurableFilters = ConfigurableFilters.Concat(new[] { new Pair<string, string>(SelectedWindow.AppName, SelectedWindow.ClassName) }).ToList();
        }

        public void RemoveFilter()
        {
            ConfigurableFilters = ConfigurableFilters.Where(f => f.Item1 != SelectedConfigurableFilter.Item1 && f.Item2 != SelectedConfigurableFilter.Item2).ToList();
        }

        private void Loaded()
        {
            WindowState = WindowState.Minimized;
        }

        private void Closing(CancelEventArgs? e)
        {
            Debug.WriteLine($"Saving settings");
            MySettings.Save();
            if (e == null)
                return;
            e.Cancel = true;
            WindowState = WindowState.Minimized;
        }
    }
}