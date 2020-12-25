using Hemera.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using static Hemera.Models.Activity;

namespace Hemera.Views.Popups
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NotificationPopup : ContentPage
    {
        private Activity activity;

        private TaskCompletionSource<bool> completed;

        private TimeType timeType;

        public NotificationPopup(Activity activity)
        {
            InitializeComponent();

            completed = new TaskCompletionSource<bool>();
            this.activity = activity;

            //Setup the initial state of radiobuttons
            switch (activity.TimeType)
            {
                case TimeType.Minute:
                    radio_minutes.IsChecked = true;
                    break;
                case TimeType.Hour:
                    radio_hours.IsChecked = true;
                    break;
                case TimeType.Day:
                    radio_days.IsChecked = true;
                    break;
                case TimeType.Disabled:
                    radio_disable.IsChecked = true;
                    break;
            }
        }

        public async Task WaitForFinish()
        {
            //Wait until user cancels input or user finishes the popup
            if (await completed?.Task)
            {
                activity.TimeType = timeType;

                if (timeType != TimeType.Disabled)
                {
                    activity.NotificationTime = uint.Parse(txt_time.Text);
                }
            }
        }

        private void radio_minutes_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            timeType = TimeType.Minute;
            txt_time.IsEnabled = true;

            //If this gets selected and time isn't valid make button disabled
            //and show the error label
            if (timeInvalid)
            {
                btn_done.IsEnabled = false;
                lbl_timeneeded.HeightRequest = 20;
            }
        }

        private void radio_hours_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            timeType = TimeType.Hour;
            txt_time.IsEnabled = true;

            //If this gets selected and time isn't valid make button disabled
            //and show the error label
            if (timeInvalid)
            {
                btn_done.IsEnabled = false;
                lbl_timeneeded.HeightRequest = 20;
            }
        }

        private void radio_days_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            timeType = TimeType.Day;
            txt_time.IsEnabled = true;

            //If this gets selected and time isn't valid make button disabled
            //and show the error label
            if (timeInvalid)
            {
                btn_done.IsEnabled = false;
                lbl_timeneeded.HeightRequest = 20;
            }
        }

        private void radio_disable_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            timeType = TimeType.Disabled;
            txt_time.IsEnabled = false;

            //We don't need time here anyways so just enable it
            btn_done.IsEnabled = true;
            lbl_timeneeded.HeightRequest = 0;
        }

        private bool timeInvalid = false;

        private void txt_time_TextChanged(object sender, TextChangedEventArgs e)
        {
            //If user cleared the field we show a warning
            //We can't get in here if type is disabled
            if (!(e.NewTextValue?.Length > 0))
            {
                lbl_timeneeded.HeightRequest = 20;
                timeInvalid = true;
                btn_done.IsEnabled = false;
                return;
            }

            //Instead don't let the user enter invalid values (Text isn't a valid uint)
            if (!uint.TryParse(e.NewTextValue, out _))
            {
                ((Entry)sender).Text = e.OldTextValue;
                return;
            }

            //Valid input
            timeInvalid = false;
            lbl_timeneeded.HeightRequest = 0;
            btn_done.IsEnabled = true;
        }

        private void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            completed.TrySetResult(false);
        }

        private void btn_done_Clicked(object sender, EventArgs e)
        {
            completed.TrySetResult(true);
        }
    }
}