using Hemera.Models;
using Hemera.Resx;
using Hemera.ViewModels;
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
            viewModel.CheckTimeValid(e);
        }

        private void Switch_Toggled(object sender, ToggledEventArgs e)
        {
            //If switch is set to false the entered time doesn't matter
            if (!e.Value)
            {
                viewModel.timeValid = true;
            }

            btn_done.IsEnabled = viewModel.timeValid && viewModel.titleValid;
        }

        private void txt_title_TextChanged(object sender, TextChangedEventArgs e)
        {
            //If title is empty it's invalid
            if (e.NewTextValue?.Length > 0)
            {
                viewModel.titleValid = true;
            }
            else
            {
                viewModel.titleValid = false;
            }

            btn_done.IsEnabled = viewModel.timeValid && viewModel.titleValid;
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
    }
}