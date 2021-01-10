using Android;
using Android.Content.PM;
using Android.Media;
using Android.Support.V4.App;
using Android.Support.V4.Content;
using Hemera.Droid.DependencyService;
using Hemera.Interfaces;
using Xamarin.Forms;
using AndroidApp = Android.App.Application;

[assembly: Dependency(typeof(AudioHelper))]
namespace Hemera.Droid.DependencyService
{
    public class AudioHelper : IAudio
    {
        public static MediaRecorder recorder;

        public bool checkPermission()
        {
            if (ContextCompat.CheckSelfPermission(AndroidApp.Context, Manifest.Permission.RecordAudio) != Permission.Granted)
            {
                ActivityCompat.RequestPermissions(MainActivity.Instance, new string[]
                {
                    Manifest.Permission.RecordAudio
                }, 12345);

                //We gonna return here, since Request gets executed async
                return false;
            }
            return true;
        }

        public bool startRecord(string filePath)
        {
            if (recorder != null)
            {
                return false;
            }

            recorder = new MediaRecorder();
            recorder.SetAudioSource(AudioSource.Mic);
            recorder.SetOutputFormat(OutputFormat.ThreeGpp);
            recorder.SetAudioEncoder(AudioEncoder.AmrNb);
            recorder.SetOutputFile(filePath);
            recorder.Prepare();
            recorder.Start();

            return true;
        }

        public void stopRecord()
        {
            if (recorder == null)
            {
                return;
            }

            recorder.Stop();
            recorder.Reset();
            recorder.Release();
            recorder = null;
        }
    }
}