using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Hemera.ViewModels.Popups
{
    public class SelectDatePopupViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public TaskCompletionSource<(DateTime, DateTime)> completed;

        #region Commands

        public Command AbortCommand { get; set; }
        public Command DoneCommand { get; set; }

        #endregion Commands

        #region Dates

        private DateTime _FromDate = DateTime.Now;
        public DateTime FromDate
        {
            get => _FromDate;
            set
            {
                _FromDate = value;
                OnPropertyChanged();
            }
        }

        private DateTime _ToDate = DateTime.Now;
        public DateTime ToDate
        {
            get => _ToDate;
            set
            {
                _ToDate = value;
                OnPropertyChanged();
            }
        }

        private DateTime _MinDate;
        public DateTime MinDate
        {
            get => _MinDate;
            set
            {
                _MinDate = value;
                OnPropertyChanged();
            }
        }

        private DateTime _MaxDate;
        public DateTime MaxDate
        {
            get => _MaxDate;
            set
            {
                _MaxDate = value;
                OnPropertyChanged();
            }
        }

        #endregion Dates

        public SelectDatePopupViewModel(DateTime minDate, DateTime maxDate)
        {
            AbortCommand = new Command(new Action(abort));
            DoneCommand = new Command(new Action(finish));

            MinDate = minDate;
            MaxDate = maxDate;

            FromDate = minDate;
            ToDate = maxDate;

            completed = new TaskCompletionSource<(DateTime, DateTime)>();
        }

        private void abort()
        {
            completed?.TrySetResult(default);
        }

        private void finish()
        {
            completed?.TrySetResult((FromDate, ToDate));
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
