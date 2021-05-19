using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Windows.ApplicationModel.AppService;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace AmethystWindows
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>

    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Enabled;
            DataContext = App.mainViewModel;
            ApplicationsBarButton.IsChecked = true;
            DisabledBarButton.IsChecked = App.mainViewModel.Disabled;
            SettingsBarButton.Click += SettingsBarButton_Click;
            RefreshButton.Click += RefreshButton_Click;
            RedrawButton.Click += RedrawButton_Click;
            DisabledBarButton.Click += DisabledBarButton_Click;
        }

        private async void DisabledBarButton_Click(object sender, RoutedEventArgs e)
        {
            if (App.mainViewModel.Disabled)
            {
                DisabledBarButton.IsChecked = false;
                App.mainViewModel.Disabled = false;
                
            } else
            {
                DisabledBarButton.IsChecked = true;
                App.mainViewModel.Disabled = true;
            }
            ValueSet message = new ValueSet();
            message.Add("disable_set", App.mainViewModel.Disabled);
            await App.Connection.SendMessageAsync(message);
        }

        private async void RedrawButton_Click(object sender, RoutedEventArgs e)
        {
            ValueSet message = new ValueSet();
            message.Add("redraw", "");
            await App.Connection.SendMessageAsync(message);
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            App.App_Refresh();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
        }

        private void SettingsBarButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(SettingPage), "MainPage");
        }

        private void FilterWindowsButton_Click(object sender, RoutedEventArgs e)
        {
            var item = (sender as FrameworkElement).DataContext;
            DesktopWindow desktopWindow = (DesktopWindow)item;
            List<Filter> filters = new List<Filter>(App.mainViewModel.Filters);

            Filter filter = new Filter(desktopWindow.AppName);
            if (!filters.Contains(filter))
            {
                filters.Add(filter);
            }
 
            App.mainViewModel.Filters = filters;
            App.App_SendFilters();
        }

        private void FilterClassButton_Click(object sender, RoutedEventArgs e)
        {
            var item = (sender as FrameworkElement).DataContext;
            DesktopWindow desktopWindow = (DesktopWindow)item;
            List<Filter> filters = new List<Filter>(App.mainViewModel.Filters);

            Filter filter = new Filter(desktopWindow.AppName, desktopWindow.ClassName);
            if (!filters.Contains(filter))
            {
                filters.Add(filter);
            }

            App.mainViewModel.Filters = filters;
            App.App_SendFilters();
        }
    }

    public sealed class AppBarLockableToggleButton : AppBarToggleButton
    {
        protected override void OnToggle()
        {
            if (!LockToggle)
            {
                base.OnToggle();
            }
        }

        public bool LockToggle { get; set; }
    }
}
