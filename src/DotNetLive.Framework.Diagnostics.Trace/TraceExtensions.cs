using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using DotNetLive.Framework.Diagnostics.Trace;

namespace Microsoft.AspNetCore.Builder
{
    public static class TraceExtensions
    {
        /// <summary>
        /// Enables the Elm logging service, which can be accessed via the <see cref="ElmPageMiddleware"/>.
        /// </summary>
        public static IApplicationBuilder UseTraceCapture(this IApplicationBuilder app)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            // add the elm provider to the factory here so the logger can start capturing logs immediately
            var factory = app.ApplicationServices.GetRequiredService<ILoggerFactory>();
            var provider = app.ApplicationServices.GetRequiredService<TraceLoggerProvider>();
            factory.AddProvider(provider);

            return app.UseMiddleware<TraceCaptureMiddleware>();
        }

        /// <summary>
        /// Enables viewing logs captured by the <see cref="ElmCaptureMiddleware"/>.
        /// </summary>
        public static IApplicationBuilder UseTracePage(this IApplicationBuilder app)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            return app.UseMiddleware<TracePageMiddleware>();
        }
    }
}