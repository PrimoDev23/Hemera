using Hemera.Helpers;
using Hemera.Models;
using Hemera.Views;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Hemera.ViewModels
{
    public class OverviewViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public Command OpenMenuCommand { get; set; }
        public Command CreateNewCommand { get; set; }
        public Command BackCommand { get; set; }
        public Command ForwardCommand { get; set; }

        private DateTime _CurrentDate = DateTime.Now;

        public DateTime CurrentDate
        {
            get => _CurrentDate;
            set
            {
                _CurrentDate = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(LiteralDate));
                Task.Run(new Action(Order));
            }
        }

        public string LiteralDate
        {
            get => CurrentDate.ToString("ddd dd.MM.yyyy");
        }

        private ObservableCollection<Activity> allActivities;

        private ObservableCollection<Activity> _ActivitiesPerDay;

        public ObservableCollection<Activity> ActivitiesPerDay
        {
            get => _ActivitiesPerDay;
            set
            {
                _ActivitiesPerDay = value;
                OnPropertyChanged();
            }
        }

        private readonly Overview page;

        public OverviewViewModel(Overview page)
        {
            OpenMenuCommand = new Command(new Action(openMenu));
            CreateNewCommand = new Command(new Action(createNewActivity));
            BackCommand = new Command(new Action(dayBack));
            ForwardCommand = new Command(new Action(dayForward));

            void load()
            {
                //Load all Activities
                allActivities = FileHelper.loadActivities();

                //Sort those by time
                ActivitiesPerDay = new ObservableCollection<Activity>(from act in allActivities.getActivitiesPerDay(CurrentDate)
                                                                      orderby act.Date
                                                                      select act);
            }
            Task.Run(new Action(load));

            this.page = page;
        }

        private void dayBack()
        {
            CurrentDate = CurrentDate.AddDays(-1);
        }

        private void dayForward()
        {
            CurrentDate = CurrentDate.AddDays(1);
        }

        private async void openMenu()
        {
            await VarContainer.menuPage.OpenMenu().ConfigureAwait(false);
        }

        private async void createNewActivity()
        {
            NewActivityPopup popup = new NewActivityPopup();
            await page.Navigation.PushModalAsync(popup, false).ConfigureAwait(false);
            Activity res = await popup.waitForFinish().ConfigureAwait(false);

            //User finished popup
            if (res != null)
            {
                await page.Navigation.PopModalAsync().ConfigureAwait(false);

                allActivities.Add(res);

                //Save the newly added activity
                await FileHelper.saveActivities(allActivities).ConfigureAwait(false);
                await Task.Run(new Action(Order)).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Get activities for the selected day and order by time
        /// </summary>
        private void Order()
        {
            ActivitiesPerDay = new ObservableCollection<Activity>(from act in allActivities.getActivitiesPerDay(CurrentDate)
                                                                  orderby act.Date
                                                                  select act);
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}