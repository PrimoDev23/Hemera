using Hemera.ViewModels;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Hemera.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Overview : ContentView
    {
        private readonly OverviewViewModel viewModel;

        public Overview()
        {
            InitializeComponent();

            viewModel = new OverviewViewModel(this);
            BindingContext = viewModel;
        }
    }
}