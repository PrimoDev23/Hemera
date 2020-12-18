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

        public TaskCompletionSource<Activity> addingSuccessful;

        public NewActivityPopup()
        {
            InitializeComponent();

            addingSuccessful = new TaskCompletionSource<Activity>();

            viewModel = new NewActivityViewModel(this);
            BindingContext = viewModel;
        }

        public Task<Activity> waitForFinish()
        {
            return addingSuccessful.Task;
        }
    }
}