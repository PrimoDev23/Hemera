using System.ComponentModel;
using System.Runtime.CompilerServices;
using Xamarin.Forms;
using Xamarin.Forms.Shapes;

namespace Hemera.Models
{
    public class Category : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public Category(Geometry pathData, bool selected, CategoryType type, ContentView view, SolidColorBrush brush)
        {
            PathData = pathData;
            this.selected = selected;
            this.type = type;
            this.view = view;
            this.BadgeBrush = brush;
        }

        public Geometry PathData { get; set; }

        private Brush _IconColor;

        public Brush IconColor
        {
            get => _IconColor;
            set
            {
                _IconColor = value;
                OnPropertyChanged();
            }
        }

        private Color _BackColor;

        public Color BackColor
        {
            get => _BackColor;
            set
            {
                _BackColor = value;
                OnPropertyChanged();
            }
        }

        public bool selected
        {
            set
            {
                if (value)
                {
                    IconColor = (Brush)Application.Current.Resources["brushPrimary"];
                }
                else
                {
                    IconColor = App.Current.RequestedTheme == OSAppTheme.Light ? (Brush)Application.Current.Resources["brushDarkTheme"] : (Brush)Application.Current.Resources["brushLightTheme"];
                }
            }
        }

        public CategoryType type;

        public ContentView view;

        public Brush BadgeBrush { get; set; }

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public enum CategoryType
    {
        Shopping,
        Sports,
        Meeting
    }
}