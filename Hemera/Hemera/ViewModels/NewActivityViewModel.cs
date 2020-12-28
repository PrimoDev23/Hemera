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

        public Command FinishCommand { get; set; }
        public Command SelectCategoryCommand { get; set; }
        public Command<ShoppingItem> RemoveCommand { get; set; }
        public Command<string> ReturnCommand { get; set; }
        public Command LocationReturnCommand { get; set; }

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

        public bool titleValid = false;
        public bool timeValid = true;

        public NewActivityPopup page;

        public NewActivityViewModel(NewActivityPopup page)
        {
            FinishCommand = new Command(new Action(finished));
            SelectCategoryCommand = new Command(new Action(selectCategory));
            ReturnCommand = new Command<string>(new Action<string>(returnPressed));
            RemoveCommand = new Command<ShoppingItem>(new Action<ShoppingItem>(removeItem));
            LocationReturnCommand = new Command(new Action(setLocation));

            void updateCategories()
            {
                Categories[0].Selected = true;
                for (int i = 1; i < Categories.Count; i++)
                {
                    Categories[i].Selected = false;
                }
            }
            Task.Run(new Action(updateCategories));

            Activity = new Activity() { CategoryType = CategoryType.Shopping };

            Activity.Checklist.Add(new ShoppingItem());

            newActivityCompleted = new TaskCompletionSource<Activity>();

            CenterUsersLocation();

            this.page = page;
        }

        private async void selectCategory()
        {
            CategoryPopup popup = new CategoryPopup();
            await page.Navigation.PushModalAsync(popup).ConfigureAwait(false);
            Category selected = await popup.CategorySelected().ConfigureAwait(false);

            if (selected != null)
            {
                CurrentCategory = selected;
                Activity.CategoryType = selected.type;
            }

            await page.Navigation.PopModalAsync().ConfigureAwait(false);
        }

        private void returnPressed(string Text)
        {
            if (Text?.Length > 0)
            {
                ShoppingItem item = new ShoppingItem();
                Activity?.Checklist.Add(item);
                page.collView.ScrollTo(Activity.Checklist.Count - 1);

                item.Focused = true;
            }
        }

        private void removeItem(ShoppingItem item)
        {
            Activity.Checklist.Remove(item);

            if (Activity.Checklist.Count == 0)
            {
                Activity.Checklist.Add(new ShoppingItem() { Focused = true });
            }
        }

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

            //Set notification
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
                page.btn_done.IsEnabled = timeValid && titleValid;
            }
        }

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

        private async void setLocation()
        {
            Location loc = (await Geocoding.GetLocationsAsync(Location)).First();

            if(loc == null)
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

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}