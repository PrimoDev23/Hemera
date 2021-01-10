using Hemera.Helpers;
using Hemera.Interfaces;
using Hemera.Models;
using Hemera.Resx;
using Hemera.Views;
using Hemera.Views.Popups;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Shapes;

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

        private Color _AudioFrameBorder = Color.Transparent;
        public Color AudioFrameBorder
        {
            get => _AudioFrameBorder;
            set
            {
                _AudioFrameBorder = value;
                OnPropertyChanged();
            }
        }

        private bool _Recording;
        public bool Recording
        {
            get => _Recording;
            set
            {
                _Recording = value;

                if (value)
                {
                    AudioFrameBorder = Color.Red;
                }
                else
                {
                    AudioFrameBorder = Color.Transparent;
                }

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
        public Command AttachFileCommand { get; set; }
        public Command RecordAudioCommand { get; set; }
        public Command<Attachment> RemoveAttachmentCommand { get; set; }

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

        private bool _DurMinuteChecked = true;
        public bool DurMinuteChecked
        {
            get => _DurMinuteChecked;
            set
            {
                _DurMinuteChecked = value;
                OnPropertyChanged();
            }
        }

        private bool _DurHourChecked;
        public bool DurHourChecked
        {
            get => _DurHourChecked;
            set
            {
                _DurHourChecked = value;
                OnPropertyChanged();
            }
        }

        #endregion RadioButton

        #region ValidProperty

        public bool titleInvalid = true;
        private bool _NotifyTimeInvalid = false;
        public bool NotifyTimeInvalid
        {
            get => _NotifyTimeInvalid;
            set
            {
                _NotifyTimeInvalid = value;
                OnPropertyChanged();
            }
        }

        private bool _DurationInvalid;
        public bool DurationInvalid
        {
            get => _DurationInvalid;
            set
            {
                _DurationInvalid = value;
                OnPropertyChanged();
            }
        }

        #endregion ValidProperty

        #region Paths

        public Geometry FileGeometry
        {
            get => VarContainer.FileGeometry;
        }

        public Geometry AudioGeometry
        {
            get => VarContainer.AudioGeometry;
        }

        #endregion Paths

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
            AttachFileCommand = new Command(new Action(attachFile));
            RecordAudioCommand = new Command(new Action(recordAudio));
            RemoveAttachmentCommand = new Command<Attachment>(new Action<Attachment>(removeAttachment));

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
            AttachFileCommand = new Command(new Action(attachFile));
            RecordAudioCommand = new Command(new Action(recordAudio));
            RemoveAttachmentCommand = new Command<Attachment>(new Action<Attachment>(removeAttachment));

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
            if (activity.Position != default)
            {
                CenterSelectedLocation(activity.Position);
            }
            else
            {
                CenterUsersLocation();
            }

            this.page = page;

            //When editing also set back the old notification setup
            switch (Activity.NotificationTimeType)
            {
                case TimeType.Minute:
                    page.txt_notificationTime.Text = Activity.NotificationTime.ToString();
                    MinuteChecked = true;
                    break;
                case TimeType.Hour:
                    page.txt_notificationTime.Text = Activity.NotificationTime.ToString();
                    HourChecked = true;
                    break;
                case TimeType.Day:
                    page.txt_notificationTime.Text = Activity.NotificationTime.ToString();
                    DayChecked = true;
                    break;
                case TimeType.Disabled:
                    NotificationsEnabled = false;
                    break;
            }

            page.txt_duration.Text = Activity.Duration.ToString();
            switch (Activity.DurationType)
            {
                case TimeType.Minute:
                    DurMinuteChecked = true;
                    break;
                case TimeType.Hour:
                    DurHourChecked = true;
                    break;
            }

            //This has to be true since it's a saved activity
            titleInvalid = false;
            NotifyTimeInvalid = false;
            DurationInvalid = false;
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
                //Get the users location
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
            if (NotifyTimeInvalid || titleInvalid || DurationInvalid)
            {
                return;
            }

            //If the last item is empty remove it
            ShoppingItem last = Activity.Checklist[^1];
            if (!(last.ItemName?.Length > 0))
            {
                Activity.Checklist.Remove(last);
            }

            //Set notification type and time
            if (NotificationsEnabled)
            {
                if (MinuteChecked)
                {
                    Activity.NotificationTimeType = TimeType.Minute;
                }
                else if (HourChecked)
                {
                    Activity.NotificationTimeType = TimeType.Hour;
                }
                else if (DayChecked)
                {
                    Activity.NotificationTimeType = TimeType.Day;
                }
                Activity.NotificationTime = double.Parse(page.txt_notificationTime.Text);
            }
            else
            {
                Activity.NotificationTimeType = TimeType.Disabled;
            }

            Activity.DurationType = DurMinuteChecked ? TimeType.Minute : TimeType.Hour;
            Activity.Duration = double.Parse(page.txt_duration.Text);

            newActivityCompleted.TrySetResult(Activity);
        }

        private async void attachFile()
        {
            try
            {
                IEnumerable<FileResult> files = await FilePicker.PickMultipleAsync().ConfigureAwait(false);

                if (files == null)
                {
                    return;
                }

                foreach (FileResult result in files)
                {
                    Activity.Attachments.Add(new Attachment(AttachmentType.File, result));
                }
            }
            catch //This is most probably happen if the user denied permission request, so don't make the app crash
            {

            }
        }

        private string currentRecordFile;

        private async void recordAudio()
        {
            bool success = true;

            //If we are already recording stop it
            if (Recording)
            {
                await Task.Run(new Action(stopRecording));

                Activity.Attachments.Add(new Attachment(AttachmentType.Audio, new FileResult(currentRecordFile)));
            }
            else //Else start recording
            {
                if (!DependencyService.Get<IAudio>().checkPermission())
                {
                    return;
                }

                string name = await page.DisplayPromptAsync(AppResources.RecordTitle, null, "OK");

                if(!(name?.Length > 0))
                {
                    return;
                }

                currentRecordFile = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), name + DateTime.Now.ToString("dd-MM-yyyy-HH-mm") + ".3gpp");

                void start()
                {
                    success = DependencyService.Get<IAudio>().startRecord(currentRecordFile);
                }
                await Task.Run(new Action(start)).ConfigureAwait(false);
            }

            if (success)
            {
                Recording = !Recording;
            }
        }

        public static void stopRecording()
        {
            DependencyService.Get<IAudio>().stopRecord();
        }

        public void removeAttachment(Attachment attachment)
        {
            Activity.Attachments.Remove(attachment);

            //Remove the file if it's an audio file
            if(attachment.Type == AttachmentType.Audio)
            {
                File.Delete(attachment.File.FullPath);
            }
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}