using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;

namespace Hemera.Models
{
    public class ShoppingItem : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private bool _Bought = false;
        public bool Bought
        {
            get => _Bought;
            set
            {
                _Bought = value;
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

        [XmlIgnore]
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
