using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;

namespace DotNetLive.Framework.Diagnostics.Trace
{
    public class TraceCaptureMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly TraceOptions _options;
        private readonly ILogger _logger;

        public TraceCaptureMiddleware(RequestDelegate next, ILoggerFactory factory, IOptions<TraceOptions> options)
        {
            _next = next;
            _options = options.Value;
            _logger = factory.CreateLogger<TraceCaptureMiddleware>();
        }

        public async Task Invoke(HttpContext context)
        {
            //if (context.Request.Path.StartsWithSegments(_options.Path))
            //if (IsStaticResource(context.Request) || context.Request.Path.StartsWithSegments(_options.Path))
            //{
            //    await _next(context);
            //    return;
            //}

            using (RequestIdentifier.Ensure(context))
            {
                var requestId = context.Features.Get<IHttpRequestIdentifierFeature>().TraceIdentifier;
                using (var disObject = _logger.BeginScope("Request: {RequestId}", requestId))
                {
                    try
                    {
                        TraceScope.Current.Context.HttpInfo = GetHttpInfo(context);
                        await _next(context);
                    }
                    finally
                    {
                        TraceScope.Current.Context.HttpInfo.StatusCode = context.Response.StatusCode;
                    }
                }
            }
        }

        /// <summary>
        /// Takes the info from the given HttpContext and copies it to an HttpInfo object
        /// </summary>
        /// <returns>The HttpInfo for the current Trace context</returns>
        private static HttpInfo GetHttpInfo(HttpContext context)
        {
            return new HttpInfo()
            {
                RequestID = context.Features.Get<IHttpRequestIdentifierFeature>().TraceIdentifier,
                Host = context.Request.Host,
                ContentType = context.Request.ContentType,
                Path = context.Request.Path,
                Scheme = context.Request.Scheme,
                StatusCode = context.Response.StatusCode,
                User = context.User,
                Method = context.Request.Method,
                Protocol = context.Request.Protocol,
                Headers = context.Request.Headers,
                Query = context.Request.QueryString,
                Cookies = context.Request.Cookies
            };
        }
    }
}