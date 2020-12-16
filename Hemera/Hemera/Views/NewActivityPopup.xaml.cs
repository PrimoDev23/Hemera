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
    public partial class NewActivityPopup : ContentPage
    {
        private readonly NewActivityViewModel viewModel;

        public NewActivityPopup()
        {
            InitializeComponent();

            viewModel = new NewActivityViewModel();
            BindingContext = viewModel;
        }
    }
}