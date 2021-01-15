using Hemera.Helpers;
using Hemera.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace Hemera.ViewModels
{
    public class SideBarViewModel : INotifyPropertyChanged
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

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
