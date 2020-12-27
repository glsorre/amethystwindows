using AmethystWindows.ViewModels;
using GalaSoft.MvvmLight.Threading;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.AppService;
using Windows.Foundation;
using Windows.Foundation.Collections;
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
        public SettingPage()
        {
            this.InitializeComponent();
            NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Enabled;
            SettingsBarButton.IsChecked = true;
            ApplicationsBarButton.Click += SettingsBarButton_Click;
            DataContext = App.mainViewModel;
            PaddingNumberBox.Loaded += PaddingNumberBox_Loaded;
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
            DispatcherHelper.CheckBeginInvokeOnUI(async () => {
                ValueSet message = new ValueSet();
                message.Add("padding_set", args.NewValue);
                await App.Connection.SendMessageAsync(message);
            });
        }

        private void SettingsBarButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MainPage), "SettingPage");
        }
    }
}
