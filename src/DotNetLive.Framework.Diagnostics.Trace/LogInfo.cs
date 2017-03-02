using Microsoft.Extensions.Logging;
using System;

namespace DotNetLive.Framework.Diagnostics.Trace
{
    public class LogInfo
    {
        public ActivityContext ActivityContext { get; set; }

        public string Name { get; set; }

        public object State { get; set; }

        public Exception Exception { get; set; }

        public string Message { get; set; }

        public LogLevel Severity { get; set; }

        public int EventID { get; set; }

        public DateTimeOffset Time { get; set; }
    }
}