using System;
using System.Collections.Generic;
using System.Text;

namespace Hemera.Interfaces
{
    public interface IDND
    {
        bool CheckForPermission();
        void AskPermission();
    }
}
