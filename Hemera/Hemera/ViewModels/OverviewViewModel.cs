using Hemera.Helpers;
using Hemera.Models;
using Hemera.Views;
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
        public Command CreateNewCommand { get; set; }

        private ObservableCollection<Activity> _DayPlans = new ObservableCollection<Activity>()
        {
            new Activity()
            {
                Date = DateTime.Now.AddHours(1),
                Title = "Test",
                Notes = "IDK"
            },
            new Activity()
            {
                Date = DateTime.Now.AddHours(1),
                Title = "Test",
                Notes = "IDK"
            },
            new Activity()
            {
                Date = DateTime.Now.AddHours(1),
                Title = "Test",
                Notes = "IDK"
            },
            new Activity()
            {
                Date = DateTime.Now.AddHours(1),
                Title = "Test",
                Notes = "IDK"
            },
            new Activity()
            {
                Date = DateTime.Now.AddHours(1),
                Title = "Test",
                Notes = "IDK"
            },
        };

        public ObservableCollection<Activity> DayPlans
        {
            get => _DayPlans;
            set
            {
                _DayPlans = value;
                OnPropertyChanged();
            }
        }

        private readonly Overview page;
        public OverviewViewModel(Overview page)
        {
            OpenMenuCommand = new Command(new Action(openMenu));
            CreateNewCommand = new Command(new Action(createNewActivity));
            this.page = page;
        }

        private async void openMenu()
        {
            await VarContainer.menuPage.OpenMenu().ConfigureAwait(false);
        }

        private void createNewActivity()
        {
            page.Navigation.PushModalAsync(new NewActivityPopup());
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}