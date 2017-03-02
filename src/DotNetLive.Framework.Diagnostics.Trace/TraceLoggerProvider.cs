using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;

namespace DotNetLive.Framework.Diagnostics.Trace
{
    internal class TraceLoggerProvider : ILoggerProvider
    {
        private readonly TraceStore _store;
        private readonly TraceOptions _options;

        public TraceLoggerProvider(TraceStore store, IOptions<TraceOptions> options)
        {
            if (store == null)
            {
                throw new ArgumentNullException(nameof(store));
            }

            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            _store = store;
            _options = options.Value;
        }

        public ILogger CreateLogger(string name)
        {
            return new TraceLogger(name, _options, _store);
        }

        public void Dispose()
        {
        }
    }
}