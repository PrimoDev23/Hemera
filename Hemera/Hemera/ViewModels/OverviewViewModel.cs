using Hemera.Helpers;
using Hemera.Interfaces;
using Hemera.Models;
using Hemera.Resx;
using Hemera.Views;
using System;
using System.Collections.Generic;
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
        public Command<Activity> TappedCommand { get; set; }

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
            TappedCommand = new Command<Activity>(new Action<Activity>(activityTapped));

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
            await page.Navigation.PushModalAsync(popup, true).ConfigureAwait(false);
            Activity res = await popup.waitForFinish().ConfigureAwait(false);

            //User finished popup
            if (res != null)
            {
                await page.Navigation.PopModalAsync().ConfigureAwait(false);

                //Add the new activity to list
                allActivities.Add(res);

                //Save the newly added activity
                await FileHelper.saveActivities(allActivities).ConfigureAwait(false);
                await Task.Run(new Action(Order)).ConfigureAwait(false);

                //Set the notification
                if (res.NotificationTimeType != TimeType.Disabled && DateTime.Compare(res.NotificationDateTime, DateTime.Now) > 0)
                {
                    DependencyService.Get<INotificationManager>().SetupNotifyWork($"{AppResources.PlanedActivity} {res.Date.ToString("t")}", res.Title, res.NotificationDateTime, $"Notify|{res.Title}|{res.Date.ToString("yyyyMMddmmhh")}|{res.CategoryType.ToString()}");
                }

                if (res.DoNotDisturb)
                {
                    DependencyService.Get<INotificationManager>().SetupDNDWork(res.Date, $"DND|{res.Title}|{res.Date.ToString("yyyyMMddmmhh")}|{res.CategoryType.ToString()}");
                }
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

        #region Buttons

        string[] noneButtons = new string[] { AppResources.Delete, AppResources.Edit, AppResources.MarkAsDone, AppResources.MarkAsMissed };
        string[] doneButtons = new string[] { AppResources.ResetStatus, AppResources.MarkAsMissed };
        string[] missedButtons = new string[] { AppResources.ResetStatus, AppResources.MarkAsDone };

        #endregion Buttons

        private async void activityTapped(Activity activity)
        {
            string[] buttons;

            buttons = activity.Status switch
            {
                ActivityStatus.None => noneButtons,
                ActivityStatus.Done => doneButtons,
                ActivityStatus.Missed => missedButtons,
            };

            string res = await page.DisplayActionSheet(AppResources.ChooseOperation, null, null, buttons).ConfigureAwait(false);

            if (res != null)
            {
                if (res == AppResources.Delete)
                {
                    await deleteActivity(activity).ConfigureAwait(false);
                }
                else if (res == AppResources.Edit)
                {
                    await editActivity(activity).ConfigureAwait(false);
                }
                else if (res == AppResources.ResetStatus)
                {
                    activity.Status = ActivityStatus.None;
                    await FileHelper.saveActivities(allActivities).ConfigureAwait(false);
                }
                else if (res == AppResources.MarkAsDone)
                {
                    activity.Status = ActivityStatus.Done;
                    await FileHelper.saveActivities(allActivities).ConfigureAwait(false);
                }
                else if (res == AppResources.MarkAsMissed)
                {
                    activity.Status = ActivityStatus.Missed;
                    await FileHelper.saveActivities(allActivities).ConfigureAwait(false);
                }
            }
        }

        private async Task deleteActivity(Activity activity)
        {
            DependencyService.Get<INotificationManager>().CancelWork($"Notify|{activity.Title}|{activity.Date.ToString("yyyyMMddmmhh")}|{activity.CategoryType.ToString()}");
            DependencyService.Get<INotificationManager>().CancelWork($"DND|{activity.Title}|{activity.Date.ToString("yyyyMMddmmhh")}|{activity.CategoryType.ToString()}");
            allActivities.Remove(activity);

            void orderAndSave()
            {
                Order();
            }
            await Task.Run(new Action(orderAndSave)).ConfigureAwait(false);
            await FileHelper.saveActivities(allActivities).ConfigureAwait(false);
        }

        //Edit activity
        private async Task editActivity(Activity activity)
        {
            //Clone the selected activity
            Activity clone = new Activity()
            {
                Title = activity.Title,
                CategoryType = activity.CategoryType,
                Checklist = activity.Checklist,
                Date = activity.Date,
                Notes = activity.Notes,
                Time = activity.Date.TimeOfDay,
                NotificationTimeType = activity.NotificationTimeType,
                NotificationTime = activity.NotificationTime,
                Position = activity.Position,
                DurationType = activity.DurationType,
                Duration = activity.Duration
            };

            //Open a new popup by using the clone
            NewActivityPopup popup = new NewActivityPopup(clone);

            void push()
            {
                page.Navigation.PushModalAsync(popup, true);
            }
            await Device.InvokeOnMainThreadAsync(new Action(push)).ConfigureAwait(false);
            Activity res = await popup.waitForFinish().ConfigureAwait(false);

            //User finished popup
            if (res != null)
            {
                //Delete the original activity
                deleteActivity(activity);

                await page.Navigation.PopModalAsync().ConfigureAwait(false);

                //Add the new activity to list
                allActivities.Add(res);

                //Save the edited activity
                await FileHelper.saveActivities(allActivities).ConfigureAwait(false);
                await Task.Run(new Action(Order)).ConfigureAwait(false);

                //Set the notification
                if (res.NotificationTimeType != TimeType.Disabled && DateTime.Compare(res.NotificationDateTime, DateTime.Now) > 0)
                {
                    DependencyService.Get<INotificationManager>().SetupNotifyWork($"{AppResources.PlanedActivity} {res.Date.ToString("t")}", res.Title, res.NotificationDateTime, $"Notify|{res.Title}|{res.Date.ToString("yyyyMMddmmhh")}|{res.CategoryType.ToString()}");
                }

                if (res.DoNotDisturb)
                {
                    DependencyService.Get<INotificationManager>().SetupDNDWork(res.Date, $"DND|{res.Title}|{res.Date.ToString("yyyyMMddmmhh")}|{res.CategoryType.ToString()}");
                }
            }
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}