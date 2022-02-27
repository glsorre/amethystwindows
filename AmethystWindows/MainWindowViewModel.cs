using AmethystWindows.DesktopWindowsManager;
using AmethystWindows.Hotkeys;
using AmethystWindows.Settings;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using Vanara.PInvoke;
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

        public ViewModelDesktopWindow(string appName, string className)
        {
            AppName = appName;
            ClassName = className;
        }

        public ViewModelDesktopWindow(string window, string appName, string className, string virtualDesktop, string monitor)
        {
            Window = window;
            AppName = appName;
            ClassName = className;
            VirtualDesktop = virtualDesktop;
            Monitor = monitor;
        }

        public override bool Equals(object? obj)
        {
            return obj is ViewModelDesktopWindow window &&
                   Window == window.Window &&
                   AppName == window.AppName &&
                   ClassName == window.ClassName &&
                   VirtualDesktop == window.VirtualDesktop &&
                   Monitor == window.Monitor;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Window, AppName, ClassName, VirtualDesktop, Monitor);
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

    public class ViewModelConfigurableFilterConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Pair<string, string> selectedConfigurableFilter)
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

    public class MainWindowViewModel : ObservableRecipient
    {
        private NotifyIconWrapper.NotifyRequestRecord? _notifyRequest;
        private bool _showInTaskbar;
        private WindowState _windowState;
        private List<ViewModelDesktopWindow> _windows;
        private List<ViewModelDesktopWindow> _excludedWindows;
        private ViewModelDesktopWindow _selectedWindow;
        private ViewModelDesktopWindow _selectedExcludedWindow;

        private int _layoutPadding;
        private int _padding;
        private int _marginTop;
        private int _marginRight;
        private int _marginBottom;
        private int _marginLeft;
        private int _step;

        private bool _disabled;

        private List<Pair<string, string>> _configurableFilters;
        private Pair<string, string> _selectedConfigurableFilter;

        private List<Pair<string, string>> _configurableAdditions;
        private Pair<string, string> _selectedConfigurableAddition;

        private Pair<VirtualDesktop, HMONITOR> _lastChangedDesktopMonitor;
        private static ObservableDesktopMonitors _desktopMonitors;
        private static ObservableHotkeys _hotkeys;

        public MainWindowViewModel()
        {
            MySettings.Load();
            _padding = MySettings.Instance.Padding;
            _marginTop = MySettings.Instance.MarginTop;
            _marginBottom = MySettings.Instance.MarginBottom;
            _marginLeft = MySettings.Instance.MarginLeft;
            _marginRight = MySettings.Instance.MarginRight;
            _layoutPadding = MySettings.Instance.LayoutPadding;
            _step = MySettings.Instance.Step;

            _disabled = MySettings.Instance.Disabled;

            _configurableFilters = MySettings.Instance.Filters;
            _configurableAdditions = MySettings.Instance.Additions;
            _hotkeys = new ObservableHotkeys(MySettings.Instance.Hotkeys);
            _desktopMonitors = new ObservableDesktopMonitors(MySettings.Instance.DesktopMonitors);
            _windows = new List<ViewModelDesktopWindow>();
            _excludedWindows = new List<ViewModelDesktopWindow>();

            _hotkeys.CollectionChanged += _hotkeys_CollectionChanged;
            _desktopMonitors.CollectionChanged += _desktopMonitors_CollectionChanged;
            _lastChangedDesktopMonitor = new Pair<VirtualDesktop, HMONITOR>(null, new HMONITOR());

            LoadedCommand = new RelayCommand(Loaded);
            ClosingCommand = new RelayCommand<CancelEventArgs>(Closing);
            NotifyIconOpenCommand = new RelayCommand(() => { WindowState = WindowState.Normal; });
            NotifyIconExitCommand = new RelayCommand(() => { Application.Current.Shutdown(); });
            UpdateWindowsCommand = new RelayCommand(UpdateWindows);
            FilterAppCommand = new RelayCommand(FilterApp);
            FilterClassWithinAppCommand = new RelayCommand(FilterClassWithinApp);
            RemoveFilterCommand = new RelayCommand(RemoveFilter);
            AddAppCommand = new RelayCommand(AddApp);
            AddClassWithinAppCommand = new RelayCommand(AddClassWithinApp);
            RemoveAdditionCommand = new RelayCommand(RemoveAddition);
            RedrawCommand = new RelayCommand(() => { App.DWM.ClearWindows(); App.DWM.CollectWindows(); App.DWM.Draw(); });
        }

        private void _desktopMonitors_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {   
            ViewModelDesktopMonitor viewModelDesktopMonitor= (ViewModelDesktopMonitor)e.NewItems[0];
            LastChangedDesktopMonitor = viewModelDesktopMonitor.getPair();
            OnPropertyChanged("DesktopMonitors");
        }

        private void _hotkeys_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged("Hotkeys");
        }

        public ICommand LoadedCommand { get; }
        public ICommand ClosingCommand { get; }
        public ICommand NotifyIconOpenCommand { get; }
        public ICommand NotifyIconExitCommand { get; }
        public ICommand UpdateWindowsCommand { get; }
        public ICommand FilterAppCommand { get; }
        public ICommand FilterClassWithinAppCommand { get; }
        public ICommand RemoveFilterCommand { get; }
        public ICommand AddAppCommand { get; }
        public ICommand AddClassWithinAppCommand { get; }
        public ICommand RemoveAdditionCommand { get; }
        public ICommand RedrawCommand { get; }
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

        public List<ViewModelDesktopWindow> Windows
        {
            get => _windows;
            set => SetProperty(ref _windows, value);
        }

        public List<ViewModelDesktopWindow> ExcludedWindows
        {
            get => _excludedWindows;
            set => SetProperty(ref _excludedWindows, value);
        }

        public ViewModelDesktopWindow SelectedWindow
        {
            get => _selectedWindow;
            set => SetProperty(ref _selectedWindow, value);
        }

        public ViewModelDesktopWindow SelectedExcludedWindow
        {
            get => _selectedExcludedWindow;
            set => SetProperty(ref _selectedExcludedWindow, value);
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

        public int Step
        {
            get => _step;
            set => SetProperty(ref _step, value);
        }

        public bool Disabled
        {
            get => _disabled;
            set => SetProperty(ref _disabled, value);
        }

        public ObservableDesktopMonitors DesktopMonitors
        {
            get => _desktopMonitors;
            set => SetProperty(ref _desktopMonitors, value);
        }

        public ObservableHotkeys Hotkeys
        {
            get => _hotkeys;
            set => SetProperty(ref _hotkeys, value);
        }

        public Pair<VirtualDesktop, HMONITOR> LastChangedDesktopMonitor
        {
            get;
            set;
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

        public List<Pair<string, string>> ConfigurableAdditions
        {
            get => _configurableAdditions;
            set => SetProperty(ref _configurableAdditions, value);
        }

        public Pair<string, string> SelectedConfigurableAddition
        {
            get => _selectedConfigurableAddition;
            set => SetProperty(ref _selectedConfigurableAddition, value);
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
            List<ViewModelDesktopWindow> windowsForComparison = App.DWM.GetWindowsByVirtualDesktop(VirtualDesktop.Current).Select(window => new ViewModelDesktopWindow(
                window.Window.DangerousGetHandle().ToString(),
                window.AppName,
                window.ClassName,
                window.VirtualDesktop.Id.ToString(),
                window.Monitor.ToString()
                )).ToList();

            if (!windowsForComparison.SequenceEqual(Windows)) Windows = windowsForComparison;
        }

        public void UpdateExcludedWindows()
        {
            List<ViewModelDesktopWindow> windowsForComparison = App.DWM.ExcludedWindows.Select(window => new ViewModelDesktopWindow(
                window.AppName,
                window.ClassName
                )).ToList();

            if (!windowsForComparison.SequenceEqual(ExcludedWindows)) ExcludedWindows = windowsForComparison;
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
            ConfigurableFilters = ConfigurableFilters.Where(f => f.Key != SelectedConfigurableFilter.Key && f.Value != SelectedConfigurableFilter.Value).ToList();
        }

        public void AddApp()
        {
            ConfigurableAdditions = ConfigurableAdditions.Concat(new[] { new Pair<string, string>(SelectedExcludedWindow.AppName, "*") }).ToList();
        }

        public void AddClassWithinApp()
        {
            ConfigurableAdditions = ConfigurableAdditions.Concat(new[] { new Pair<string, string>(SelectedExcludedWindow.AppName, SelectedExcludedWindow.ClassName) }).ToList();
        }

        public void RemoveAddition()
        {
            ConfigurableAdditions = ConfigurableAdditions.Where(f => f.Key != SelectedConfigurableAddition.Key && f.Value != SelectedConfigurableAddition.Value).ToList();
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

    public class ViewModelDesktopMonitor : INotifyPropertyChanged
    {
        private HMONITOR _monitor;
        private VirtualDesktop _virtualDesktop;
        private int _factor;
        private Layout _layout;
        public HMONITOR Monitor { 
            get => _monitor;

            set
            {
                _monitor = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Monitor)));
            }
        }
        public VirtualDesktop VirtualDesktop {
            get => _virtualDesktop;

            set
            {
                _virtualDesktop = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(VirtualDesktop)));
            }
        }
        public int Factor
        {
            get => _factor;

            set
            {
                _factor = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Factor)));
            }
        }
        public Layout Layout
        {
            get => _layout;

            set
            {
                _layout = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Layout)));
            }
        }

        public ViewModelDesktopMonitor(HMONITOR monitor, VirtualDesktop virtualDesktop, int factor, Layout layout)
        {
            Monitor = monitor;
            VirtualDesktop = virtualDesktop;
            Factor = factor;
            Layout = layout;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        public void Shrink()
        {
            --Factor;
        }

        public void Expand()
        {
            ++Factor;
        }

        public void RotateLayoutClockwise()
        {
            IEnumerable<Layout> values = Enum.GetValues(typeof(Layout)).Cast<Layout>();
            if (Layout == values.Max())
            {
                Layout = Layout.Horizontal;
            }
            else
            {
                ++Layout;
            }
        }

        public void RotateLayoutCounterClockwise()
        {
            IEnumerable<Layout> values = Enum.GetValues(typeof(Layout)).Cast<Layout>();
            if (Layout == 0)
            {
                Layout = Layout.Tall;
            }
            else
            {
                --Layout;
            }
        }

        public override bool Equals(object? obj)
        {
            return obj is ViewModelDesktopMonitor monitor &&
                   EqualityComparer<HMONITOR>.Default.Equals(Monitor, monitor.Monitor) &&
                   EqualityComparer<VirtualDesktop>.Default.Equals(VirtualDesktop, monitor.VirtualDesktop) &&
                   Factor == monitor.Factor &&
                   Layout == monitor.Layout;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Monitor, VirtualDesktop, Factor, Layout);
        }

        public Pair<VirtualDesktop, HMONITOR> getPair()
        {
            return new Pair<VirtualDesktop, HMONITOR>(VirtualDesktop, Monitor);
        }
    }

    public class ViewModelHotkey : INotifyPropertyChanged
    {
        private Hotkey _hotkey;
        private string _command;

        public Hotkey Hotkey {
            get => _hotkey;
            set
            {
                _hotkey = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Hotkey)));
            }
        }
        public string Command {
            get => _command;
            set
            {
                _command = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Hotkey)));
            }
        }

        public ViewModelHotkey(string command, Hotkey hotkey)
        {
            Hotkey = hotkey;
            Command = command;
        }

        public ViewModelHotkey()
        {
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        public override bool Equals(object? obj)
        {
            return obj is ViewModelHotkey hotkey &&
                   EqualityComparer<Hotkey>.Default.Equals(Hotkey, hotkey.Hotkey) &&
                   Command == hotkey.Command;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Hotkey, Command);
        }
    }

    public class ObservableDesktopMonitors : ObservableCollection<ViewModelDesktopMonitor>
    {
        public ObservableDesktopMonitors(List<ViewModelDesktopMonitor> list) : base(list)
        {
            foreach (ViewModelDesktopMonitor viewModelDesktopMonitor in list)
            {
                viewModelDesktopMonitor.PropertyChanged += ItemPropertyChanged;
            }
            CollectionChanged += ObservableDesktopMonitors_CollectionChanged;
        }

        public ObservableDesktopMonitors()
        {
            CollectionChanged += ObservableDesktopMonitors_CollectionChanged;
        }

        private void ObservableDesktopMonitors_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (Object item in e.NewItems)
                {
                    ((INotifyPropertyChanged)item).PropertyChanged += ItemPropertyChanged;
                }
            }
            if (e.OldItems != null)
            {
                foreach (Object item in e.OldItems)
                {
                    ((INotifyPropertyChanged)item).PropertyChanged -= ItemPropertyChanged;
                }
            }
        }

        private void ItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            NotifyCollectionChangedEventArgs args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, sender, sender);
            OnCollectionChanged(args);
        }

        public ViewModelDesktopMonitor this[Pair<VirtualDesktop, HMONITOR> desktopMonitor] => FindByDesktopMonitor(desktopMonitor);

        private ViewModelDesktopMonitor FindByDesktopMonitor(Pair<VirtualDesktop, HMONITOR> desktopMonitor)
        {
            return this.First(viewModelDesktopMonitor => viewModelDesktopMonitor.Monitor.Equals(desktopMonitor.Value) && viewModelDesktopMonitor.VirtualDesktop.Equals(desktopMonitor.Key));
        }
    }

    public class ObservableHotkeys : ObservableCollection<ViewModelHotkey>
    {
        public ObservableHotkeys(List<ViewModelHotkey> list) : base(list)
        {
            foreach (ViewModelHotkey viewModelHotkey in list)
            {
                viewModelHotkey.PropertyChanged += ItemPropertyChanged;
            }
            CollectionChanged += ObservableHotkeys_CollectionChanged;
        }

        public ObservableHotkeys()
        {
            CollectionChanged += ObservableHotkeys_CollectionChanged;
        }

        private void ObservableHotkeys_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (Object item in e.NewItems)
                {
                    ((INotifyPropertyChanged)item).PropertyChanged += ItemPropertyChanged;
                }
            }
            if (e.OldItems != null)
            {
                foreach (Object item in e.OldItems)
                {
                    ((INotifyPropertyChanged)item).PropertyChanged -= ItemPropertyChanged;
                }
            }
        }

        private void ItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            NotifyCollectionChangedEventArgs args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, sender, sender);
            OnCollectionChanged(args);
        }
    }
}