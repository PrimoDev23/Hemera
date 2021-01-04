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
                AndroidNotificationManager manager = new AndroidNotificationManager();

                //Notification
                if (InputData.GetString("Type") == "Notify")
                {
                    manager.Show(InputData.GetString("Title"), InputData.GetString("Message"));
                    return Result.InvokeSuccess();
                }
                else //DND
                {
                    manager.enableDND();
                    return Result.InvokeSuccess();
                }
            }catch
            {
                return Result.InvokeFailure();
            }
        }
    }
}