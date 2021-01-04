using Hemera.Helpers;
using Hemera.Models;
using Hemera.Resx;
using Hemera.Views;
using Hemera.Views.Popups;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace Hemera.ViewModels
{
    public class NewActivityViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public TaskCompletionSource<Activity> newActivityCompleted;

        public Activity Activity { get; set; }

        public ObservableCollection<Category> Categories { get; set; } = VarContainer.categories;

        #region Properties

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

        private bool _NotificationsEnabled = true;
        public bool NotificationsEnabled
        {
            get => _NotificationsEnabled;
            set
            {
                _NotificationsEnabled = value;
                OnPropertyChanged();
            }
        }

        private string _Location;
        public string Location
        {
            get => _Location;
            set
            {
                _Location = value;
                OnPropertyChanged();
            }
        }

        #endregion Properties

        #region Commands

        public Command FinishCommand { get; set; }
        public Command SelectCategoryCommand { get; set; }
        public Command<ShoppingItem> RemoveCommand { get; set; }
        public Command<string> ReturnCommand { get; set; }
        public Command LocationReturnCommand { get; set; }

        #endregion Commands

        #region RadioButtons

        private bool _MinuteChecked = true;
        public bool MinuteChecked
        {
            get => _MinuteChecked;
            set
            {
                _MinuteChecked = value;
                OnPropertyChanged();
            }
        }

        private bool _HourChecked;
        public bool HourChecked
        {
            get => _HourChecked;
            set
            {
                _HourChecked = value;
                OnPropertyChanged();
            }
        }

        private bool _DayChecked;
        public bool DayChecked
        {
            get => _DayChecked;
            set
            {
                _DayChecked = value;
                OnPropertyChanged();
            }
        }

        #endregion RadioButton

        #region ValidProperty

        public bool titleValid = false;
        public bool timeValid = true;

        #endregion ValidProperty

        public NewActivityPopup page;

        #region Constructors

        public NewActivityViewModel(NewActivityPopup page)
        {
            //Set commands
            FinishCommand = new Command(new Action(finished));
            SelectCategoryCommand = new Command(new Action(selectCategory));
            ReturnCommand = new Command<string>(new Action<string>(returnPressed));
            RemoveCommand = new Command<ShoppingItem>(new Action<ShoppingItem>(removeItem));
            LocationReturnCommand = new Command(new Action(setLocation));

            //Select the first Category
            void updateCategories()
            {
                Categories[0].Selected = true;
                for (int i = 1; i < Categories.Count; i++)
                {
                    Categories[i].Selected = false;
                }
            }
            Task.Run(new Action(updateCategories));

            //Create a new Activity with default type shopping
            Activity = new Activity() { CategoryType = CategoryType.Shopping };

            //Add one item so a textbox is shown
            Activity.Checklist.Add(new ShoppingItem());

            //Set the CompletitionSource
            newActivityCompleted = new TaskCompletionSource<Activity>();

            //Center the users location on the map
            CenterUsersLocation();

            this.page = page;
        }

        public NewActivityViewModel(NewActivityPopup page, Activity activity)
        {
            //Set the commands
            FinishCommand = new Command(new Action(finished));
            SelectCategoryCommand = new Command(new Action(selectCategory));
            ReturnCommand = new Command<string>(new Action<string>(returnPressed));
            RemoveCommand = new Command<ShoppingItem>(new Action<ShoppingItem>(removeItem));
            LocationReturnCommand = new Command(new Action(setLocation));

            //Select the Category that is given in activity
            void updateCategories()
            {
                Category curr;
                for (int i = 0; i < Categories.Count; i++)
                {
                    curr = Categories[i];
                    curr.Selected = curr.type == activity.CategoryType;
                }
                CurrentCategory = activity.Category;
            }
            Task.Run(new Action(updateCategories));

            //Set current activity
            Activity = activity;

            if (Activity.Checklist == null)
            {
                Activity.Checklist = new ObservableCollection<ShoppingItem>()
                {
                    new ShoppingItem()
                };
            }
            else
            {
                //Add one entry to make adding new entries easier
                Activity.Checklist.Add(new ShoppingItem());
            }

            //Set the CompletitionSource
            newActivityCompleted = new TaskCompletionSource<Activity>();

            //If Position is selected center on it
            if (activity.Position == default)
            {
                CenterSelectedLocation(activity.Position);
            }
            else
            {
                CenterUsersLocation();
            }

            //This has to be true since it's a saved activity
            titleValid = true;

            this.page = page;
        }

        #endregion Constructors

        #region Checklist

        //If user presses return
        private void returnPressed(string Text)
        {
            //If Text isn't empty we can add a new list item
            if (Text?.Length > 0)
            {
                ShoppingItem item = new ShoppingItem();
                Activity?.Checklist.Add(item);
                page.collView.ScrollTo(Activity.Checklist.Count - 1);

                item.Focused = true;
            }
        }

        //Remove a item from the checklist
        private void removeItem(ShoppingItem item)
        {
            //First remove that entry
            Activity.Checklist.Remove(item);

            //If the list is empty add a new empty item
            if (Activity.Checklist.Count == 0)
            {
                Activity.Checklist.Add(new ShoppingItem() { Focused = true });
            }
        }

        #endregion Checklist

        #region Location

        //Center the users location
        public async Task CenterUsersLocation()
        {
            try
            {
                var position = await Geolocation.GetLocationAsync().ConfigureAwait(false);

                void UI()
                {
                    page.map.MoveToRegion(MapSpan.FromCenterAndRadius(new Position(position.Latitude, position.Longitude), Distance.FromKilometers(1)));
                }
                await Device.InvokeOnMainThreadAsync(new Action(UI)).ConfigureAwait(false);
            }
            catch
            {
                //Location is not enabled
            }
        }

        //Center the users selected location (For edit)
        public async Task CenterSelectedLocation(Position pos)
        {
            void UI()
            {
                page.map.MoveToRegion(MapSpan.FromCenterAndRadius(pos, Distance.FromKilometers(1)));
            }
            await Device.InvokeOnMainThreadAsync(new Action(UI)).ConfigureAwait(false);
        }

        //Add Pin by address
        private async void setLocation()
        {
            Location loc = (await Geocoding.GetLocationsAsync(Location)).First();

            if (loc == null)
            {
                return;
            }

            page.map.Pins.Clear();

            Pin pin = new Pin()
            {
                Position = new Position(loc.Latitude, loc.Longitude),
                Label = AppResources.SelectedPosition
            };
            page.map.Pins.Add(pin);

            Activity.Position = pin.Position;

            page.map.MoveToRegion(MapSpan.FromCenterAndRadius(new Position(loc.Latitude, loc.Longitude), Distance.FromKilometers(1)));
        }

        #endregion Location

        private async void selectCategory()
        {
            //Show category popup and wait for selection
            CategoryPopup popup = new CategoryPopup();
            await page.Navigation.PushModalAsync(popup).ConfigureAwait(false);
            Category selected = await popup.CategorySelected().ConfigureAwait(false);

            //If user selected a category change the CurrentCategory
            if (selected != null)
            {
                CurrentCategory = selected;
                Activity.CategoryType = selected.type;
            }

            await page.Navigation.PopModalAsync().ConfigureAwait(false);
        }

        //User tapped done button
        private void finished()
        {
            //If the last item is empty remove it
            ShoppingItem last = Activity.Checklist[^1];
            if (!(last.ItemName?.Length > 0))
            {
                Activity.Checklist.Remove(last);

                //If checklist is empty after removing it just set it to null
                if (Activity.Checklist.Count == 0)
                {
                    Activity.Checklist = null;
                }
            }

            //Set notification type and time
            if (NotificationsEnabled)
            {
                if (MinuteChecked)
                {
                    Activity.TimeType = TimeType.Minute;
                }
                else if (HourChecked)
                {
                    Activity.TimeType = TimeType.Hour;
                }
                else if (DayChecked)
                {
                    Activity.TimeType = TimeType.Day;
                }
                Activity.NotificationTime = uint.Parse(page.txt_notificationTime.Text);
            }
            else
            {
                Activity.TimeType = TimeType.Disabled;
            }

            newActivityCompleted.TrySetResult(Activity);
        }

        //Check if the time is valid
        public void CheckTimeValid(TextChangedEventArgs e)
        {
            try
            {
                //If user cleared the field we show a warning
                //We can't get in here if type is disabled
                if (!(e.NewTextValue?.Length > 0))
                {
                    page.lbl_timeneeded.IsVisible = true;
                    timeValid = false;
                    return;
                }

                //Instead don't let the user enter invalid values (Text isn't a valid uint)
                if (!uint.TryParse(e.NewTextValue, out _))
                {
                    page.txt_notificationTime.Text = e.OldTextValue;
                    return;
                }

                //Valid input
                timeValid = true;
                page.lbl_timeneeded.IsVisible = false;
            }
            finally
            {
                //We need to set button to enabled if both are valid
                page.btn_done.IsEnabled = timeValid && titleValid;
            }
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}