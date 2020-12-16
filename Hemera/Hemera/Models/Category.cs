using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using Xamarin.Forms;
using Xamarin.Forms.Shapes;

namespace Hemera.Models
{
    public class Category : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public Category(Geometry pathData, bool selected)
        {
            this.PathData = pathData;
            this.selected = selected;
        }

        public Geometry PathData { get; set; }

        public Brush IconColor { get; set; }
        public Color BackColor { get; set; }

        private bool _selected = false;
        public bool selected
        {
            set
            {
                _selected = value;
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
    }
}
