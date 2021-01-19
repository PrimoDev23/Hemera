using Hemera.Models;
using Hemera.Resx;
using Hemera.ViewModels;
using Hemera.Views;
using System;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using Xamarin.Forms.Shapes;

namespace Hemera.Helpers
{
    public static class VarContainer
    {
        public static StartPage holderPage;

        public static OverviewViewModel currentOverviewModel;

        public static ObservableCollection<Category> categories = new ObservableCollection<Models.Category>()
        {
            new Category(AppResources.Shopping, VarContainer.createPath("M19 6h-2c0-2.76-2.24-5-5-5S7 3.24 7 6H5c-1.1 0-1.99.9-1.99 2L3 20c0 1.1.9 2 2 2h14c1.1 0 2-.9 2-2V8c0-1.1-.9-2-2-2zm-7-3c1.66 0 3 1.34 3 3H9c0-1.66 1.34-3 3-3zm0 10c-2.76 0-5-2.24-5-5h2c0 1.66 1.34 3 3 3s3-1.34 3-3h2c0 2.76-2.24 5-5 5z"), true, CategoryType.Shopping, new SolidColorBrush(Color.Yellow)),
            new Category(AppResources.Sports, VarContainer.createPath("M13.49 5.48c1.1 0 2-.9 2-2s-.9-2-2-2-2 .9-2 2 .9 2 2 2zm-3.6 13.9l1-4.4 2.1 2v6h2v-7.5l-2.1-2 .6-3c1.3 1.5 3.3 2.5 5.5 2.5v-2c-1.9 0-3.5-1-4.3-2.4l-1-1.6c-.4-.6-1-1-1.7-1-.3 0-.5.1-.8.1l-5.2 2.2v4.7h2v-3.4l1.8-.7-1.6 8.1-4.9-1-.4 2 7 1.4z"), false, CategoryType.Sports, new SolidColorBrush(Color.Green)),
            new Category(AppResources.Meeting, VarContainer.createPath("M14 6v15H3v-2h2V3h9v1h5v15h2v2h-4V6h-3zm-4 5v2h2v-2h-2z"), false, CategoryType.Meeting, new SolidColorBrush(Color.Red)),
            new Category(AppResources.Work, VarContainer.createPath("M20 6h-4V4c0-1.11-.89-2-2-2h-4c-1.11 0-2 .89-2 2v2H4c-1.11 0-1.99.89-1.99 2L2 19c0 1.11.89 2 2 2h16c1.11 0 2-.89 2-2V8c0-1.11-.89-2-2-2zm-6 0h-4V4h4v2z"), false, CategoryType.Work, new SolidColorBrush(Color.YellowGreen)),
            new Category(AppResources.Break, VarContainer.createPath("M20 3H4v10c0 2.21 1.79 4 4 4h6c2.21 0 4-1.79 4-4v-3h2c1.11 0 2-.9 2-2V5c0-1.11-.89-2-2-2zm0 5h-2V5h2v3zM4 19h16v2H4z"), false, CategoryType.Break, new SolidColorBrush(Color.ForestGreen)),
            new Category(AppResources.Learning, VarContainer.createPath("M5 13.18v4L12 21l7-3.82v-4L12 17l-7-3.82zM12 3L1 9l11 6 9-4.91V17h2V9L12 3z"), false, CategoryType.Learning, new SolidColorBrush(Color.YellowGreen)),
            new Category(AppResources.Housework, VarContainer.createPath("M19,9.3V4h-3v2.6L12,3L2,12h3v8h5v-6h4v6h5v-8h3L19,9.3z M10,10c0-1.1,0.9-2,2-2s2,0.9,2,2H10z"), false, CategoryType.Housework, new SolidColorBrush(Color.LightCyan)),
        };

        public static Geometry FileGeometry = VarContainer.createPath("M14 2H6c-1.1 0-1.99.9-1.99 2L4 20c0 1.1.89 2 1.99 2H18c1.1 0 2-.9 2-2V8l-6-6zm2 14h-3v3h-2v-3H8v-2h3v-3h2v3h3v2zm-3-7V3.5L18.5 9H13z");
        public static Geometry AudioGeometry = VarContainer.createPath("M12 14c1.66 0 2.99-1.34 2.99-3L15 5c0-1.66-1.34-3-3-3S9 3.34 9 5v6c0 1.66 1.34 3 3 3zm5.3-3c0 3-2.54 5.1-5.3 5.1S6.7 14 6.7 11H5c0 3.41 2.72 6.23 6 6.72V21h2v-3.28c3.28-.48 6-3.3 6-6.72h-1.7z");

        public static Geometry createPath(string data)
        {
            return (Geometry)new PathGeometryConverter().ConvertFromInvariantString(data);
        }

        public static string activityPath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "activities.json");

        public static Color colorPrimaryOpaque = Color.FromHex("#5500c853");
    }
}