using Microsoft.AspNetCore.Mvc;
using System.Linq;
using DotNetLive.Framework.Models;
using System;

namespace DotNetLive.Framework.WebFramework.Controllers
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