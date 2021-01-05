using Android.App;
using Android.Content;
using Hemera.Droid.DependencyService;
using Hemera.Interfaces;
using Xamarin.Forms;
using AndroidApp = Android.App.Application;

[assembly: Dependency(typeof(DND))]
namespace Hemera.Droid.DependencyService
{
    public class DND : IDND
    {
        public void AskPermission()
        {
            Intent intent = new Intent("android.settings.NOTIFICATION_POLICY_ACCESS_SETTINGS");
            intent.AddFlags(ActivityFlags.NewTask);
            AndroidApp.Context.StartActivity(intent);
        }

        public bool CheckForPermission()
        {
            NotificationManager manager = (NotificationManager)AndroidApp.Context.GetSystemService(AndroidApp.NotificationService);

            return manager.IsNotificationPolicyAccessGranted;
        }
    }
}