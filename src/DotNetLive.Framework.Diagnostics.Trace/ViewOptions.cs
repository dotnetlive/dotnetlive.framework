using Microsoft.Extensions.Logging;

namespace DotNetLive.Framework.Diagnostics.Trace
{
    public class ViewOptions
    {
        /// <summary>
        /// The minimum <see cref="LogLevel"/> of logs shown on the elm page.
        /// </summary>
        public LogLevel MinLevel { get; set; }

        /// <summary>
        /// The prefix for the logger names of logs shown on the elm page.
        /// </summary>
        public string NamePrefix { get; set; }
    }
}