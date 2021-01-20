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

        //Current activity
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

        public Geometry FileGeometry => VarContainer.FileGeometry;

        public Geometry AudioGeometry => VarContainer.AudioGeometry;

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

        /// <summary>
        /// User presses return key
        /// </summary>
        /// <param name="Text">Text in current entry</param>
        private void returnPressed(string Text)
        {
            //If Text isn't empty we can add a new list item
            if (Text?.Length > 0)
            {
                //Add a new item
                ShoppingItem item = new ShoppingItem();
                Activity?.Checklist.Add(item);

                //Scroll to the newly created item
                page.collView.ScrollTo(Activity.Checklist.Count - 1);

                //Focus entry
                item.Focused = true;
            }
        }

        /// <summary>
        /// Remove an item from the checklist
        /// </summary>
        /// <param name="item">Item to remove</param>
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

        /// <summary>
        /// Center location of the user if possible
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Center a specific position
        /// </summary>
        /// <param name="pos">Position to center</param>
        /// <returns></returns>
        public async Task CenterSelectedLocation(Position pos)
        {
            void UI()
            {
                page.map.MoveToRegion(MapSpan.FromCenterAndRadius(pos, Distance.FromKilometers(1)));
            }
            await Device.InvokeOnMainThreadAsync(new Action(UI)).ConfigureAwait(false);
        }

        /// <summary>
        /// Add pin by address
        /// </summary>
        private async void setLocation()
        {
            //Get the Location for an address
            Location loc = (await Geocoding.GetLocationsAsync(Location)).First();

            //Return if no data was found
            if (loc == null)
            {
                return;
            }

            //Clear pins
            page.map.Pins.Clear();

            //Add a new pin
            Pin pin = new Pin()
            {
                Position = new Position(loc.Latitude, loc.Longitude),
                Label = AppResources.SelectedPosition
            };
            page.map.Pins.Add(pin);

            //Set the current activity's location
            Activity.Position = pin.Position;

            page.map.MoveToRegion(MapSpan.FromCenterAndRadius(new Position(loc.Latitude, loc.Longitude), Distance.FromKilometers(1)));
        }

        #endregion Location

        /// <summary>
        /// Select a category
        /// </summary>
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

        /// <summary>
        /// User finished this view and pressed done button
        /// </summary>
        private void finished()
        {
            //If any needed data is invalid return
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

            //Set the duration data
            Activity.DurationType = DurMinuteChecked ? TimeType.Minute : TimeType.Hour;
            Activity.Duration = float.Parse(page.txt_duration.Text);

            newActivityCompleted.TrySetResult(Activity);
        }

        #region Attachments

        /// <summary>
        /// Attach a file to the activity
        /// </summary>
        private async void attachFile()
        {
            try
            {
                //Let the user pick files
                IEnumerable<FileResult> files = await FilePicker.PickMultipleAsync().ConfigureAwait(false);

                if (files == null)
                {
                    return;
                }

                //Add them to the activity
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

        /// <summary>
        /// Record audio
        /// </summary>
        private async void recordAudio()
        {
            bool success = true;

            //If we are already recording stop it
            if (Recording)
            {
                await Task.Run(new Action(stopRecording)).ConfigureAwait(false);

                Activity.Attachments.Add(new Attachment(AttachmentType.Audio, new FileResult(currentRecordFile)));
            }
            else //Else start recording
            {
                //Check if we have permission
                if (!DependencyService.Get<IAudio>().checkPermission())
                {
                    return;
                }

                //Let the user enter a name for the file
                string name = await page.DisplayPromptAsync(AppResources.RecordTitle, null, "OK").ConfigureAwait(false);

                if (!(name?.Length > 0))
                {
                    return;
                }

                //Create filename for record
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

        /// <summary>
        /// Stop current recording session
        /// </summary>
        public static void stopRecording()
        {
            DependencyService.Get<IAudio>().stopRecord();
        }

        /// <summary>
        /// Remove an attachment
        /// </summary>
        /// <param name="attachment">Attachment to remove</param>
        public void removeAttachment(Attachment attachment)
        {
            //Remove the attachment from the list
            Activity.Attachments.Remove(attachment);

            //Remove the file if it's an audio file
            if (attachment.Type == AttachmentType.Audio)
            {
                File.Delete(attachment.File.FullPath);
            }
        }

        #endregion Attachments

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}