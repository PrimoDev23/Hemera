using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using AndroidX.Core.App;
using AndroidX.Work;
using Hemera.Droid.DependencyService;
using Hemera.Interfaces;
using System;
using Xamarin.Forms;
using AndroidApp = Android.App.Application;

[assembly: Dependency(typeof(AndroidNotificationManager))]
namespace Hemera.Droid.DependencyService
{
    public class AndroidNotificationManager : INotificationManager
    {
        private const string channelId = "HemeraChannel";
        private const string channelName = "HemeraChannel";
        private const string channelDescription = "The default channel for notifications by Hemera.";

        public const string TitleKey = "title";
        public const string MessageKey = "message";
        private bool channelInitialized = false;
        private int messageId = 0;
        private int pendingIntentId = 0;
        private NotificationManager manager;

        public void SetupNotifyWork(string title, string message, DateTime date, string name)
        {
            var data = new Data.Builder();
            data.PutString("Type", "Notify");
            data.PutString("Title", title);
            data.PutString("Message", message);

            var work = OneTimeWorkRequest.Builder.From<NotificationWorkManager>()
                .SetInitialDelay(date.Subtract(DateTime.Now))
                .SetInputData(data.Build())
                .Build();
            
            WorkManager.Instance.EnqueueUniqueWork(name, ExistingWorkPolicy.Keep, work);
        }

        public void SetupDNDWork(DateTime date, string name)
        {
            var data = new Data.Builder();
            data.PutString("Type", "DND");

            var work = OneTimeWorkRequest.Builder.From<NotificationWorkManager>()
                .SetInitialDelay(date.Subtract(DateTime.Now))
                .SetInputData(data.Build())
                .Build();
            
            WorkManager.Instance.EnqueueUniqueWork(name, ExistingWorkPolicy.Keep, work);
        }

        public void Show(string title, string message)
        {
            if (!channelInitialized)
            {
                CreateNotificationChannel();
            }

            Intent intent = new Intent(AndroidApp.Context, typeof(MainActivity));
            intent.PutExtra(TitleKey, title);
            intent.PutExtra(MessageKey, message);

            PendingIntent pendingIntent = PendingIntent.GetActivity(AndroidApp.Context, pendingIntentId++, intent, PendingIntentFlags.UpdateCurrent);

            NotificationCompat.Builder builder = new NotificationCompat.Builder(AndroidApp.Context, channelId)
                .SetContentIntent(pendingIntent)
                .SetContentTitle(title)
                .SetContentText(message)
                .SetLargeIcon(BitmapFactory.DecodeResource(AndroidApp.Context.Resources, Resource.Drawable.calendar_black))
                .SetSmallIcon(Resource.Drawable.calendar_black)
                .SetDefaults((int)NotificationDefaults.Sound | (int)NotificationDefaults.Vibrate);

            Notification notification = builder.Build();
            manager.Notify(messageId++, notification);
        }

        public void enableDND()
        {
            NotificationManager manager = (NotificationManager)AndroidApp.Context.GetSystemService(AndroidApp.NotificationService);
            manager.SetInterruptionFilter(InterruptionFilter.Priority);
        }

        private void CreateNotificationChannel()
        {
            manager = (NotificationManager)AndroidApp.Context.GetSystemService(AndroidApp.NotificationService);

            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                var channelNameJava = new Java.Lang.String(channelName);
                var channel = new NotificationChannel(channelId, channelNameJava, NotificationImportance.Default)
                {
                    Description = channelDescription
                };
                manager.CreateNotificationChannel(channel);
            }

            channelInitialized = true;
        }

        public void CancelWork(string name)
        {
            WorkManager.Instance.CancelUniqueWork(name);
        }
    }
}