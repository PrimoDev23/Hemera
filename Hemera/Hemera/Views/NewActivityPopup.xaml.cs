using Hemera.Models;
using Hemera.ViewModels;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Hemera.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NewActivityPopup : ContentPage
    {
        private readonly NewActivityViewModel viewModel;

        public NewActivityPopup()
        {
            InitializeComponent();

            viewModel = new NewActivityViewModel(this);
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
    }
}