using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace Hemera.Models
{
    public class ChartSettings : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public ChartSettings(ChartType type, bool selected)
        {
            this.ChartType = type;
            Selected = selected;
        }

        public ChartType ChartType;

        private bool _Selected = false;
        public bool Selected
        {
            get => _Selected;
            set
            {
                _Selected = value;
                OnPropertyChanged();
            }
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public enum ChartType : byte
    {
        Yearly,
        Monthly,
        Weekly
    }
}
