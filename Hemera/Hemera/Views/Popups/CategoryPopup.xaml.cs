using Hemera.Models;
using Hemera.ViewModels.Popups;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Hemera.Views.Popups
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CategoryPopup : ContentPage
    {
        private readonly CategoryPopupViewModel viewModel;

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