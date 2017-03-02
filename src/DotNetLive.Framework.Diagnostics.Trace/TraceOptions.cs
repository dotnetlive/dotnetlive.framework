using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;

namespace DotNetLive.Framework.Diagnostics.Trace
{
    public class TraceOptions
    {
        /// <summary>
        /// Specifies the path to view the logs.
        /// </summary>
        public PathString Path { get; set; } = new PathString("/trace");

        /// <summary>
        /// Determines whether log statements should be logged based on the name of the logger
        /// and the <see cref="M:LogLevel"/> of the message.
        /// </summary>
        public Func<string, LogLevel, bool> Filter { get; set; } = (name, level) => true;
    }
}