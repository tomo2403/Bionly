﻿using Bionly.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Bionly.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DashboardPage : ContentPage
    {
        private string DeviceId = null;

        public DashboardPage()
        {
            InitializeComponent();
            SettingsViewModel.LoadAllDevices.Execute(null);
        }

        private void ListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            DeviceId = ((Models.Device)e.Item).Id;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            WelcomeTxt.Text = ((DashboardViewModel)BindingContext).GetWelcomeText();
            WelcomeImg.Source = ((DashboardViewModel)BindingContext).GetWelcomeImage();
            ConnectedTxt.Text = ((DashboardViewModel)BindingContext).GetConnectedText();
            RadChart.Chart = ((DashboardViewModel)BindingContext).radChart;
        }

        private async void ChartsBtn_Clicked(object sender, System.EventArgs e)
        {
            await Navigation.PushAsync(new ChartsPage(DeviceId));
        }

        private async void MeasurementsBtn_Clicked(object sender, System.EventArgs e)
        {
            await Navigation.PushAsync(new MeasurementsPage(DeviceId));
        }

        private async void ImagesBtn_Clicked(object sender, System.EventArgs e)
        {
            await Navigation.PushAsync(new ImagesPage(DeviceId));
        }

        private async void ConfigBtn_Clicked(object sender, System.EventArgs e)
        {
            await DisplayAlert("Fehler", "Das Gerät unterstützt keine Fernkonfiguration.", "OK");
        }

        private void DeviceList_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItemIndex < 0)
            {
                StartpageContainer.IsVisible = true;
                DeviceContainer.IsVisible = false;
            }
            else
            {
                StartpageContainer.IsVisible = false;
                DeviceContainer.IsVisible = true;
            }
        }
    }
}