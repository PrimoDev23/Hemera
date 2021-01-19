using Hemera.Models;
using Hemera.Resx;
using Hemera.Views;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace Hemera.ViewModels
{
    public class DetailViewViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private Activity _Activity;
        public Activity Activity
        {
            get => _Activity;
            set
            {
                _Activity = value;
                OnPropertyChanged();
            }
        }

        private bool _NotesEnabled;
        public bool NotesEnabled
        {
            get => _NotesEnabled;
            set
            {
                _NotesEnabled = value;
                OnPropertyChanged();
            }
        }

        private bool _ChecklistEnabled;
        public bool ChecklistEnabled
        {
            get => _ChecklistEnabled;
            set
            {
                _ChecklistEnabled = value;
                OnPropertyChanged();
            }
        }

        private bool _MapEnabled;
        public bool MapEnabled
        {
            get => _MapEnabled;
            set
            {
                _MapEnabled = value;
                OnPropertyChanged();
            }
        }

        private bool _AttachmentsEnabled;
        public bool AttachmentsEnabled
        {
            get => _AttachmentsEnabled;
            set
            {
                _AttachmentsEnabled = value;
                OnPropertyChanged();
            }
        }

        public Command<Attachment> OpenCommand { get; set; }

        public DetailViewViewModel(DetailView page, Activity activity)
        {
            OpenCommand = new Command<Attachment>(new Action<Attachment>(openFile));
            Activity = activity;

            NotesEnabled = activity.Notes?.Length > 0;
            ChecklistEnabled = activity.Checklist?.Count > 0;
            AttachmentsEnabled = activity.Attachments?.Count > 0;

            //Show the pin on the map and center it
            if (activity.Position != default)
            {
                MapEnabled = true;
                page.map.Pins.Add(new Pin()
                {
                    Position = activity.Position,
                    Label = AppResources.SelectedPosition
                });
                page.map.MoveToRegion(MapSpan.FromCenterAndRadius(activity.Position, Distance.FromKilometers(1)));
            }
        }

        /// <summary>
        /// open file with default app
        /// </summary>
        /// <param name="attachment">
        /// The attachment to open
        /// </param>
        private async void openFile(Attachment attachment)
        {
            try
            {
                await Launcher.OpenAsync(new OpenFileRequest()
                {
                    File = new ReadOnlyFile(attachment.File.FullPath)
                }).ConfigureAwait(false);
            }
            catch //We should guard this, just in case
            {
                //Actually we don't need to do anything here
                //It's probably users fault if we get here
            }
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
