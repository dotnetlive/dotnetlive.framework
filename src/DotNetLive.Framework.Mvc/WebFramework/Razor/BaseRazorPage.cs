using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Razor.Internal;
using System.Security.Principal;

namespace DotNetLive.Framework.Mvc.WebFramework.Razor
{
    public abstract class BaseRazorPage<T> : RazorPage<T>
    {
        //[RazorInjectAttribute]
        //public GlobalSettings GlobalSettings { get; private set; }

        public IIdentity CurrentIdentity => Context.User.Identity;

        public string CurrentRequestPath => Context.Request.Path.Value + Context.Request.QueryString.Value;
    }
}
