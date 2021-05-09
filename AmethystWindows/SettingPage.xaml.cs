using AmethystWindows.ViewModels;
using DebounceThrottle;
using GalaSoft.MvvmLight.Threading;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.AppService;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace AmethystWindows
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SettingPage : Page
    {
        private DebounceDispatcher debounceDispatcher = new DebounceDispatcher(250);

        public SettingPage()
        {
            this.InitializeComponent();
            DataContext = App.mainViewModel;
            NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Enabled;
            SettingsBarButton.IsChecked = true;
            ApplicationsBarButton.Click += SettingsBarButton_Click;
            StartupButton.Click += StartupButton_Click;
            PaddingNumberBox.Loaded += PaddingNumberBox_Loaded;
            MarginTopNumberBox.Loaded += MarginTopNumberBox_Loaded;
            MarginBottonNumberBox.Loaded += MarginBottonNumberBox_Loaded;
            MarginLeftNumberBox.Loaded += MarginLeftNumberBox_Loaded;
            MarginRightNumberBox.Loaded += MarginRightNumberBox_Loaded;
        }

        private void MarginRightNumberBox_Loaded(object sender, RoutedEventArgs e)
        {
            MarginRightNumberBox.ValueChanged += MarginRightNumberBox_ValueChanged;
        }

        private void MarginRightNumberBox_ValueChanged(Microsoft.UI.Xaml.Controls.NumberBox sender, Microsoft.UI.Xaml.Controls.NumberBoxValueChangedEventArgs args)
        {
            debounceDispatcher.Debounce(() =>
            {
                DispatcherHelper.CheckBeginInvokeOnUI(async () =>
                {
                    ValueSet message = new ValueSet();
                    message.Add("margin_right_set", args.NewValue);
                    await App.Connection.SendMessageAsync(message);
                });
            });
        }

        private void MarginLeftNumberBox_Loaded(object sender, RoutedEventArgs e)
        {
            MarginLeftNumberBox.ValueChanged += MarginLeftNumberBox_ValueChanged;
        }

        private void MarginLeftNumberBox_ValueChanged(Microsoft.UI.Xaml.Controls.NumberBox sender, Microsoft.UI.Xaml.Controls.NumberBoxValueChangedEventArgs args)
        {
            debounceDispatcher.Debounce(() =>
            {
                DispatcherHelper.CheckBeginInvokeOnUI(async () =>
                {
                    ValueSet message = new ValueSet();
                    message.Add("margin_left_set", args.NewValue);
                    await App.Connection.SendMessageAsync(message);
                });
            });
        }

        private void MarginBottonNumberBox_Loaded(object sender, RoutedEventArgs e)
        {
            MarginBottonNumberBox.ValueChanged += MarginBottonNumberBox_ValueChanged;
        }

        private void MarginBottonNumberBox_ValueChanged(Microsoft.UI.Xaml.Controls.NumberBox sender, Microsoft.UI.Xaml.Controls.NumberBoxValueChangedEventArgs args)
        {
            debounceDispatcher.Debounce(() =>
            {
                DispatcherHelper.CheckBeginInvokeOnUI(async () =>
                {
                    ValueSet message = new ValueSet();
                    message.Add("margin_bottom_set", args.NewValue);
                    await App.Connection.SendMessageAsync(message);
                });
            });
        }

        private void MarginTopNumberBox_Loaded(object sender, RoutedEventArgs e)
        {
            MarginTopNumberBox.ValueChanged += MarginTopNumberBox_ValueChanged;
        }

        private void MarginTopNumberBox_ValueChanged(Microsoft.UI.Xaml.Controls.NumberBox sender, Microsoft.UI.Xaml.Controls.NumberBoxValueChangedEventArgs args)
        {
            debounceDispatcher.Debounce(() =>
            {
                DispatcherHelper.CheckBeginInvokeOnUI(async () =>
                {
                    ValueSet message = new ValueSet();
                    message.Add("margin_top_set", args.NewValue);
                    await App.Connection.SendMessageAsync(message);
                });
            });
        }

        private async void StartupButton_Click(object sender, RoutedEventArgs e)
        {
            StartupTask startupTask = await StartupTask.GetAsync("AmethystWindowsId");
            StartupStatus.Text = startupTask.State.ToString();
            switch (startupTask.State)
            {
                case StartupTaskState.Disabled:
                    // Task is disabled but can be enabled.
                    StartupTaskState newState = await startupTask.RequestEnableAsync();
                    StartupStatus.Text = newState.ToString();
                    break;
                case StartupTaskState.DisabledByUser:
                    // Task is disabled and user must enable it manually.
                    MessageDialog disabledDialog = new MessageDialog(
                        "You can enable this in the Startup tab in Task Manager.");
                    await disabledDialog.ShowAsync();
                    break;
                case StartupTaskState.DisabledByPolicy:
                    MessageDialog policyDialog = new MessageDialog(
                        "You do not have rights to start this app at login.");
                    await policyDialog.ShowAsync();
                    break;
                case StartupTaskState.Enabled:
                    break;
            }
        }

        private void FilterRemoveButton_Click(object sender, RoutedEventArgs e)
        {
            var item = (sender as FrameworkElement).DataContext;
            List<Filter> filters = new List<Filter>(App.mainViewModel.Filters);
            int index = filters.IndexOf((Filter)item);
            filters.RemoveAt(index);
            App.mainViewModel.Filters = filters;
            SettingPage_SendFilters();
        }

        private async void SettingPage_SendFilters()
        {
            ValueSet message = new ValueSet();
            List<List<String>> list = new List<List<String>>();
            foreach (var f in App.mainViewModel.Filters)
            {
                List<String> item = new List<string>();
                item.Add(f.AppName);
                item.Add(f.ClassName);
                list.Add(item);
            }
            message.Add("filters_set", JsonConvert.SerializeObject(list));
            await App.Connection.SendMessageAsync(message);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
        }

        private void PaddingNumberBox_Loaded(object sender, RoutedEventArgs e)
        {
            PaddingNumberBox.ValueChanged += PaddingNumberBox_ValueChanged;
        }

        private void PaddingNumberBox_ValueChanged(Microsoft.UI.Xaml.Controls.NumberBox sender, Microsoft.UI.Xaml.Controls.NumberBoxValueChangedEventArgs args)
        {
            debounceDispatcher.Debounce(() =>
            {
                DispatcherHelper.CheckBeginInvokeOnUI(async () =>
                {
                    ValueSet message = new ValueSet();
                    message.Add("padding_set", args.NewValue);
                    await App.Connection.SendMessageAsync(message);
                });
            });
        }

        private void SettingsBarButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MainPage), "SettingPage");
        }
    }
}
