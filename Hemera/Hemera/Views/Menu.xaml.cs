using Hemera.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Hemera.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Menu : FlyoutPage
    {
        public Menu()
        {
            InitializeComponent();

            VarContainer.menuPage = this;
            VarContainer.sideBar = (SideBar)Flyout;
        }

        public Task OpenMenu()
        {
            void run()
            {
                IsPresented = true;
            }
            return Device.InvokeOnMainThreadAsync(new Action(run));
        }

        /// <summary>
        /// Update the detail page of Menu
        /// </summary>
        /// <param name="page">Page to push</param>
        /// <returns></returns>
        public Task UpdateDetail(Page page)
        {
            void run()
            {
                Detail = page;
            }
            return Device.InvokeOnMainThreadAsync(new Action(run));
        }
    }
}