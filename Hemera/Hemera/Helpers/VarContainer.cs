using Hemera.Models;
using Hemera.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Hemera.Helpers
{
    public static class VarContainer
    {
        public static Menu menuPage;
        public static SideBar sideBar;

        public static ObservableCollection<Category> categories;
    }
}