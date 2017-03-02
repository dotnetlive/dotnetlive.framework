using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;

namespace DotNetLive.Framework.Diagnostics.Trace
{
    public class TraceLogger : ILogger
    {
        private readonly string _name;
        private readonly TraceOptions _options;
        private readonly TraceStore _store;

        public TraceLogger(string name, TraceOptions options, TraceStore store)
        {
            _name = name;
            _options = options;
            _store = store;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception,
                          Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel) || (state == null && exception == null))
            {
                return;
            }

            switch (_name)
            {
                case "Microsoft.AspNetCore.Server.Kestrel":
                //case "Microsoft.AspNetCore.Hosting.Internal.WebHost":
                    return;
                default:
                    break;
            }



            LogInfo info = new LogInfo()
            {
                ActivityContext = GetCurrentActivityContext(),
                Name = _name,
                EventID = eventId.Id,
                Severity = logLevel,
                Exception = exception,
                State = state,
                Message = formatter == null ? state.ToString() : formatter(state, exception),
                Time = DateTimeOffset.UtcNow
            };
            if (TraceScope.Current != null)
            {
                TraceScope.Current.Node.Messages.Add(info);
            }
            // The log does not belong to any scope - create a new context for it
            else
            {
                var context = GetNewActivityContext();
                context.RepresentsScope = false;  // mark as a non-scope log
                context.Root = new ScopeNode();
                context.Root.Messages.Add(info);
                _store.AddActivity(context);
            }
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return _options.Filter(_name, logLevel);
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            var scope = new TraceScope(_name, state);
            scope.Context = TraceScope.Current?.Context ?? GetNewActivityContext();
            return TraceScope.Push(scope, _store);
        }

        private ActivityContext GetNewActivityContext()
        {
            return new ActivityContext()
            {
                Id = Guid.NewGuid(),
                Time = DateTimeOffset.UtcNow,
                RepresentsScope = true
            };
        }

        private ActivityContext GetCurrentActivityContext()
        {
            return TraceScope.Current?.Context ?? GetNewActivityContext();
        }


        public static bool IsStaticResource(HttpRequest request)
        {
            if (request == null)
                throw new ArgumentNullException("request");

            string path = request.Path;
            if (path.LastIndexOf('.') == -1)//Not found '.' char
                return false;

            string extension = path.Substring(path.LastIndexOf('.'));

            if (string.IsNullOrWhiteSpace(extension)) return false;

            switch (extension.ToLower())
            {
                case ".axd":
                case ".ashx":
                case ".bmp":
                case ".css":
                case ".gif":
                case ".htm":
                case ".html":
                case ".ico":
                case ".jpeg":
                case ".jpg":
                case ".js":
                case ".png":
                case ".rar":
                case ".zip":
                    return true;
            }

            return false;
        }
    }
}