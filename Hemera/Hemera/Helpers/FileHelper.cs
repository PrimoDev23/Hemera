using Hemera.Models;
using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;

namespace Hemera.Helpers
{
    public static class FileHelper
    {
        /// <summary>
        /// Save the given activities into a json at activityPath
        /// </summary>
        /// <param name="activities">Activities to save</param>
        /// <returns></returns>
        public static Task saveActivities(ObservableCollection<Activity> activities)
        {
            void save()
            {
                File.WriteAllText(VarContainer.activityPath, JsonConvert.SerializeObject(activities));
            }
            return Task.Run(new Action(save));
        }

        public static ObservableCollection<Activity> loadActivities()
        {
            if (!File.Exists(VarContainer.activityPath))
            {
                return new ObservableCollection<Activity>();
            }

            return JsonConvert.DeserializeObject<ObservableCollection<Activity>>(File.ReadAllText(VarContainer.activityPath));
        }
    }
}