using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Arke.E2E.Common
{
    public abstract class EndToEnd : IDisposable
    {
        public abstract string TestName { get; }

        public abstract Task<TestResult> Run();

        public void LogDebug(string message)
        {
            Console.WriteLine(message);
        }

        public void LogWarn(string message)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(message);
            Console.ForegroundColor = ConsoleColor.White;
        }

        public void LogError(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(message);
            Console.ForegroundColor = ConsoleColor.White;
        }

        public abstract void Dispose();
    }
}
