using Hemera.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Hemera.Views.Popups
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Shopping : ContentView
    {
        public Command<string> ReturnCommand { get; set; }
        public Command<ShoppingItem> RemoveCommand { get; set; }

        public Activity CurrentActivity;

        public Shopping()
        {
            InitializeComponent();

            ReturnCommand = new Command<string>(new Action<string>(returnPressed));
            RemoveCommand = new Command<ShoppingItem>(new Action<ShoppingItem>(removeItem));
        }

        private void returnPressed(string Text)
        {
            if (Text?.Length > 0)
            {
                CurrentActivity?.Checklist.Add(new ShoppingItem());
            }
        }

        private void removeItem(ShoppingItem item)
        {
            if (CurrentActivity?.Checklist.Count > 1)
            {
                CurrentActivity.Checklist.Remove(item);
            }
            else
            {
                CurrentActivity.Checklist.Clear();
                CurrentActivity.Checklist.Add(new ShoppingItem());
            }
        }
    }
}