using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Xml.Serialization;
using Xamarin.Forms;

namespace Hemera.Models
{
    public class Day : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private string _Title;
        public string Title
        {
            get => _Title;
            set
            {
                _Title = value;
                OnPropertyChanged();
            }
        }

        private string _Notes;
        public string Notes
        {
            get => _Notes;
            set
            {
                _Notes = value;
                OnPropertyChanged();
            }
        }

        private DateTime _Date;
        public DateTime Date
        {
            get => _Date;
            set
            {
                _Date = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<string> _Checklist;
        public ObservableCollection<string> Checklist
        {
            get => _Checklist;
            set
            {
                _Checklist = value;
                OnPropertyChanged();
            }
        }

        private ImageSource _Badge;

        [XmlIgnore]
        public ImageSource Badge
        {
            get => _Badge;
            set
            {
                _Badge = value;
                OnPropertyChanged();
            }
        }

        public string BadgeName
        {
            set
            {
                Badge = value;
            }
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}