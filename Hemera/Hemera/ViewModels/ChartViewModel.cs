using Hemera.Helpers;
using Hemera.Models;
using Microcharts;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Hemera.ViewModels
{
    public class ChartViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private Chart _Chart;
        public Chart Chart
        {
            get => _Chart;
            set
            {
                _Chart = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<Category> Categories => VarContainer.categories;

        public ChartViewModel()
        {
            initPerCategoryChart();
        }

        unsafe private void initPerCategoryChart()
        {
            //We don't need to show the diagram if this is empty
            if(!(VarContainer.allActivities?.Count > 0))
            {
                return;
            }

            //Init Entry-Array with length of categories
            float* values = stackalloc float[VarContainer.categories.Count];

            //Calculate the sum of duration for every category
            Activity curr;
            for (int i = 0; i < VarContainer.allActivities.Count; i++)
            {
                curr = VarContainer.allActivities[i];

                if(curr.DurationType == TimeType.Hour)
                {
                    values[(int)curr.CategoryType] += curr.Duration;
                }
                else
                {
                    values[(int)curr.CategoryType] += curr.Duration / 60f;
                }
            }

            ChartEntry[] entries = new ChartEntry[VarContainer.categories.Count];

            Category curr_category;
            for (int i = 0; i < VarContainer.categories.Count; i++)
            {
                curr_category = VarContainer.categories[i];
                entries[i] = new ChartEntry(values[i])
                {
                    Color = new SKColor((byte)(curr_category.BadgeBrush.Color.R * 100), (byte)(curr_category.BadgeBrush.Color.G * 100), (byte)(curr_category.BadgeBrush.Color.B * 100)),
                    Label = curr_category.Name,
                    ValueLabel = values[i].ToString() + "h",
                    ValueLabelColor = new SKColor((byte)(curr_category.BadgeBrush.Color.R * 100), (byte)(curr_category.BadgeBrush.Color.G * 100), (byte)(curr_category.BadgeBrush.Color.B * 100)),
                    TextColor = Application.Current.RequestedTheme == OSAppTheme.Dark ? SKColors.White : SKColors.Black,
                };
            }

            Chart = new DonutChart()
            {
                Entries = entries,
                BackgroundColor = Application.Current.RequestedTheme == OSAppTheme.Dark ? new SKColor(35, 35, 35) : SKColors.White,
                LabelTextSize = 50
            };
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
