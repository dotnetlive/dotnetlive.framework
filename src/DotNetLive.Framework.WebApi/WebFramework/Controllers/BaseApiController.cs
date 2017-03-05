using DotNetLive.Framework.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;

namespace DotNetLive.Framework.WebApi.WebFramework.Controllers
{
    public abstract class BaseApiController : Controller
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