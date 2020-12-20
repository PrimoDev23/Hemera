using Hemera.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace Hemera.Helpers
{
    public static class LINQReplacer
    {
        /// <summary>
        /// Get all activities of given day
        /// </summary>
        /// <param name="activities">Activities to search</param>
        /// <param name="date">Date to search by</param>
        /// <returns></returns>
        public static IEnumerable<Activity> getActivitiesPerDay(this ObservableCollection<Activity> activities, DateTime date)
        {
            Activity curr;
            for (int i = 0; i < activities.Count; i++)
            {
                curr = activities[i];

                //If it's the same day return it
                if (curr.Date.Day == date.Day && curr.Date.Month == date.Month && curr.Date.Year == date.Year)
                {
                    yield return curr;
                }
            }
        }
    }
}