using AmethystWindows.DesktopWindowsManager;
using AmethystWindows.GridGenerator;
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
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using Vanara.PInvoke;
using Windows.ApplicationModel;
using Windows.Data.Xml.Dom;
using Windows.Foundation.Collections;
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

        public string Version { get; set; }

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
        private static ModifierKeys Modifier1 = ModifierKeys.Shift | ModifierKeys.Alt;
        private static ModifierKeys Modifier2 = ModifierKeys.Shift | ModifierKeys.Alt | ModifierKeys.Windows;
        private readonly HashSet<ViewModelHotkey> defaultHotkeys = new HashSet<ViewModelHotkey>()
        {
            new ViewModelHotkey() { Command = HotkeyCommand.rotateLayoutClockwise, Hotkey = new Hotkey(Key.Space, Modifier1), Description = "Rotate Layout Clockwise" },
            new ViewModelHotkey() { Command = HotkeyCommand.rotateLayoutCounterclockwise, Hotkey = new Hotkey(Key.Space, Modifier2), Description = "Rotate Layout Counterclockvise" },

            new ViewModelHotkey() { Command = HotkeyCommand.setMainPane, Hotkey = new Hotkey(Key.Enter, Modifier1), Description = "Set Main Pane" },
            new ViewModelHotkey() { Command = HotkeyCommand.setSecondaryPane, Hotkey = new Hotkey(Key.Enter, Modifier2), Description = "Set Secondary Pane" },

            new ViewModelHotkey() { Command = HotkeyCommand.swapFocusedCounterclockwise, Hotkey = new Hotkey(Key.H, Modifier1), Description = "Move Focused Window Counterclockwise" },
            new ViewModelHotkey() { Command = HotkeyCommand.swapFocusedClockwise, Hotkey = new Hotkey(Key.L, Modifier1), Description = "Move Focused Window Clockwise" },

            new ViewModelHotkey() { Command = HotkeyCommand.swapFocusCounterclockwise, Hotkey = new Hotkey(Key.J, Modifier1), Description = "Move Focus Counterclockwise" },
            new ViewModelHotkey() { Command = HotkeyCommand.swapFocusClockwise, Hotkey = new Hotkey(Key.K, Modifier1), Description = "Move Focus Counterclockwise" },

            new ViewModelHotkey() { Command = HotkeyCommand.moveFocusPreviousScreen, Hotkey = new Hotkey(Key.P, Modifier1), Description = "Move Focus To Previous Screen" },
            new ViewModelHotkey() { Command = HotkeyCommand.moveFocusNextScreen, Hotkey = new Hotkey(Key.N, Modifier1), Description = "Move Focus To Next Screen" },

            new ViewModelHotkey() { Command = HotkeyCommand.expandMainPane, Hotkey = new Hotkey(Key.L, Modifier2), Description = "Expand Main Pane" },
            new ViewModelHotkey() { Command = HotkeyCommand.shrinkMainPane, Hotkey = new Hotkey(Key.H, Modifier2), Description = "Shrink Main Pane" },

            new ViewModelHotkey() { Command = HotkeyCommand.moveFocusedPreviousScreen, Hotkey = new Hotkey(Key.K, Modifier2), Description = "Move Focused Window To Previous Screen" },
            new ViewModelHotkey() { Command = HotkeyCommand.moveFocusedNextScreen, Hotkey = new Hotkey(Key.J, Modifier2), Description = "Move Focused Window TO Next Screen" },

            new ViewModelHotkey() { Command = HotkeyCommand.redrawSimple, Hotkey = new Hotkey(Key.Z, Modifier1), Description = "Redraw" },
            new ViewModelHotkey() { Command = HotkeyCommand.redrawForced, Hotkey = new Hotkey(Key.Z, Modifier2), Description = "Evaluate And Redraw" },

            new ViewModelHotkey() { Command = HotkeyCommand.moveFocusedNextSpace, Hotkey = new Hotkey(Key.Left, Modifier2), Description = "Move Focused Window To Next Virtual Desktop" },
            new ViewModelHotkey() { Command = HotkeyCommand.moveFocusedPreviousSpace, Hotkey = new Hotkey(Key.Right, Modifier2), Description = "Move Focused Window To Next Virtual Desktop" },

            new ViewModelHotkey() { Command = HotkeyCommand.moveFocusedToSpace1, Hotkey = new Hotkey(Key.D1, Modifier2), Description = "Move Focused Window To First Virtual Desktop" },
            new ViewModelHotkey() { Command = HotkeyCommand.moveFocusedToSpace2, Hotkey = new Hotkey(Key.D2, Modifier2), Description = "Move Focused Window To Second Virtual Desktop" },
            new ViewModelHotkey() { Command = HotkeyCommand.moveFocusedToSpace3, Hotkey = new Hotkey(Key.D3, Modifier2), Description = "Move Focused Window To Third Virtual Desktop" },
            new ViewModelHotkey() { Command = HotkeyCommand.moveFocusedToSpace4, Hotkey = new Hotkey(Key.D4, Modifier2), Description = "Move Focused Window To Fouth Virtual Desktop" },
            new ViewModelHotkey() { Command = HotkeyCommand.moveFocusedToSpace5, Hotkey = new Hotkey(Key.D5, Modifier2), Description = "Move Focused Window To Fifth Virtual Desktop" },

            new ViewModelHotkey() { Command = HotkeyCommand.switchToSpace1, Hotkey = new Hotkey(Key.D1, Modifier1), Description = "Switch To First Virtual Desktop" },
            new ViewModelHotkey() { Command = HotkeyCommand.switchToSpace2, Hotkey = new Hotkey(Key.D2, Modifier1), Description = "Switch To Second Virtual Desktop" },
            new ViewModelHotkey() { Command = HotkeyCommand.switchToSpace3, Hotkey = new Hotkey(Key.D3, Modifier1), Description = "Switch To Third Virtual Desktop" },
            new ViewModelHotkey() { Command = HotkeyCommand.switchToSpace4, Hotkey = new Hotkey(Key.D4, Modifier1), Description = "Switch To Fourth Virtual Desktop" },
            new ViewModelHotkey() { Command = HotkeyCommand.switchToSpace5, Hotkey = new Hotkey(Key.D5, Modifier1), Description = "Switch To Fifth Virtual Desktop" },
        };

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
        private int _virtualDesktops;

        private bool _disabled;
        private string _version;

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
            _padding = !MySettings.Instance.Padding.Equals(null) ? MySettings.Instance.Padding : 0;
            _marginTop = !MySettings.Instance.MarginTop.Equals(null) ? MySettings.Instance.MarginTop : 0;
            _marginBottom = !MySettings.Instance.MarginBottom.Equals(null) ? MySettings.Instance.MarginBottom : 0;
            _marginLeft = !MySettings.Instance.MarginLeft.Equals(null) ? MySettings.Instance.MarginLeft : 0;
            _marginRight = !MySettings.Instance.MarginRight.Equals(null) ? MySettings.Instance.MarginRight : 0;
            _layoutPadding = !MySettings.Instance.LayoutPadding.Equals(null) ? MySettings.Instance.LayoutPadding : 4;
            _virtualDesktops = !MySettings.Instance.VirtualDesktops.Equals(null) ? MySettings.Instance.VirtualDesktops : 3;
            _step = !MySettings.Instance.Step.Equals(null) ? MySettings.Instance.Step : 25;

            _disabled = !MySettings.Instance.Disabled.Equals(null) ? MySettings.Instance.Disabled : false;
            _version = getVersion();

            _configurableFilters = MySettings.Instance.Filters;
            _configurableAdditions = MySettings.Instance.Additions;

            if (MySettings.Instance.Hotkeys.Count! < defaultHotkeys.Count)
            {
                MessageBox.Show("Hotkeys have been reset to default.", "Amethyst Windows", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK, MessageBoxOptions.DefaultDesktopOnly);
                MySettings.Instance.Hotkeys = defaultHotkeys;
                MySettings.Save();
            }
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

        public int VirtualDesktops
        {
            get => _virtualDesktops;
            set => SetProperty(ref _virtualDesktops, value);
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

        public string Version
        {
            get => _version;
            set => SetProperty(ref _version, value);
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

        private string getVersion()
        {
            var version = Package.Current.Id.Version;
            return $"{version.Major}.{version.Minor}.{version.Build}.{version.Revision}";
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
                Layout = Layout.TallLeft;
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

    public enum HotkeyCommand : int
    {
        rotateLayoutClockwise = 0,
        rotateLayoutCounterclockwise = 1,

        setMainPane = 2,
        setSecondaryPane = 3,

        swapFocusedCounterclockwise = 4,
        swapFocusedClockwise = 5,

        swapFocusCounterclockwise = 6,
        swapFocusClockwise = 7,

        moveFocusPreviousScreen = 8,
        moveFocusNextScreen = 9,

        expandMainPane = 10,
        shrinkMainPane = 11,

        moveFocusedPreviousScreen = 12,
        moveFocusedNextScreen = 13,
        
        redrawSimple = 14,
        redrawForced = 15,

        moveFocusedNextSpace = 16,
        moveFocusedPreviousSpace = 17,

        moveFocusedToSpace1 = 18,
        moveFocusedToSpace2 = 19,
        moveFocusedToSpace3 = 20,
        moveFocusedToSpace4 = 21,
        moveFocusedToSpace5 = 22,

        switchToSpace1 = 23,
        switchToSpace2 = 24,
        switchToSpace3 = 25,
        switchToSpace4 = 26,
        switchToSpace5 = 27
    }

    public class ViewModelHotkey : INotifyPropertyChanged
    {
        private Hotkey _hotkey;
        private HotkeyCommand _command;
        private string _description;

        public Hotkey Hotkey {
            get => _hotkey;
            set
            {
                _hotkey = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Hotkey)));
            }
        }
        public HotkeyCommand Command {
            get => _command;
            set
            {
                _command = value;
            }
        }

        public string Description { get => _description; set => _description = value; }

        public ViewModelHotkey(HotkeyCommand command, Hotkey hotkey, string description)
        {
            Hotkey = hotkey;
            Command = command;
            Description = description;
        }

        public ViewModelHotkey()
        {
        }

        public event PropertyChangedEventHandler? PropertyChanged;
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
        public ObservableHotkeys(HashSet<ViewModelHotkey> list) : base(list)
        {
            foreach (ViewModelHotkey viewModelHotkey in list)
            {
                viewModelHotkey.PropertyChanged += ItemPropertyChanged;
            }
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