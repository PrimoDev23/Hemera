using Hemera.Helpers;
using Hemera.Models;
using Hemera.Resx;
using Hemera.Views;
using Hemera.Views.Popups;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Hemera.ViewModels
{
    public class NewActivityViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public Activity Activity { get; set; }

        private ObservableCollection<Category> _Categories = VarContainer.categories;
        public ObservableCollection<Category> Categories
        {
            get => _Categories;
            set
            {
                _Categories = value;
                OnPropertyChanged();
            }
        }

        private string _Header = "Shopping";
        public string Header
        {
            get => _Header;
            set
            {
                _Header = value;
                OnPropertyChanged();
            }
        }

        private Category CurrentCategory;

        public Command<Category> SelectionChangedCommand { get; set; }
        public Command ClosePopupCommand { get; set; }
        public Command FinishCommand { get; set; }

        private View _SwitchingContent;
        public View SwitchingContent
        {
            get => _SwitchingContent;
            set
            {
                _SwitchingContent = value;
                OnPropertyChanged();
            }
        }

        NewActivityPopup page;
        public NewActivityViewModel(NewActivityPopup page)
        {
            SelectionChangedCommand = new Command<Category>(new Action<Category>(selectionChanged));
            ClosePopupCommand = new Command(new Action(closePopup));
            FinishCommand = new Command(new Action(finishPopup));

            Activity = new Activity();

            void resetSelection()
            {
                selectionChanged(Categories[0]);
            }
            Task.Run(new Action(resetSelection));

            Application.Current.RequestedThemeChanged += Current_RequestedThemeChanged;

            this.page = page;
        }

        private void Current_RequestedThemeChanged(object sender, AppThemeChangedEventArgs e)
        {
            //Update Categories/Reload them
            Categories = VarContainer.categories;
        }

        private async void selectionChanged(Category category)
        {
            Category curr;
            for (int i = 0; i < Categories.Count; i++)
            {
                //Select the tapped item
                curr = Categories[i];

                if(curr.type == category.type)
                {
                    //Select category and update mask
                    curr.selected = true;

                    switch (curr.type)
                    {
                        case CategoryType.Shopping:
                            Header = AppResources.Shopping;

                            //Show a empty item for the first line the user can input
                            Activity.Checklist = new ObservableCollection<ShoppingItem>() { new ShoppingItem() };

                            //Set Activity for curr
                            ((Shopping)curr.view).CurrentActivity = Activity;
                            break;
                        case CategoryType.Sports:
                            Header = AppResources.Sports;

                            //We don't need checklist here
                            Activity.Checklist = null;
                            break;
                        case CategoryType.Meeting:
                            Header = AppResources.Meeting;

                            //We don't need checklist here
                            Activity.Checklist = null;
                            break;
                    }

                    SwitchingContent = curr.view.Content;

                    void UI()
                    {
                        SwitchingContent.BindingContext = Activity;
                        SwitchingContent.Visual = VisualMarker.Material;
                    }
                    await Device.InvokeOnMainThreadAsync(new Action(UI)).ConfigureAwait(false);

                    CurrentCategory = curr;
                }
                else
                {
                    //Deselect category
                    curr.selected = false;
                }
            }
        }

        private void closePopup()
        {
            page.addingSuccessful.TrySetResult(null);
            page.Navigation.PopModalAsync();
        }

        private void finishPopup()
        {
            //TODO: Check if need information is filled

            //Needed information isn't filled
            if(!(Activity.Title?.Length > 0) || DateTime.Compare(Activity.Date, DateTime.Now) < 0)
            {
                return;
            }

            //If current type is shopping, check if the checklist is empty
            //and clear it if needed, to save some storage later when we use XML to save stuff
            if (CurrentCategory.type == CategoryType.Shopping && Activity.Checklist.Count == 1 && Activity.Checklist[0].ItemName?.Length > 0)
            {
                Activity.Checklist = null;
            }

            //TODO: Create notification for date and time
            //TODO: Save into XML

            page.addingSuccessful.TrySetResult(Activity);
            page.Navigation.PopModalAsync();
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
