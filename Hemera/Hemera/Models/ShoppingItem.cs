﻿using Newtonsoft.Json;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Hemera.Models
{
    public class ShoppingItem : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private bool _Checked = false;
        public bool Checked
        {
            get => _Checked;
            set
            {
                _Checked = value;
                OnPropertyChanged();
            }
        }

        private string _ItemName;
        public string ItemName
        {
            get => _ItemName;
            set
            {
                _ItemName = value;
                OnPropertyChanged();
            }
        }

        [JsonIgnore]
        private bool _Focused;
        public bool Focused
        {
            get => _Focused;
            set
            {
                _Focused = value;
                OnPropertyChanged();
            }
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
