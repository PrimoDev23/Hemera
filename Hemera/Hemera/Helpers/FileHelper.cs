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
                using (StreamWriter sw = new StreamWriter(VarContainer.activityPath))
                {
                    using (JsonWriter reader = new JsonTextWriter(sw))
                    {
                        JsonSerializer serializer = new JsonSerializer();
                        serializer.Serialize(reader, activities);
                    }
                }
            }
            return Task.Run(new Action(save));
        }

        public static ObservableCollection<Activity> loadActivities()
        {
            if (!File.Exists(VarContainer.activityPath))
            {
                return new ObservableCollection<Activity>();
            }

            using (StreamReader sr = new StreamReader(VarContainer.activityPath))
            {
                using (JsonReader reader = new JsonTextReader(sr))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    return serializer.Deserialize<ObservableCollection<Activity>>(reader);
                }
            }
        }
    }
}