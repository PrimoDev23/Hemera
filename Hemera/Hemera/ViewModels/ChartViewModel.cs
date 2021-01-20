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

        #region MaxDuration

        //Duration of the max. Duration Category
        private string _MaxDurationCategoryDur = "-";
        public string MaxDurationCategoryDur
        {
            get => _MaxDurationCategoryDur;
            set
            {
                _MaxDurationCategoryDur = value;
                OnPropertyChanged();
            }
        }

        //Category-title of the max. Duration Category
        private string _MaxDurationCategoryCat = "-";
        public string MaxDurationCategoryCat
        {
            get => _MaxDurationCategoryCat;
            set
            {
                _MaxDurationCategoryCat = value;
                OnPropertyChanged();
            }
        }

        private string _MaxDurationActivityDur = "-";
        public string MaxDurationActivityDur
        {
            get => _MaxDurationActivityDur;
            set
            {
                _MaxDurationActivityDur = value;
                OnPropertyChanged();
            }
        }

        private string _MaxDurationActivityTitle = "-";
        public string MaxDurationActivityTitle
        {
            get => _MaxDurationActivityTitle;
            set
            {
                _MaxDurationActivityTitle = value;
                OnPropertyChanged();
            }
        }

        #endregion MaxDuration

        public ChartViewModel()
        {
            Task.Run(new Action(initPerCategoryChart));
        }

        /// <summary>
        /// Initialize the Chart and other values
        /// </summary>
        unsafe private void initPerCategoryChart()
        {
            //We don't need to show the diagram if this is empty
            if(!(VarContainer.allActivities?.Count > 0))
            {
                return;
            }

            //Init Entry-Array with length of categories
            float* values = stackalloc float[VarContainer.categories.Count];

            float maxDuration = 0f;

            //Calculate the sum of duration for every category
            Activity curr;
            for (int i = 0; i < VarContainer.allActivities.Count; i++)
            {
                curr = VarContainer.allActivities[i];

                if(curr.DurationType == TimeType.Hour)
                {
                    values[(int)curr.CategoryType] += curr.Duration;

                    if (curr.Duration > maxDuration)
                    {
                        maxDuration = curr.Duration;
                        MaxDurationActivityDur = $"{curr.Duration.ToString()}h";
                        MaxDurationActivityTitle = $"({curr.Title})";
                    }
                }
                else
                {
                    float divided = curr.Duration / 60f;
                    values[(int)curr.CategoryType] += divided;

                    if (divided > maxDuration)
                    {
                        maxDuration = divided;
                        MaxDurationActivityDur = $"{divided.ToString()}h";
                        MaxDurationActivityTitle = $"({curr.Title})";
                    }
                }
            }

            List<ChartEntry> entries = new List<ChartEntry>();

            maxDuration = 0f;

            //Fill the entries array
            Category curr_category;
            SKColor curr_color;
            for (int i = 0; i < VarContainer.categories.Count; i++)
            {
                //continue if we don't have any values
                if(values[i] == 0)
                {
                    continue;
                }

                curr_category = VarContainer.categories[i];
                curr_color = ColorHelper.ConvertToSKColor(curr_category.BadgeBrush.Color.ToHex());

                entries.Add(new ChartEntry(values[i])
                {
                    Color = curr_color,
                    Label = curr_category.Name,
                    ValueLabel = values[i].ToString() + "h",
                    ValueLabelColor = curr_color,
                    TextColor = Application.Current.RequestedTheme == OSAppTheme.Dark ? SKColors.White : SKColors.Black,
                });

                //Set the maxDuration of Categories
                if(values[i] > maxDuration)
                {
                    maxDuration = values[i];
                    MaxDurationCategoryCat = $"({curr_category.Name})";
                    MaxDurationCategoryDur = $"{values[i].ToString()}h";
                }
            }

            //Show a Donutchart
            Chart = new RadialGaugeChart()
            {
                Entries = entries,
                BackgroundColor = Application.Current.RequestedTheme == OSAppTheme.Dark ? new SKColor(35, 35, 35) : SKColors.White,
                LabelTextSize = 30,
                LineSize = 14
            };
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
