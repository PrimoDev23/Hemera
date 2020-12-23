using Hemera.Interfaces;
using Hemera.Models;
using Hemera.Views.Popups;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Hemera.ViewModels.Popups
{
    public class ShoppingViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public Command<string> ReturnCommand { get; set; }
        public Command<ShoppingItem> RemoveCommand { get; set; }

        private Activity _Activity;
        public Activity Activity
        {
            get => _Activity;
            set
            {
                _Activity = value;
                OnPropertyChanged();
            }
        }

        Shopping view;
        public ShoppingViewModel(Shopping view)
        {
            ReturnCommand = new Command<string>(new Action<string>(returnPressed));
            RemoveCommand = new Command<ShoppingItem>(new Action<ShoppingItem>(removeItem));

            this.view = view;
        }

        private void returnPressed(string Text)
        {
            if (Text?.Length > 0)
            {
                ShoppingItem item = new ShoppingItem();
                Activity?.Checklist.Add(item);
                view.collView.ScrollTo(Activity.Checklist.Count - 1);

                item.Focused = true;
            }
        }

        private void removeItem(ShoppingItem item)
        {
            if (Activity?.Checklist.Count > 1)
            {
                item.Focused = false;

                Activity.Checklist.Remove(item);
            }
            else
            {
                Activity.Checklist.Clear();
                Activity.Checklist.Add(new ShoppingItem() { Focused = true });
            }
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
