
using Hemera.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Hemera.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SideBar : ContentPage
    {
        private readonly SideBarViewModel viewModel;
        public SideBar()
        {
            InitializeComponent();

            viewModel = new SideBarViewModel();
            BindingContext = viewModel;
        }
    }
}