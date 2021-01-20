using Hemera.Helpers;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.CompilerServices;
using Xamarin.Forms.Shapes;

namespace Hemera.Models
{
    public class MenuItem : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public MenuItem(string title, Geometry pathData)
        {
            Title = title;
            PathData = pathData;
        }

        public MenuItem(string title, Geometry pathData, bool selected)
        {
            Title = title;
            PathData = pathData;
            Selected = selected;
        }

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

        private Geometry _PathData;
        public Geometry PathData
        {
            get => _PathData;
            set
            {
                _PathData = value;
                OnPropertyChanged();
            }
        }

        private bool _Selected;
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
}
