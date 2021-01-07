using Hemera.Models;
using Hemera.Resx;
using Hemera.Views;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Xamarin.Forms.Maps;

namespace Hemera.ViewModels
{
    public class DetailViewViewModel : INotifyPropertyChanged
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

        private bool _NotesEnabled;
        public bool NotesEnabled
        {
            get => _NotesEnabled;
            set
            {
                _NotesEnabled = value;
                OnPropertyChanged();
            }
        }

        private bool _ChecklistEnabled;
        public bool ChecklistEnabled
        {
            get => _ChecklistEnabled;
            set
            {
                _ChecklistEnabled = value;
                OnPropertyChanged();
            }
        }

        private bool _MapEnabled;
        public bool MapEnabled
        {
            get => _MapEnabled;
            set
            {
                _MapEnabled = value;
                OnPropertyChanged();
            }
        }

        public DetailViewViewModel(DetailView page, Activity activity)
        {
            Activity = activity;

            NotesEnabled = activity.Notes?.Length > 0;
            ChecklistEnabled = activity.Checklist?.Count > 0;

            //Show the pin on the map and center it
            if (activity.Position != default)
            {
                MapEnabled = true;
                page.map.Pins.Add(new Pin()
                {
                    Position = activity.Position,
                    Label = AppResources.SelectedPosition
                });
                page.map.MoveToRegion(MapSpan.FromCenterAndRadius(activity.Position, Distance.FromKilometers(1)));
            }
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
