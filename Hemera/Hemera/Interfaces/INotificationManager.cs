using System;
using System.Collections.Generic;
using System.Text;

namespace Hemera.Interfaces
{
    public interface INotificationManager
    {
        void SetupWork(string title, string message, DateTime date, string name);
        void CancelWork(string name);
    }
}
