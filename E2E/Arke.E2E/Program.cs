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
                Console.WriteLine(test.Run().Result.ToString());
            }

            Console.WriteLine("All End To End Tests Completed!");

            Console.ReadLine();
        }
    }
}
