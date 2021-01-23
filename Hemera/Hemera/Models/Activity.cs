using Hemera.Helpers;
using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
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

        private TimeSpan _Time = DateTime.Now.TimeOfDay;
        public TimeSpan Time
        {
            get => _Time;
            set
            {
                _Time = value;
                Date = new DateTime(Date.Year, Date.Month, Date.Day, value.Hours, value.Minutes, 0);

                OnPropertyChanged();
            }
        }

        private DateTime _Date = DateTime.Now;
        public DateTime Date
        {
            get => _Date;
            set
            {
                if (TimeSpan.Compare(value.TimeOfDay, Time) != 0)
                {
                    _Date = new DateTime(value.Year, value.Month, value.Day, Time.Hours, Time.Minutes, 0);
                }
                else
                {
                    _Date = value;
                }

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

        [JsonIgnore]
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
                Category = VarContainer.categories[(int)value];

                _CategoryType = value;
            }
        }

        public DateTime NotificationDateTime { get; set; }

        public TimeType NotificationTimeType { get; set; }

        private double _NotificationTime = 10;
        public double NotificationTime
        {
            get => _NotificationTime;
            set
            {
                switch (NotificationTimeType)
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

        private bool _DoNotDisturb;
        public bool DoNotDisturb
        {
            get => _DoNotDisturb;
            set
            {
                _DoNotDisturb = value;
                OnPropertyChanged();
            }
        }

        public TimeType DurationType;

        private float _Duration = 60;
        public float Duration
        {
            get => _Duration;
            set
            {
                _Duration = value;

                EndDate = DurationType switch
                {
                    TimeType.Minute => Date.AddMinutes(value),
                    TimeType.Hour => Date.AddHours(value),
                    //This should not happen
                    TimeType.Day => throw new NotImplementedException(),
                    TimeType.Disabled => throw new NotImplementedException(),
                    _ => throw new NotImplementedException(),
                };
            }
        }

        private DateTime _EndDate = DateTime.Now.AddMinutes(60);
        [JsonIgnore]
        public DateTime EndDate
        {
            get => _EndDate;
            set
            {
                _EndDate = value;
                OnPropertyChanged();
            }
        }

        private ActivityStatus _Status;
        public ActivityStatus Status
        {
            get => _Status;
            set
            {
                _Status = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<Attachment> _Attachments = new ObservableCollection<Attachment>();
        public ObservableCollection<Attachment> Attachments
        {
            get => _Attachments;
            set
            {
                _Attachments = value;
                OnPropertyChanged();
            }
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public enum ActivityStatus
    {
        None = 0,
        Done = 1,
        Missed = 2
    }

    public enum TimeType : byte
    {
        Minute = 0,
        Hour = 1,
        Day = 2,
        Disabled = 3
    }
}