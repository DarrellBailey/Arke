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
        /// <summary>
        /// Create an Arke Exception with a message.
        /// </summary>
        /// <param name="message">The message.</param>
        public ArkeException(string message) : base(message) { }

        /// <summary>
        /// Create an Arke Exception with a message and inner exception.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        public ArkeException(string message, Exception innerException) : base(message, innerException) { }
    }
}
