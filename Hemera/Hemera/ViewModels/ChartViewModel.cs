using Hemera.Helpers;
using Hemera.Models;
using Microcharts;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Hemera.ViewModels
{
    public class ChartViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private int selectedTab = 0;

        private ObservableCollection<ChartSettings> _ChartSettings = new ObservableCollection<ChartSettings>()
        {
            new ChartSettings(ChartType.Weekly, true),
            new ChartSettings(ChartType.Monthly, false),
            new ChartSettings(ChartType.Yearly, false),
        };
        public ObservableCollection<ChartSettings> ChartSettings
        {
            get => _ChartSettings;
            set
            {
                _ChartSettings = value;
                OnPropertyChanged();
            }
        }

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

        #region Commands

        public Command<ChartSettings> SwitchTabCommand { get; set; }

        #endregion Commands

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

        //Duration of the longest activity
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

        //Title of the activity with most duration
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
            SwitchTabCommand = new Command<ChartSettings>(new Action<ChartSettings>(switchTab));

            //Init chart and other values
            Task.Run(new Action(initPerCategoryChart));
        }

        /// <summary>
        /// Initialize the Chart and other values
        /// </summary>
        unsafe private void initPerCategoryChart()
        {
            //We don't need to show the diagram if this is empty
            if (!(VarContainer.allActivities?.Count > 0))
            {
                return;
            }

            Activity[] activities = getCurrentActivities().ToArray();

            //We don't need to show the diagram if this is empty
            if (!(activities?.Length > 0))
            {
                //we need to clear values here
                Chart = null;
                MaxDurationActivityDur = "-";
                MaxDurationActivityTitle = "-";
                MaxDurationCategoryDur = "-";
                MaxDurationCategoryCat = "-";

                return;
            }

            //Init Entry-Array with length of categories
            float* values = stackalloc float[VarContainer.categories.Count];

            float maxDuration = 0f;

            //Calculate the sum of duration for every category
            Activity curr;
            for (int i = 0; i < activities.Length; i++)
            {
                curr = activities[i];

                if (curr.DurationType == TimeType.Hour)
                {
                    values[(int)curr.CategoryType] += curr.Duration;

                    //Check if the current duration is higher than current maxDuration
                    if (curr.Duration > maxDuration)
                    {
                        //Set new maxDuration
                        maxDuration = curr.Duration;

                        //Set MaxDuration Texts
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
                        //Set new maxDuration
                        maxDuration = divided;

                        //Set MaxDuration texts
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
                if (values[i] == 0)
                {
                    continue;
                }

                curr_category = VarContainer.categories[i];
                curr_color = ColorHelper.ConvertToSKColor(curr_category.BadgeBrush.Color.ToHex());

                //Add a new entry
                entries.Add(new ChartEntry(values[i])
                {
                    Color = curr_color,
                    Label = curr_category.Name,
                    ValueLabel = values[i].ToString() + "h",
                    ValueLabelColor = curr_color,
                    TextColor = Application.Current.RequestedTheme == OSAppTheme.Dark ? SKColors.White : SKColors.Black,
                });

                //Set the maxDuration of Categories
                if (values[i] > maxDuration)
                {
                    //Set new maxDuration
                    maxDuration = values[i];

                    //Set MaxDuration texts
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

        private void switchTab(ChartSettings selected)
        {
            ChartSettings curr;
            for (int i = 0; i < ChartSettings.Count; i++)
            {
                curr = ChartSettings[i];
                curr.Selected = curr == selected;

                if (curr.Selected)
                {
                    selectedTab = i;
                }
            }

            //Init chart and other values
            Task.Run(new Action(initPerCategoryChart));
        }

        /// <summary>
        /// Get activities to analyze according to charttype
        /// </summary>
        /// <returns></returns>
        private IEnumerable<Activity> getCurrentActivities()
        {
            ChartType type = ChartSettings[selectedTab].ChartType;

            DateTime startDate = DateTime.Now, endDate = DateTime.Now;

            switch (type)
            {
                case ChartType.Yearly:
                    startDate = getStartOfYear();
                    endDate = getEndOfYear();
                    break;
                case ChartType.Monthly:
                    startDate = getStartOfMonth();
                    endDate = getEndOfMonth();
                    break;
                case ChartType.Weekly:
                    startDate = getStartOfWeek();
                    endDate = getEndOfWeek();
                    break;
            }

            return getActivitiesBetweenDates(startDate, endDate);
        }

        #region Date

        /// <summary>
        /// Get the date of monday this week
        /// </summary>
        /// <returns>Date of Monday this week</returns>
        private DateTime getStartOfWeek()
        {
            //Create a fresh date at 00:00:00
            DateTime now = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);

            if (now.DayOfWeek == DayOfWeek.Sunday)
            {
                //Sunday is exactly at the end so we want to jump back to monday
                return now.AddDays(-6);
            }
            else
            {
                //Tueday has the value two and needs 1 day to be subtracted (-2 + 1)
                return now.AddDays(-(int)now.DayOfWeek + 1);
            }
        }

        /// <summary>
        /// Get the date of sunday this week
        /// </summary>
        /// <returns>Date of Sunday this week</returns>
        private DateTime getEndOfWeek()
        {
            //Create a fresh date at 23:59:59
            DateTime now = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59);

            if (now.DayOfWeek == DayOfWeek.Sunday)
            {
                //Sunday is the end of the week so just return it
                return now;
            }
            else
            {
                //Monday starts with value 1 and needs 5 days to be added (6 - 1)
                return now.AddDays(6 - (int)now.DayOfWeek);
            }
        }

        /// <summary>
        /// Get the first day of this month
        /// </summary>
        /// <returns>First day of current month</returns>
        private DateTime getStartOfMonth()
        {
            return new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
        }

        /// <summary>
        /// Get the last day of this month
        /// </summary>
        /// <returns>last day of current month</returns>
        private DateTime getEndOfMonth()
        {
            return DateTime.Now.Month switch
            {
                1 or 3 or 5 or 7 or 8 or 10 or 12 => new DateTime(DateTime.Now.Year, DateTime.Now.Month, 31, 23, 59, 59),
                _ => new DateTime(DateTime.Now.Year, DateTime.Now.Month, 30, 23, 59, 59),
            };
        }

        /// <summary>
        /// Get the first day of this year
        /// </summary>
        /// <returns>First day of current year</returns>
        private DateTime getStartOfYear()
        {
            return new DateTime(DateTime.Now.Year, 1, 1);
        }

        /// <summary>
        /// Get the last day of this year
        /// </summary>
        /// <returns>Last day of current year</returns>
        private DateTime getEndOfYear()
        {
            return new DateTime(DateTime.Now.Year, 12, 31, 23, 59, 59);
        }

        private IEnumerable<Activity> getActivitiesBetweenDates(DateTime startDate, DateTime endDate)
        {
            Activity curr;
            for (int i = 0; i < VarContainer.allActivities.Count; i++)
            {
                curr = VarContainer.allActivities[i];

                //Check if Date is between start and end
                if (DateTime.Compare(startDate, curr.Date) < 0 && DateTime.Compare(curr.Date, endDate) < 0)
                {
                    yield return curr;
                }
            }
        }

        #endregion Date

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
