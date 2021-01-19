using System.ComponentModel;
using System.Runtime.CompilerServices;
using Xamarin.Forms;
using Xamarin.Forms.Shapes;

namespace Hemera.Models
{
    public class Category : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public Category(string name, Geometry pathData, bool selected, CategoryType type, SolidColorBrush brush)
        {
            Name = name;
            PathData = pathData;
            Selected = selected;
            this.type = type;
            BadgeBrush = brush;
        }

        public string Name { get; set; }

        public Geometry PathData { get; set; }

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

        public CategoryType type;

        public SolidColorBrush BadgeBrush { get; set; }

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public enum CategoryType
    {
        Shopping,
        Sports,
        Meeting,
        Work,
        Break,
        Learning,
        Housework
    }
}