using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Arke.E2E.Common
{
    public abstract class EndToEnd
    {
        public abstract string TestName { get; }

        public abstract Task<TestResult> Run();

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
