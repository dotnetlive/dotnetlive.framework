using DotNetLive.Framework.Diagnostics.Trace.Views;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DotNetLive.Framework.Diagnostics.Trace
{
    internal class TracePageMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly TraceOptions _options;
        private readonly TraceStore _store;

        public TracePageMiddleware(RequestDelegate next, IOptions<TraceOptions> options, TraceStore store)
        {
            _next = next;
            _options = options.Value;
            _store = store;
        }

        public async Task Invoke(HttpContext context)
        {
            if (!context.Request.Path.StartsWithSegments(_options.Path))
            {
                await _next(context);
                return;
            }

            var t = await ParseParams(context);
            var options = t.Item1;
            var redirect = t.Item2;
            if (redirect)
            {
                return;
            }
            if (context.Request.Path == _options.Path)
            {
                RenderMainLogPage(options, context);
            }
            else
            {
                RenderDetailsPage(options, context);
            }
        }

        private async void RenderMainLogPage(ViewOptions options, HttpContext context)
        {
            var model = new LogPageModel()
            {
                Activities = _store.GetActivities(),
                Options = options,
                Path = _options.Path
            };
            var logPage = new LogPage(model);

            await logPage.ExecuteAsync(context);
        }

        private async void RenderDetailsPage(ViewOptions options, HttpContext context)
        {
            var parts = context.Request.Path.Value.Split('/');
            var id = Guid.Empty;
            if (!Guid.TryParse(parts[parts.Length - 1], out id))
            {
                context.Response.StatusCode = 400;
                await context.Response.WriteAsync("Invalid Id");
                return;
            }
            var model = new DetailsPageModel()
            {
                Activity = _store.GetActivities().Where(a => a.Id == id).FirstOrDefault(),
                Options = options
            };
            var detailsPage = new DetailsPage(model);
            await detailsPage.ExecuteAsync(context);
        }

        private async Task<Tuple<ViewOptions, bool>> ParseParams(HttpContext context)
        {
            var options = new ViewOptions()
            {
                MinLevel = LogLevel.Debug,
                NamePrefix = string.Empty
            };
            var isRedirect = false;

            IFormCollection form = null;
            if (context.Request.HasFormContentType)
            {
                form = await context.Request.ReadFormAsync();
            }

            if (form != null && form.ContainsKey("clear"))
            {
                _store.Clear();
                context.Response.Redirect(context.Request.PathBase.Add(_options.Path).ToString());
                isRedirect = true;
            }
            else
            {
                if (context.Request.Query.ContainsKey("level"))
                {
                    var minLevel = options.MinLevel;
                    if (Enum.TryParse<LogLevel>(context.Request.Query["level"], out minLevel))
                    {
                        options.MinLevel = minLevel;
                    }
                }
                if (context.Request.Query.ContainsKey("name"))
                {
                    var namePrefix = context.Request.Query["name"];
                    options.NamePrefix = namePrefix;
                }
            }
            return Tuple.Create(options, isRedirect);
        }
    }
}