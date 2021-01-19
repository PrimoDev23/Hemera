using Hemera.Helpers;
using Hemera.Interfaces;
using Hemera.Models;
using Hemera.Resx;
using Hemera.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using Xamarin.Forms;
using MenuItem = Hemera.Models.MenuItem;

namespace Hemera.ViewModels
{
    public class StartPageViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private ObservableCollection<MenuItem> _MenuItems = new ObservableCollection<MenuItem>()
        {
            new MenuItem("Home", VarContainer.createPath("M10 20v-6h4v6h5v-8h3L12 3 2 12h3v8z"), true)
        };
        public ObservableCollection<MenuItem> MenuItems
        {
            get => _MenuItems;
            set
            {
                _MenuItems = value;
                OnPropertyChanged();
            }
        }

        public Command CreateNewCommand { get; set; }
        public Command ExpandMenuCommand { get; set; }
        public Command SlideUpCommand { get; set; }
        public Command SlideDownCommand { get; set; }

        private bool _BottomMenuVisible;
        public bool BottomMenuVisible
        {
            get => _BottomMenuVisible;
            set
            {
                _BottomMenuVisible = value;
                OnPropertyChanged();
            }
        }

        private readonly StartPage page;
        public StartPageViewModel(StartPage page)
        {
            CreateNewCommand = new Command(new Action(createActivity));
            ExpandMenuCommand = new Command(new Action(expandCommand));
            SlideUpCommand = new Command(new Action(slideUp));
            SlideDownCommand = new Command(new Action(slideDown));

            this.page = page;
        }

        private async void createActivity()
        {
            await VarContainer.currentOverviewModel?.createNewActivity();
        }

        private void expandCommand()
        {
            BottomMenuVisible = !BottomMenuVisible;
            //Expand it
            if (BottomMenuVisible)
            {
                page.bottomMenu.TranslateTo(0, 0);
                page.backgroundLayer.FadeTo(1);
            }
            else
            {
                page.bottomMenu.TranslateTo(0, 344);
                page.backgroundLayer.FadeTo(0);
            }
        }

        private void slideUp()
        {
            if (!BottomMenuVisible)
            {
                expandCommand();
            }
        }

        private void slideDown()
        {
            if (BottomMenuVisible)
            {
                expandCommand();
            }
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
