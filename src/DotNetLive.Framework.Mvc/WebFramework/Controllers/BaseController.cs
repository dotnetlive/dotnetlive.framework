using DotNetLive.Framework.Mvc.UserIdentity;
using DotNetLive.Framework.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;

namespace DotNetLive.Framework.Mvc.WebFramework.Controllers
{
    public abstract class BaseController : Controller
    {
        public string Token
        {
            get
            {
                if(!User.Identity.IsAuthenticated)
                {
                    throw new Exception("你没有登陆");
                }
                return User.Claims.SingleOrDefault(x => x.Type == ApplicationUser.JwtClaimName)?.Value;
            }
        }
    }
}