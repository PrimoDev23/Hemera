using Hemera.Helpers;
using Hemera.Models;
using Hemera.Views;
using Hemera.Views.Popups;
using Microcharts;
using SkiaSharp;
using System;
using System.Collections.Generic;
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

        #region DateProperties

        private DateTime _SelectedDate = DateTime.Now;
        public DateTime SelectedDate
        {
            get => _SelectedDate;
            set
            {
                _SelectedDate = value;

                Task.Run(new Action(initPerCategoryChart));

                OnPropertyChanged();
            }
        }

        private DateTime minDate = DateTime.MaxValue;

        private DateTime maxDate = DateTime.MinValue;

        #endregion DateProperties

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

        private string _SumDurationStatusNone = "-";
        public string SumDurationStatusNone
        {
            get => _SumDurationStatusNone;
            set
            {
                _SumDurationStatusNone = value;
                OnPropertyChanged();
            }
        }

        private string _SumDurationStatusDone = "-";
        public string SumDurationStatusDone
        {
            get => _SumDurationStatusDone;
            set
            {
                _SumDurationStatusDone = value;
                OnPropertyChanged();
            }
        }

        private string _SumDurationStatusMissed = "-";
        public string SumDurationStatusMissed
        {
            get => _SumDurationStatusMissed;
            set
            {
                _SumDurationStatusMissed = value;
                OnPropertyChanged();
            }
        }

        #endregion MaxDuration

        private readonly ChartView page;
        public ChartViewModel(ChartView page)
        {
            //Init chart and other values
            Task.Run(new Action(Init));

            this.page = page;

            VarContainer.currentChartViewModel = this;
        }

        private void Init()
        {
            getMinMaxDates();

            initPerCategoryChart();
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
                SumDurationStatusNone = "-";
                SumDurationStatusDone = "-";
                SumDurationStatusMissed = "-";

                return;
            }

            //Init Entry-Array with length of categories
            float* values = stackalloc float[VarContainer.categories.Count];

            float maxDuration = 0f, sumNone = 0f, sumDone = 0f, sumMissed = 0f;
            Activity maxDurationActivity = null;

            //Calculate the sum of duration for every category
            Activity curr;
            float curr_duration = 0f;
            for (int i = 0; i < activities.Length; i++)
            {
                curr = activities[i];

                curr_duration = curr.DurationType == TimeType.Hour ? curr.Duration : curr.Duration / 60f;
                values[(int)curr.CategoryType] += curr_duration;

                //Check if the current duration is higher than current maxDuration
                if (curr_duration > maxDuration)
                {
                    //Set new maxDuration
                    maxDuration = curr_duration;
                    maxDurationActivity = curr;
                }

                switch (curr.Status)
                {
                    case ActivityStatus.None:
                        sumNone += curr_duration;
                        break;
                    case ActivityStatus.Done:
                        sumDone += curr_duration;
                        break;
                    case ActivityStatus.Missed:
                        sumMissed += curr_duration;
                        break;
                }
            }

            //Set sum Texts
            SumDurationStatusNone = sumNone >= 1 ? $"{sumNone.ToString("0.##")}h" : $"{(sumNone * 60f).ToString("F0")}min";
            SumDurationStatusDone = sumDone >= 1 ? $"{sumDone.ToString("0.##")}h" : $"{(sumDone * 60f).ToString("F0")}min";
            SumDurationStatusMissed = sumMissed >= 1 ? $"{sumMissed.ToString("0.##")}h" : $"{(sumMissed * 60f).ToString("F0")}min";

            //Set MaxDuration Texts
            MaxDurationActivityDur = maxDuration >= 1 ? $"{maxDuration.ToString("0.##")}h" : $"{(maxDuration * 60f).ToString("F0")}min";
            MaxDurationActivityTitle = $"({maxDurationActivity.Title})";

            List<ChartEntry> entries = new List<ChartEntry>();

            maxDuration = 0f;
            Category maxDurationCategory = null;

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
                    ValueLabel = values[i] >= 1 ? $"{values[i].ToString("0.##")}h" : $"{(values[i] * 60f).ToString("F0")}min",
                    ValueLabelColor = curr_color,
                    TextColor = Application.Current.RequestedTheme == OSAppTheme.Dark ? SKColors.White : SKColors.Black,
                });

                //Set the maxDuration of Categories
                if (values[i] > maxDuration)
                {
                    //Set new maxDuration
                    maxDuration = values[i];
                    maxDurationCategory = curr_category;
                }
            }

            //Set MaxDuration texts
            MaxDurationCategoryCat = $"({maxDurationCategory.Name})";
            MaxDurationCategoryDur = maxDuration >= 1 ? $"{maxDuration.ToString("0.##")}h" : $"{(maxDuration * 60f).ToString("F0")}min";

            //Show a Donutchart
            Chart = new RadialGaugeChart()
            {
                Entries = entries,
                BackgroundColor = Application.Current.RequestedTheme == OSAppTheme.Dark ? new SKColor(35, 35, 35) : SKColors.White,
                LabelTextSize = 30,
                LineSize = 14
            };
        }

        /// <summary>
        /// Get activities to analyze according to charttype
        /// </summary>
        /// <returns></returns>
        private IEnumerable<Activity> getCurrentActivities()
        {
            return getActivitiesBetweenDates(minDate, maxDate.Add(new TimeSpan(23, 59, 59)));
        }

        #region Date

        /// <summary>
        /// Get the activities between two dates
        /// </summary>
        /// <param name="startDate">Date to start with (included)</param>
        /// <param name="endDate">Date to end with (included)</param>
        /// <returns></returns>
        private IEnumerable<Activity> getActivitiesBetweenDates(DateTime startDate, DateTime endDate)
        {
            Activity curr;
            for (int i = 0; i < VarContainer.allActivities.Count; i++)
            {
                curr = VarContainer.allActivities[i];

                //Check if Date is between start and end
                if (DateTime.Compare(startDate, curr.Date) <= 0 && DateTime.Compare(curr.Date, endDate) <= 0)
                {
                    yield return curr;
                }
            }
        }

        private void getMinMaxDates()
        {
            Activity curr;
            for (int i = 0; i < VarContainer.allActivities.Count; i++)
            {
                curr = VarContainer.allActivities[i];
                //Update minDate if curr.Date is smaller
                if (DateTime.Compare(curr.Date, minDate) < 0)
                {
                    minDate = curr.Date;
                }

                //Update maxDate if curr.Date is bigger
                if (DateTime.Compare(curr.Date, maxDate) > 0)
                {
                    maxDate = curr.Date;
                }
            }
        }

        public async Task selectDate()
        {
            SelectDatePopup popup = new SelectDatePopup(minDate, maxDate);

            await page.Navigation.PushModalAsync(popup).ConfigureAwait(false);
            (DateTime, DateTime) result = await popup.waitTillFinish().ConfigureAwait(false);

            if (result != default)
            {
                minDate = result.Item1;
                maxDate = result.Item2;

                Task.Run(new Action(initPerCategoryChart));
            }

            await page.Navigation.PopModalAsync().ConfigureAwait(false);
        }

        #endregion Date

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
