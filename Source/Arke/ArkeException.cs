using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Arke
{
    /// <summary>
    /// Arke Exception
    /// </summary>
    public class ArkeException : Exception
    {
        public ArkeException(string message) : base(message) { }

        public ArkeException(string message, Exception innerException) : base(message, innerException) { }
    }
}
