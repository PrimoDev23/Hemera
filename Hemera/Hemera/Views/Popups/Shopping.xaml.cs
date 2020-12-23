using Hemera.Interfaces;
using Hemera.Models;
using Hemera.ViewModels.Popups;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Hemera.Views.Popups
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Shopping : ContentView, IValidate
    {
        public ShoppingViewModel viewModel;

        public Shopping()
        {
            InitializeComponent();

            viewModel = new ShoppingViewModel(this);
            BindingContext = viewModel;
        }

        public async Task<bool> ValidateInput()
        {
            if (!(viewModel.Activity?.Title?.Length > 0))
            {
                void setHeight()
                {
                    lbl_titlemissing.HeightRequest = 20;
                }
                await Device.InvokeOnMainThreadAsync(new Action(setHeight)).ConfigureAwait(false);
                return false;
            }
            else
            {
                void setHeight()
                {
                    lbl_titlemissing.HeightRequest = 0;
                }
                await Device.InvokeOnMainThreadAsync(new Action(setHeight)).ConfigureAwait(false);
            }

            return true;
        }
    }
}