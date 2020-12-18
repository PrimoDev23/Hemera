using System.ComponentModel;
using System.Runtime.CompilerServices;
using Xamarin.Forms;
using Xamarin.Forms.Shapes;

namespace Hemera.Models
{
    public class Category : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public Category(Geometry pathData, bool selected, CategoryType type, ContentView view)
        {
            PathData = pathData;
            this.selected = selected;
            this.type = type;
            this.view = view;
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
                    IconColor = Brush.White;
                    BackColor = (Color)Application.Current.Resources["colorPrimary"];
                }
                else
                {
                    IconColor = new SolidColorBrush((Color)Application.Current.Resources["colorPrimary"]);
                    BackColor = App.Current.RequestedTheme == OSAppTheme.Light ? (Color)Application.Current.Resources["colorLightTheme"] : (Color)Application.Current.Resources["colorDarkTheme"];
                }
            }
        }

        public CategoryType type;

        public ContentView view;

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
