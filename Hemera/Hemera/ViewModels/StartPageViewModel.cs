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

        //MenuItems for the bottom menu
        private ObservableCollection<MenuItem> _MenuItems = new ObservableCollection<MenuItem>()
        {
            new MenuItem(AppResources.HomePage, VarContainer.createPath("M10 20v-6h4v6h5v-8h3L12 3 2 12h3v8z"), true),
            new MenuItem(AppResources.ChartPage, VarContainer.createPath("M19 3H5c-1.1 0-2 .9-2 2v14c0 1.1.9 2 2 2h14c1.1 0 2-.9 2-2V5c0-1.1-.9-2-2-2zM9 17H7v-5h2v5zm4 0h-2v-3h2v3zm0-5h-2v-2h2v2zm4 5h-2V7h2v10z"), false),
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

        #region Commands

        public Command CreateNewCommand { get; set; }
        public Command ExpandMenuCommand { get; set; }
        public Command SlideUpCommand { get; set; }
        public Command SlideDownCommand { get; set; }
        public Command<MenuItem> SelectMenuItemCommand { get; set; }

        #endregion Commands

        #region MenuVisibility

        private bool _BottomMenuVisible = false;
        public bool BottomMenuVisible
        {
            get => _BottomMenuVisible;
            set
            {
                _BottomMenuVisible = value;
                OnPropertyChanged();
            }
        }

        //Allow changing Visibility of Add-Button
        private bool allowChangeAddButtonVisible = true;
        private bool _AddButtonVisible = true;
        public bool AddButtonVisible
        {
            get => _AddButtonVisible;
            set
            {
                _AddButtonVisible = value;
                OnPropertyChanged();
            }
        }

        #endregion MenuVisibility

        private readonly StartPage page;
        public StartPageViewModel(StartPage page)
        {
            CreateNewCommand = new Command(new Action(createActivity));
            ExpandMenuCommand = new Command(new Action(toggleMenu));
            SlideUpCommand = new Command(new Action(slideUp));
            SlideDownCommand = new Command(new Action(slideDown));
            SelectMenuItemCommand = new Command<MenuItem>(new Action<MenuItem>(selectMenuItem));

            this.page = page;
        }

        #region MoveMenu

        /// <summary>
        /// Toggle bottom bar menu
        /// </summary>
        private void toggleMenu()
        {
            BottomMenuVisible = !BottomMenuVisible;

            //Check if changing the visibility of add button is allowed
            if (allowChangeAddButtonVisible)
            {
                AddButtonVisible = !BottomMenuVisible;
            }

            //Expand it
            if (BottomMenuVisible)
            {
                page.bottomMenu.TranslateTo(0, 0);
                page.backgroundLayer.FadeTo(1);
            }
            else //Collapse it
            {
                page.bottomMenu.TranslateTo(0, 344);
                page.backgroundLayer.FadeTo(0);
            }
        }

        /// <summary>
        /// Expand Menu if not already expanded
        /// </summary>
        private void slideUp()
        {
            if (!BottomMenuVisible)
            {
                toggleMenu();
            }
        }

        /// <summary>
        /// Collapse menu if not already collapsed
        /// </summary>
        private void slideDown()
        {
            if (BottomMenuVisible)
            {
                toggleMenu();
            }
        }

        #endregion MoveMenu

        /// <summary>
        /// Select a menu item
        /// </summary>
        /// <param name="item">Menu item to select</param>
        private void selectMenuItem(MenuItem item)
        {
            //Return if it's already selected
            if (item.Selected)
            {
                return;
            }

            //Change selection
            MenuItem curr;
            for (int i = 0; i < MenuItems.Count; i++)
            {
                curr = MenuItems[i];
                curr.Selected = curr == item;
            }

            toggleMenu();

            //Display page
            if (item.Title == AppResources.HomePage)
            {
                page.holderView.Content = new Overview();
                AddButtonVisible = true;
                allowChangeAddButtonVisible = true;
            }
            else if (item.Title == AppResources.ChartPage)
            {
                page.holderView.Content = new ChartView();
                AddButtonVisible = false;
                allowChangeAddButtonVisible = false;
            }
        }

        /// <summary>
        /// Create new activity
        /// </summary>
        private async void createActivity()
        {
            await VarContainer.currentOverviewModel?.createNewActivity();
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
