using Microsoft.AspNetCore.Http.Features;

namespace DotNetLive.Framework.Diagnostics.Trace
{
    internal class HttpRequestIdentifierFeature : IHttpRequestIdentifierFeature
    {
        public string TraceIdentifier { get; set; }
    }
}