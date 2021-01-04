using Hemera.Interfaces;
using Hemera.Models;
using Hemera.Resx;
using Hemera.ViewModels;
using System;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Xaml;

namespace Hemera.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NewActivityPopup : ContentPage
    {
        private readonly NewActivityViewModel viewModel;

        public bool timeValid = true;

        public NewActivityPopup()
        {
            InitializeComponent();

            viewModel = new NewActivityViewModel(this);
            BindingContext = viewModel;
        }

        public NewActivityPopup(Activity activity)
        {
            InitializeComponent();

            viewModel = new NewActivityViewModel(this, activity);
            BindingContext = viewModel;
        }

        public Task<Activity> waitForFinish()
        {
            //Return the TaskCompletitionSource so we can await it
            return viewModel.newActivityCompleted.Task;
        }

        protected override bool OnBackButtonPressed()
        {
            //We aborted creating a new Activity, so set the result
            viewModel.newActivityCompleted.TrySetResult(null);

            //And use the normal back action
            return base.OnBackButtonPressed();
        }

        private void txt_notificationTime_TextChanged(object sender, TextChangedEventArgs e)
        {
            if(!(e.NewTextValue?.Length > 0))
            {
                viewModel.NotifyTimeInvalid = true;
                return;
            }
            
            if (e.NewTextValue.StartsWith("-"))
            {
                //Prevent user from entering minus
                txt_notificationTime.Text = e.OldTextValue;
                return;
            }

            viewModel.NotifyTimeInvalid = false;
        }

        private void Switch_Toggled(object sender, ToggledEventArgs e)
        {
            //If switch is set to false the entered time doesn't matter
            if (!e.Value)
            {
                viewModel.NotifyTimeInvalid = false;
            }
            else
            {
                //It can only be valid or empty here so it doesn't really matter that old and newtext are different
                txt_notificationTime_TextChanged(txt_notificationTime, new TextChangedEventArgs(txt_notificationTime.Text, txt_notificationTime.Text));
            }
        }

        private void txt_title_TextChanged(object sender, TextChangedEventArgs e)
        {
            //If title isn't empty it's valid
            if (e.NewTextValue?.Length > 0)
            {
                viewModel.titleInvalid = false;
            }
            else //It isn't
            {
                viewModel.titleInvalid = true;
            }
        }

        private void map_MapClicked(object sender, Xamarin.Forms.Maps.MapClickedEventArgs e)
        {
            map.Pins.Clear();
            map.Pins.Add(new Pin()
            {
                Position = e.Position,
                Label = AppResources.SelectedPosition
            });

            viewModel.Activity.Position = e.Position;
        }

        private void switch_dnd_Toggled(object sender, ToggledEventArgs e)
        {
            //Check if permission is given
            if (e.Value && !DependencyService.Get<IDND>().CheckForPermission())
            {
                //If Permission is not given ask for it and reset the switch
                DependencyService.Get<IDND>().AskPermission();
                viewModel.Activity.DoNotDisturb = false;
            }
        }

        private void txt_duration_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!(e.NewTextValue?.Length > 0))
            {
                viewModel.DurationInvalid = true;
                return;
            }

            if (e.NewTextValue.StartsWith("-"))
            {
                //Prevent user from entering minus
                txt_duration.Text = e.OldTextValue;
                return;
            }

            viewModel.DurationInvalid = false;
        }
    }
}