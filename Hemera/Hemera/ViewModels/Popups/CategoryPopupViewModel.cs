using Hemera.Helpers;
using Hemera.Models;
using Hemera.Views.Popups;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Xamarin.Forms;

namespace Hemera.ViewModels.Popups
{
    public class CategoryPopupViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        #region Commands

        public Command DoneCommand { get; set; }
        public Command AbortCommand { get; set; }

        #endregion Commands

        public ObservableCollection<Category> Categories { get; set; } = VarContainer.categories;

        private readonly int selectedCategoryIndex;

        private readonly CategoryPopup popup;
        public CategoryPopupViewModel(CategoryPopup popup)
        {
            DoneCommand = new Command(new Action(done));
            AbortCommand = new Command(new Action(abort));

            //Save old index to make sure we reset it on abort
            for (int i = 0; i < Categories.Count; i++)
            {
                if (Categories[i].Selected)
                {
                    selectedCategoryIndex = i;
                    break;
                }
            }

            this.popup = popup;
        }

        /// <summary>
        /// Complete the current popup
        /// </summary>
        private void done()
        {
            Category curr;

            //Set the selected category
            for (int i = 0; i < Categories.Count; i++)
            {
                curr = Categories[i];
                if (curr.Selected)
                {
                    //And finish the TaskCompletitionSource with selected category
                    popup.categorySelected.TrySetResult(curr);
                }
            }
        }

        /// <summary>
        /// Close the popup
        /// </summary>
        private void abort()
        {
            //Prevents changing the selected radiobutton on abort
            for (int i = 0; i < Categories.Count; i++)
            {
                Categories[i].Selected = i == selectedCategoryIndex;
            }

            popup.categorySelected.TrySetResult(null);
        }
    }
}
