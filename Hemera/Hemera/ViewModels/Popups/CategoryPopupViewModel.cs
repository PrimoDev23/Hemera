using Hemera.Helpers;
using Hemera.Models;
using Hemera.Views.Popups;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using Xamarin.Forms;

namespace Hemera.ViewModels.Popups
{
    public class CategoryPopupViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public Command DoneCommand { get; set; }
        public Command AbortCommand { get; set; }

        public ObservableCollection<Category> Categories { get; set; } = VarContainer.categories;

        int selectedCategoryIndex;

        CategoryPopup popup;
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

        private void done()
        {
            Category curr;

            //Set the selected category
            for (int i = 0; i < Categories.Count; i++)
            {
                curr = Categories[i];
                if (curr.Selected)
                {
                    popup.categorySelected.TrySetResult(curr);
                }
            }
        }

        private void abort()
        {
            //Prevents changing the selected radiobutton on abort
            for (int i = 0; i < Categories.Count; i++)
            {
                if(i == selectedCategoryIndex)
                {
                    Categories[i].Selected = true;
                }
                else
                {
                    Categories[i].Selected = false;
                }
            }

            popup.categorySelected.TrySetResult(null);
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
