using Hemera.Helpers;
using Hemera.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using Xamarin.Forms;

namespace Hemera.ViewModels
{
    public class OverviewViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public Command OpenMenuCommand { get; set; }

        private ObservableCollection<Day> _DayPlans = new ObservableCollection<Day>()
        {
            new Day()
            {
                Date = DateTime.Now.AddHours(1),
                Title = "Test",
                Notes = "IDK"
            },
            new Day()
            {
                Date = DateTime.Now.AddHours(1),
                Title = "Test",
                Notes = "IDK"
            },
            new Day()
            {
                Date = DateTime.Now.AddHours(1),
                Title = "Test",
                Notes = "IDK"
            },
            new Day()
            {
                Date = DateTime.Now.AddHours(1),
                Title = "Test",
                Notes = "IDK"
            },
            new Day()
            {
                Date = DateTime.Now.AddHours(1),
                Title = "Test",
                Notes = "IDK"
            },
        };

        public ObservableCollection<Day> DayPlans
        {
            get => _DayPlans;
            set
            {
                _DayPlans = value;
                OnPropertyChanged();
            }
        }

        public OverviewViewModel()
        {
            OpenMenuCommand = new Command(new Action(openMenu));
        }

        private async void openMenu()
        {
            await VarContainer.menuPage.OpenMenu().ConfigureAwait(false);
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}