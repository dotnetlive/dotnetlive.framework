using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace DotNetLive.Framework.Diagnostics.Trace.Views
{
    public class LogPageModel
    {
        public IEnumerable<ActivityContext> Activities { get; set; }

        public ViewOptions Options { get; set; }

        public PathString Path { get; set; }
    }
}
