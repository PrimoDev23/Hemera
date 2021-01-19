using Hemera.Helpers;
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
    public partial class StartPage : ContentPage
    {
        private readonly StartPageViewModel viewModel;
        public StartPage()
        {
            InitializeComponent();

            viewModel = new StartPageViewModel(this);
            BindingContext = viewModel;

            holderView.Content = new Overview();

            VarContainer.holderPage = this;
        }
    }
}