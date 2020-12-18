using System.Threading.Tasks;
using Xamarin.Forms;

namespace Hemera.Controls
{
    public class FocusEntry : Entry
    {
        public FocusEntry()
        {
            Unfocused += FocusEntry_Unfocused;
        }

        private void FocusEntry_Unfocused(object sender, FocusEventArgs e)
        {
            IsFocused = false;
        }

        public bool IsFocused
        {
            get => (bool)GetValue(IsFocusedProperty);
            set => SetValue(IsFocusedProperty, value);
        }
        public static readonly BindableProperty IsFocusedProperty = BindableProperty.Create(nameof(IsFocused), typeof(bool), typeof(FocusEntry), false, BindingMode.TwoWay, propertyChanged: FocusChanged);

        private static async void FocusChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if ((bool)newValue)
            {
                //This somehow fixes issues that control can't get focused
                await Task.Delay(200);
                bool res = ((FocusEntry)bindable).Focus();
            }
        }

        ~FocusEntry()
        {
            Unfocused -= FocusEntry_Unfocused;
        }
    }
}
