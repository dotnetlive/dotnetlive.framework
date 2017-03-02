using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace DotNetLive.Framework.Diagnostics.Trace
{
    public class HttpInfo
    {
        public string RequestID { get; set; }

        public HostString Host { get; set; }

        public PathString Path { get; set; }

        public string ContentType { get; set; }

        public string Scheme { get; set; }

        public int StatusCode { get; set; }

        public ClaimsPrincipal User { get; set; }

        public string Method { get; set; }

        public string Protocol { get; set; }

        public IHeaderDictionary Headers { get; set; }

        public QueryString Query { get; set; }

        public IRequestCookieCollection Cookies { get; set; }
    }
}