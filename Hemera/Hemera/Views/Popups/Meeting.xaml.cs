
using Hemera.Interfaces;
using Hemera.Resx;
using Hemera.ViewModels.Popups;
using Plugin.Geolocator;
using System;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Xaml;

namespace Hemera.Views.Popups
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Meeting : ContentView, IValidate
    {
        public MeetingViewModel viewModel;

        public Meeting()
        {
            InitializeComponent();

            viewModel = new MeetingViewModel(this);
            BindingContext = viewModel;
        }

        public async Task CenterUsersLocation()
        {
            try
            {
                var position = await Geolocation.GetLocationAsync().ConfigureAwait(false);

                void UI()
                {
                    map.MoveToRegion(MapSpan.FromCenterAndRadius(new Position(position.Latitude, position.Longitude), Distance.FromKilometers(1)));
                }
                await Device.InvokeOnMainThreadAsync(new Action(UI)).ConfigureAwait(false);
            }
            catch
            {
                //Location is not enabled
            }
        }

        public async Task<bool> ValidateInput()
        {
            if (!(viewModel.Activity?.Title?.Length > 0))
            {
                void setHeight()
                {
                    lbl_titlemissing.HeightRequest = 20;
                }
                await Device.InvokeOnMainThreadAsync(new Action(setHeight)).ConfigureAwait(false);
                return false;
            }
            else
            {
                void setHeight()
                {
                    lbl_titlemissing.HeightRequest = 0;
                }
                await Device.InvokeOnMainThreadAsync(new Action(setHeight)).ConfigureAwait(false);
            }

            return true;
        }

        private void map_MapClicked(object sender, MapClickedEventArgs e)
        {
            map.Pins.Clear();
            map.Pins.Add(new Pin()
            {
                Position = e.Position,
                Label = AppResources.SelectedPosition
            });

            viewModel.Activity.Position = e.Position;
        }
    }
}