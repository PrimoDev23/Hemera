using Hemera.Models;
using Hemera.Views;
using System;
using System.Collections.ObjectModel;
using System.IO;

namespace Hemera.Helpers
{
    public static class VarContainer
    {
        public static Menu menuPage;
        public static SideBar sideBar;

        public static ObservableCollection<Category> categories;

        public static string activityPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "activities.xml");
    }
}