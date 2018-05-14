using Arke.E2E.Common;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Arke.Net.E2E
{
    public class ArkeNetE2E : EndToEnd
    {
        public override string TestName => "Arke.Net";

        public override async Task<TestResult> Run()
        {
            LogDebug("Beginning ArkeNet End To End Testing");

            return TestResult.Success;
        }
    }
}
