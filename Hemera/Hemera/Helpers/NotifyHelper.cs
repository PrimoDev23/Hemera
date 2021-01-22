using Hemera.Interfaces;
using Hemera.Models;
using Hemera.Resx;
using System;
using Xamarin.Forms;

namespace Hemera.Helpers
{
    public static class NotifyHelper
    {
        /// <summary>
        /// Create notification for the given activity
        /// </summary>
        /// <param name="activity">Activity to create notification for</param>
        public static void CreateNotification(Activity activity)
        {
            //Set the notification
            if (activity.NotificationTimeType != TimeType.Disabled && DateTime.Compare(activity.NotificationDateTime, DateTime.Now) > 0)
            {
                DependencyService.Get<INotificationManager>().SetupNotifyWork($"{AppResources.PlanedActivity} {activity.Date.ToString("t")}", activity.Title, activity.NotificationDateTime, $"Notify|{activity.Title}|{activity.Date.ToString("yyyyMMddmmhh")}|{activity.CategoryType.ToString()}");
            }

            //Set DND at the specified time
            if (activity.DoNotDisturb)
            {
                DependencyService.Get<INotificationManager>().SetupDNDWork(activity.Date, $"DND|{activity.Title}|{activity.Date.ToString("yyyyMMddmmhh")}|{activity.CategoryType.ToString()}");
            }
        }

        /// <summary>
        /// Delete notification for the given activity
        /// </summary>
        /// <param name="activity">Activity to delete notification for</param>
        public static void DeleteNotification(Activity activity)
        {
            //Cancel the work for the given activity
            DependencyService.Get<INotificationManager>().CancelWork($"Notify|{activity.Title}|{activity.Date.ToString("yyyyMMddmmhh")}|{activity.CategoryType.ToString()}");
            DependencyService.Get<INotificationManager>().CancelWork($"DND|{activity.Title}|{activity.Date.ToString("yyyyMMddmmhh")}|{activity.CategoryType.ToString()}");
        }
    }
}
