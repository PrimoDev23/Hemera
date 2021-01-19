using Hemera.Helpers;
using Hemera.Models;
using Hemera.ViewModels;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Hemera.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DetailView : ContentPage
    {
        private readonly DetailViewViewModel viewModel;
        private readonly OverviewViewModel overviewViewModel;
        public DetailView(OverviewViewModel overviewViewModel, Activity activity)
        {
            InitializeComponent();

            this.overviewViewModel = overviewViewModel;

            viewModel = new DetailViewViewModel(this, activity);
            BindingContext = viewModel;
        }

        protected override void OnDisappearing()
        {
            //We need to save the current activity on disappear if it gets killed (useful with Checklist)
            FileHelper.saveActivities(VarContainer.allActivities);

            base.OnDisappearing();
        }
    }
}