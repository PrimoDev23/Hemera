using Hemera.Models;
using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Hemera.ViewModels.Popups
{
    public class MeetingViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private Activity _Activity;
        public Activity Activity
        {
            get => _Activity;
            set
            {
                _Activity = value;
                OnPropertyChanged();
            }
        }

        public string Location { get; set; }

        public Command ReturnCommand { get; set; }

        //Meeting page;
        public MeetingViewModel()
        {
            ReturnCommand = new Command(new Action(setLocation));

            //this.page = page;
        }

        private async void setLocation()
        {
            Location loc = (await Geocoding.GetLocationsAsync(Location)).First();

            //page.map.MoveToRegion(MapSpan.FromCenterAndRadius(new Position(loc.Latitude, loc.Longitude), Distance.FromKilometers(1)));
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
