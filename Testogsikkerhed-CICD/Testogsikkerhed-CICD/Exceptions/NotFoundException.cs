using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Testogsikkerhed_CICD.Exceptions
{
    /// <summary>
    /// Exception used to differentiate between custom handled exceptions, and undhandled .NET exception that is caused by bugs.
    /// </summary>
    /// <example>
    /// When Services have to inform Controllers that the request was not found. This simply return a friendly error messages to the client.
    /// </example>
    public class NotFoundException : Exception
    {
        public NotFoundException() : base() { }

        public NotFoundException(string message) : base(message) { }

        public NotFoundException(string message, params object[] args)
            : base(String.Format(CultureInfo.CurrentCulture, message, args))
        {
        }
    }
}
