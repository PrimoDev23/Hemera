using Hemera.Models;
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
    public partial class CategoryPopup : ContentPage
    {
        CategoryPopupViewModel viewModel;

        public TaskCompletionSource<Category> categorySelected;

        public CategoryPopup()
        {
            InitializeComponent();

            viewModel = new CategoryPopupViewModel(this);
            BindingContext = viewModel;

            categorySelected = new TaskCompletionSource<Category>();
        }

        public Task<Category> CategorySelected()
        {
            return categorySelected.Task;
        }
    }
}