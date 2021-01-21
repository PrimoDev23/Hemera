using Hemera.ViewModels.Popups;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Hemera.Views.Popups
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SelectDatePopup : ContentPage
    {
        private readonly SelectDatePopupViewModel viewModel;
        public SelectDatePopup(DateTime minDate, DateTime maxDate)
        {
            InitializeComponent();

            viewModel = new SelectDatePopupViewModel(minDate, maxDate);
            BindingContext = viewModel;
        }

        public Task<(DateTime, DateTime)> waitTillFinish()
        {
            return viewModel.completed.Task;
        }
    }
}