using Hemera.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Hemera.Helpers
{
    public static class FileHelper
    {
        /// <summary>
        /// Save the given activities into an xml at activityPath
        /// </summary>
        /// <param name="activities">Activities to save</param>
        /// <returns></returns>
        public static Task saveActivities(ObservableCollection<Activity> activities)
        {
            void save()
            {
                using (FileStream fs = new FileStream(VarContainer.activityPath, FileMode.Create))
                {
                    XmlSerializer xml = new XmlSerializer(typeof(ObservableCollection<Activity>));
                    xml.Serialize(fs, activities);
                }
            }
            return Task.Run(new Action(save));
        }

        public static ObservableCollection<Activity> loadActivities()
        {
            //We get some shitty error here
            //Probably just some stream fucked up while debugging, but to prevent annoying exceptions just do a try catch
            try
            {
                if (!File.Exists(VarContainer.activityPath))
                {
                    return new ObservableCollection<Activity>();
                }

                using (FileStream fs = new FileStream(VarContainer.activityPath, FileMode.Open))
                {
                    XmlSerializer xml = new XmlSerializer(typeof(ObservableCollection<Activity>));
                    return (ObservableCollection<Activity>)xml.Deserialize(fs);
                }
            }catch(Exception ex)
            {
                return new ObservableCollection<Activity>();
            }
        }
    }
}