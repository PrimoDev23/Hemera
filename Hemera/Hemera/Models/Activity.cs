using Hemera.Helpers;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;
using Xamarin.Forms.Maps;

namespace Hemera.Models
{
    public class Activity : INotifyPropertyChanged
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

        private readonly TimeSpan _Time = DateTime.Now.TimeOfDay;

        public TimeSpan Time
        {
            get => _Time;
            set
            {
                Date = new DateTime(Date.Year, Date.Month, Date.Day, value.Hours, value.Minutes, 0);
            }
        }

        private DateTime _Date = DateTime.Now;

        public DateTime Date
        {
            get => _Date;
            set
            {
                _Date = value;

                //Update NotificationTime for new Date
                NotificationTime = _NotificationTime;

                OnPropertyChanged();
            }
        }

        private ObservableCollection<ShoppingItem> _Checklist = new ObservableCollection<ShoppingItem>();

        public ObservableCollection<ShoppingItem> Checklist
        {
            get => _Checklist;
            set
            {
                _Checklist = value;
                OnPropertyChanged();
            }
        }

        private Category _Category;

        [XmlIgnore]
        public Category Category
        {
            get => _Category;
            set
            {
                _Category = value;
                OnPropertyChanged();
            }
        }

        private CategoryType _CategoryType;

        public CategoryType CategoryType
        {
            get => _CategoryType;
            set
            {
                switch (value)
                {
                    case CategoryType.Shopping:
                        Category = VarContainer.categories[0];
                        break;

                    case CategoryType.EnduranceSports:
                        Category = VarContainer.categories[1];
                        break;

                    case CategoryType.Meeting:
                        Category = VarContainer.categories[2];
                        break;
                }
                _CategoryType = value;
            }
        }

        public DateTime NotificationDateTime { get; set; }

        public TimeType TimeType { get; set; } = TimeType.Minute;

        private uint _NotificationTime = 10;
        public uint NotificationTime
        {
            get => _NotificationTime;
            set
            {
                switch (TimeType)
                {
                    case TimeType.Minute:
                        NotificationDateTime = Date.AddMinutes(-value);
                        break;
                    case TimeType.Hour:
                        NotificationDateTime = Date.AddHours(-value);
                        break;
                    case TimeType.Day:
                        NotificationDateTime = Date.AddDays(-value);
                        break;
                }
                _NotificationTime = value;
            }
        }

        public Position Position;

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public enum TimeType : byte
    {
        Minute = 0,
        Hour = 1,
        Day = 2,
        Disabled = 3
    }
}