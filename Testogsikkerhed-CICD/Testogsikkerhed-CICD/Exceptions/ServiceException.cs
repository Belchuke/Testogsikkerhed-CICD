using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Testogsikkerhed_CICD.Exceptions
{
    /// <summary>
    /// Exception used to differentiate between custom handled exceptions, and undhandled .NET exception that is caused by bugs.
    /// The exception message might be written to a client.
    /// </summary>
    /// <example>
    /// When Services have to inform Controllers of a minor error, that simply needs to return a friendly error messages to the client.
    /// </example>
    public class ServiceException : Exception
    {
        /// <param name="message">Note this could be written directly to a client.</param>
        public ServiceException(string message) : base(message) { }

        /// <param name="message">Note this could be written directly to a client.</param>
        public ServiceException(string message, params object[] args)
            : base(String.Format(CultureInfo.CurrentCulture, message, args))
        {
        }
    }
}
