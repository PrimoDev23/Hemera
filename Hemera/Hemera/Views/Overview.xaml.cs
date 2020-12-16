using Hemera.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Hemera.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Overview : ContentPage
    {
        private readonly OverviewViewModel viewModel;

        public Overview()
        {
            InitializeComponent();

            viewModel = new OverviewViewModel();
            BindingContext = viewModel;
        }
    }
}