using Android.Content;
using Android.Runtime;
using AndroidX.Work;
using System;

namespace Hemera.Droid.DependencyService
{
    public class NotificationWorkManager : Worker
    {
        protected NotificationWorkManager(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        public NotificationWorkManager(Context context, WorkerParameters workerParams) : base(context, workerParams)
        {
        }

        public override Result DoWork()
        {
            try
            {
                //Notification
                if (InputData.GetString("Type") == "Notify")
                {
                    new AndroidNotificationManager().Show(InputData.GetString("Title"), InputData.GetString("Message"));
                    return Result.InvokeSuccess();
                }
                else //DND
                {
                    AndroidNotificationManager.EnableDND();
                    return Result.InvokeSuccess();
                }
            }catch
            {
                return Result.InvokeFailure();
            }
        }
    }
}