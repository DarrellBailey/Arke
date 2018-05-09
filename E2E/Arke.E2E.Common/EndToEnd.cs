using System;
using System.Collections.Generic;
using System.Text;

namespace Arke.E2E.Common
{
    public abstract class EndToEnd
    {
        public abstract TestResult Run();

        public void LogDebug(string message)
        {

        }

        public void LogWarn(string message)
        {

        }

        public void LogError(string message)
        {

        }
    }
}
