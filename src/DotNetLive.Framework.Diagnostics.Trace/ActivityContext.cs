using System;

namespace DotNetLive.Framework.Diagnostics.Trace
{
    public class ActivityContext
    {
        public Guid Id { get; set; }

        public HttpInfo HttpInfo { get; set; }

        public ScopeNode Root { get; set; }

        public DateTimeOffset Time { get; set; }

        public bool IsCollapsed { get; set; }

        public bool RepresentsScope { get; set; }
    }
}