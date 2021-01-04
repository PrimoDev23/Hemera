using System;

namespace Hemera.Interfaces
{
    public interface INotificationManager
    {
        void SetupNotifyWork(string title, string message, DateTime date, string name);
        void SetupDNDWork(DateTime date, string name);
        void CancelWork(string name);
    }
}
