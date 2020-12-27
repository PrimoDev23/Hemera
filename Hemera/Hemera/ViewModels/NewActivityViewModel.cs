using Hemera.Helpers;
using Hemera.Interfaces;
using Hemera.Models;
using Hemera.Resx;
using Hemera.Views;
using Hemera.Views.Popups;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Hemera.ViewModels
{
    public class NewActivityViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public TaskCompletionSource<Activity> newActivityCompleted;

        private string _Header = AppResources.Shopping;
        public string Header
        {
            get => _Header;
            set
            {
                _Header = value;
                OnPropertyChanged();
            }
        }

        public Activity Activity { get; set; }

        public ObservableCollection<Category> Categories { get; set; } = VarContainer.categories;

        private Category _CurrentCategory = VarContainer.categories[0];

        public Category CurrentCategory
        {
            get => _CurrentCategory;
            set
            {
                _CurrentCategory = value;
                OnPropertyChanged();
            }
        }

        public Command<Category> ItemSelectedCommand { get; set; }
        public Command FinishCommand { get; set; }
        public Command NotificationCommand { get; set; }

        public NewActivityPopup page;

        public NewActivityViewModel(NewActivityPopup page)
        {
            ItemSelectedCommand = new Command<Category>(new Action<Category>(CategoryChanged));
            FinishCommand = new Command(new Action(finished));
            NotificationCommand = new Command(new Action(setNotification));

            Activity = new Activity();

            void init()
            {
                CategoryChanged(Categories[0]);
            }

            Task.Run(new Action(init));

            newActivityCompleted = new TaskCompletionSource<Activity>();

            this.page = page;
        }

        private async void CategoryChanged(Category category)
        {
            CurrentCategory = category;

            Category curr;

            //Set selected true for category user tapped
            //Else set it to false
            for (int i = 0; i < Categories.Count; i++)
            {
                curr = Categories[i];

                if (curr.type == category.type)
                {
                    curr.selected = true;

                    switch (curr.type)
                    {
                        case CategoryType.Shopping:
                            Header = AppResources.Shopping;

                            Activity.Checklist = new ObservableCollection<ShoppingItem>()
                            {
                                new ShoppingItem()
                            };

                            Shopping view = (Shopping)curr.view;

                            view.viewModel.Activity = Activity;
                            break;
                        case CategoryType.EnduranceSports:
                            Header = AppResources.Sports;

                            Activity.Checklist = null;
                            break;
                        case CategoryType.Meeting:
                            Header = AppResources.Meeting;

                            Activity.Checklist = null;

                            Meeting meeting = (Meeting)curr.view;
                            await meeting.CenterUsersLocation();

                            meeting.viewModel.Activity = Activity;
                            break;
                    }

                    //Set the current activities type
                    Activity.CategoryType = curr.type;
                }
                else
                {
                    curr.selected = false;
                }
            }

            void UI()
            {
                //Set the currents page content to the view of the category
                page.innerContent.Content = category.view;
                //page.innerContent.Content.BindingContext = Activity;
            }
            await Device.InvokeOnMainThreadAsync(new Action(UI)).ConfigureAwait(false);
        }

        private async void finished()
        {
            //Check if the input is valid
            if(!await ((IValidate)page.innerContent.Content).ValidateInput().ConfigureAwait(false))
            {
                return;
            }

            newActivityCompleted.TrySetResult(Activity);
        }

        private async void setNotification()
        {
            NotificationPopup popup = new NotificationPopup(Activity);

            await page.Navigation.PushModalAsync(popup).ConfigureAwait(false);
            await popup.WaitForFinish();

            await page.Navigation.PopModalAsync();
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}