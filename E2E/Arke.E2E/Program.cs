using Arke.E2E.Common;
using Arke.Net.E2E;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Arke.E2E
{
    class Program
    {
        static void Main(string[] args)
        {
            List<EndToEnd> tests = new List<EndToEnd>();

            tests.Add(new ArkeNetE2E());

            foreach(var test in tests)
            {
                Console.WriteLine("Testing: " + test.TestName);
                var testResult = test.Run().Result;
                if(testResult == TestResult.Success)
                {
                    LogSuccess(test.TestName);
                }
                else
                {
                    LogFailure(test.TestName);
                }
                test.Dispose();
            }

            Console.WriteLine("All End To End Tests Completed!");

            Console.ReadLine();
        }

        static void LogSuccess(string testName)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(testName + " Succeeded!");
            Console.ForegroundColor = ConsoleColor.White;
        }

        static void LogFailure(string testName)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(testName + " Failed!");
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}
