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

namespace Hemera.ViewModels
{
    public class OverviewViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        #region Commands

        public Command BackCommand { get; set; }
        public Command ForwardCommand { get; set; }
        public Command<Activity> OpenSelectionMenuCommand { get; set; }
        public Command<Activity> TappedCommand { get; set; }

        #endregion Commands

        #region Date

        private DateTime _CurrentDate = DateTime.Now;

        public DateTime CurrentDate
        {
            get => _CurrentDate;
            set
            {
                _CurrentDate = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(LiteralDate));
                Task.Run(new Action(getActivitiesPerDay));
            }
        }

        public string LiteralDate
        {
            get => CurrentDate.ToString("ddd dd.MM.yyyy");
        }

        #endregion Date

        //Activities for the selected date
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

            //Get the activities for today
            Task.Run(new Action(getActivitiesPerDay));

            VarContainer.currentOverviewModel = this;

            this.page = page;
        }

        #region DateMoving

        /// <summary>
        /// Go one day back
        /// </summary>
        private void dayBack()
        {
            CurrentDate = CurrentDate.AddDays(-1);
        }

        /// <summary>
        /// Go one day forward
        /// </summary>
        private void dayForward()
        {
            CurrentDate = CurrentDate.AddDays(1);
        }

        #endregion DateMoving

        /// <summary>
        /// Get activities for the selected day and order by time
        /// </summary>
        private void getActivitiesPerDay()
        {
            ActivitiesPerDay = new ObservableCollection<Activity>(from act in VarContainer.allActivities.getActivitiesPerDay(CurrentDate)
                                                                  orderby act.Date
                                                                  select act);
        }

        #region Buttons

        private readonly string[] noneButtons = new string[] { AppResources.Delete, AppResources.Edit, AppResources.MarkAsDone, AppResources.MarkAsMissed };
        private readonly string[] doneButtons = new string[] { AppResources.ResetStatus, AppResources.MarkAsMissed };
        private readonly string[] missedButtons = new string[] { AppResources.ResetStatus, AppResources.MarkAsDone };

        #endregion Buttons

        #region ActivityHandling

        /// <summary>
        /// Create a new activity
        /// </summary>
        /// <returns></returns>
        public async Task createNewActivity()
        {
            //Show NewActivityPopup
            NewActivityPopup popup = new NewActivityPopup();
            await page.Navigation.PushModalAsync(popup, true).ConfigureAwait(false);

            //Wait for the user to complete the NewActivityPopup
            Activity res = await popup.waitForFinish().ConfigureAwait(false);

            //User finished popup
            if (res != null)
            {
                //Pop popup
                await page.Navigation.PopModalAsync().ConfigureAwait(false);

                //Add the new activity to list
                VarContainer.allActivities.Add(res);

                //Save the newly added activity
                await FileHelper.saveActivities(VarContainer.allActivities).ConfigureAwait(false);
                await Task.Run(new Action(getActivitiesPerDay)).ConfigureAwait(false);

                NotifyHelper.CreateNotification(res);
            }
        }

        /// <summary>
        /// Open the menu for an activity
        /// </summary>
        /// <param name="activity">Activity to open the menu for</param>
        private async void openSelectionMenu(Activity activity)
        {
            //Get the buttons, according to the current activity state
            string[] buttons = activity.Status switch
            {
                ActivityStatus.None => noneButtons,
                ActivityStatus.Done => doneButtons,
                ActivityStatus.Missed => missedButtons,
                _ => throw new NotImplementedException(),
            };

            //Display menu
            string res = await VarContainer.holderPage.DisplayActionSheet(AppResources.ChooseOperation, null, null, buttons).ConfigureAwait(false);

            //According to the users choice execute the operation
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
                    NotifyHelper.CreateNotification(activity);
                    await FileHelper.saveActivities(VarContainer.allActivities).ConfigureAwait(false);
                }
                else if (res == AppResources.MarkAsDone)
                {
                    activity.Status = ActivityStatus.Done;
                    NotifyHelper.DeleteNotification(activity);
                    await FileHelper.saveActivities(VarContainer.allActivities).ConfigureAwait(false);
                }
                else if (res == AppResources.MarkAsMissed)
                {
                    activity.Status = ActivityStatus.Missed;
                    NotifyHelper.DeleteNotification(activity);
                    await FileHelper.saveActivities(VarContainer.allActivities).ConfigureAwait(false);
                }
            }
        }

        /// <summary>
        /// User tapped an activity
        /// </summary>
        /// <param name="activity">Activity to open detailpage for</param>
        private async void activityTapped(Activity activity)
        {
            //Open the activity in the detailview
            DetailView detail = new DetailView(this, activity);
            await page.Navigation.PushAsync(detail).ConfigureAwait(false);
        }

        /// <summary>
        /// Delete the given activity
        /// </summary>
        /// <param name="activity">Activity to delete</param>
        /// <returns></returns>
        private async Task deleteActivity(Activity activity)
        {
            NotifyHelper.DeleteNotification(activity);

            //Remove the activity
            VarContainer.allActivities.Remove(activity);

            //Remove Attachments and by CurrentDate
            void RemoveAndOrder()
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
                getActivitiesPerDay();
            }
            await Task.Run(new Action(RemoveAndOrder)).ConfigureAwait(false);

            //Save all activities
            await FileHelper.saveActivities(VarContainer.allActivities).ConfigureAwait(false);
        }

        /// <summary>
        /// Edit the selected activity
        /// </summary>
        /// <param name="activity">Activity to edit</param>
        /// <returns></returns>
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

            //Push the NewActivityPopup
            void push()
            {
                page.Navigation.PushModalAsync(popup, true);
            }
            await Device.InvokeOnMainThreadAsync(new Action(push)).ConfigureAwait(false);

            //Wait for the user to finish
            Activity res = await popup.waitForFinish().ConfigureAwait(false);

            //User finished popup
            if (res != null)
            {
                //Delete the original activity
                await deleteActivity(activity).ConfigureAwait(false);

                //Pop popup
                await page.Navigation.PopModalAsync().ConfigureAwait(false);

                //Add the new activity to list
                VarContainer.allActivities.Add(res);

                //Save the edited activity
                await FileHelper.saveActivities(VarContainer.allActivities).ConfigureAwait(false);
                await Task.Run(new Action(getActivitiesPerDay)).ConfigureAwait(false);

                //Set the notification
                if (res.NotificationTimeType != TimeType.Disabled && DateTime.Compare(res.NotificationDateTime, DateTime.Now) > 0)
                {
                    DependencyService.Get<INotificationManager>().SetupNotifyWork($"{AppResources.PlanedActivity} {res.Date.ToString("t")}", res.Title, res.NotificationDateTime, $"Notify|{res.Title}|{res.Date.ToString("yyyyMMddmmhh")}|{res.CategoryType.ToString()}");
                }

                //Set DND at specified time
                if (res.DoNotDisturb)
                {
                    DependencyService.Get<INotificationManager>().SetupDNDWork(res.Date, $"DND|{res.Title}|{res.Date.ToString("yyyyMMddmmhh")}|{res.CategoryType.ToString()}");
                }
            }
        }

        #endregion ActivityHandling

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}