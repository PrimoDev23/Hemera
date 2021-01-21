using Hemera.ViewModels;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Hemera.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ChartView : ContentView
    {
        private readonly ChartViewModel viewModel;
        public ChartView()
        {
            InitializeComponent();

            viewModel = new ChartViewModel();
            BindingContext = viewModel;
        }
    }
}