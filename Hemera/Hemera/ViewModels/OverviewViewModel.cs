using Hemera.Helpers;
using Hemera.Interfaces;
using Hemera.Models;
using Hemera.Resx;
using Hemera.Views;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Xamarin.Forms;
using MenuItem = Hemera.Models.MenuItem;

namespace Hemera.ViewModels
{
    public class OverviewViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public Command BackCommand { get; set; }
        public Command ForwardCommand { get; set; }
        public Command<Activity> OpenSelectionMenuCommand { get; set; }
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
            BackCommand = new Command(new Action(dayBack));
            ForwardCommand = new Command(new Action(dayForward));
            OpenSelectionMenuCommand = new Command<Activity>(new Action<Activity>(openSelectionMenu));
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

            VarContainer.currentOverviewModel = this;

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

        private readonly string[] noneButtons = new string[] { AppResources.Delete, AppResources.Edit, AppResources.MarkAsDone, AppResources.MarkAsMissed };
        private readonly string[] doneButtons = new string[] { AppResources.ResetStatus, AppResources.MarkAsMissed };
        private readonly string[] missedButtons = new string[] { AppResources.ResetStatus, AppResources.MarkAsDone };

        #endregion Buttons

        public async Task createNewActivity()
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

        private async void openSelectionMenu(Activity activity)
        {
            string[] buttons = activity.Status switch
            {
                ActivityStatus.None => noneButtons,
                ActivityStatus.Done => doneButtons,
                ActivityStatus.Missed => missedButtons,
                _ => throw new NotImplementedException(),
            };

            string res = await VarContainer.holderPage.DisplayActionSheet(AppResources.ChooseOperation, null, null, buttons).ConfigureAwait(false);

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

        private async void activityTapped(Activity activity)
        {
            DetailView detail = new DetailView(this, activity);
            await page.Navigation.PushAsync(detail).ConfigureAwait(false);
        }

        public async Task saveFromOuter()
        {
            await FileHelper.saveActivities(allActivities).ConfigureAwait(false);
        }

        private async Task deleteActivity(Activity activity)
        {
            DependencyService.Get<INotificationManager>().CancelWork($"Notify|{activity.Title}|{activity.Date.ToString("yyyyMMddmmhh")}|{activity.CategoryType.ToString()}");
            DependencyService.Get<INotificationManager>().CancelWork($"DND|{activity.Title}|{activity.Date.ToString("yyyyMMddmmhh")}|{activity.CategoryType.ToString()}");
            allActivities.Remove(activity);

            void orderAndSave()
            {
                if (activity.Attachments?.Count > 0)
                {
                    string path;
                    for (int i = 0; i < activity.Attachments.Count; i++)
                    {
                        path = activity.Attachments[i].File.FullPath;
                        if (File.Exists(path))
                        {
                            File.Delete(path);
                        }
                    }
                }
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
                await deleteActivity(activity).ConfigureAwait(false);

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