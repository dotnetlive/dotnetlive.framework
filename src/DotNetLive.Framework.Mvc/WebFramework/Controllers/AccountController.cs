using DotNetLive.Framework.Mvc.UserIdentity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Net;

namespace DotNetLive.Framework.Mvc.WebFramework.Controllers
{
    [Authorize]
    public class BaseAccountController : Controller
    {
        private SecuritySettings _securitySettings;

        public BaseAccountController(IOptions<SecuritySettings> securitySetting)
        {
            this._securitySettings = securitySetting.Value;
        }

        [AllowAnonymous]
        public ActionResult Login(string returnUrl = "")
        {
            return Redirect($"{LoginUrl}/account/login?returnUrl={GetEncodeReturlUrl(returnUrl)}");
        }

        private string GetEncodeReturlUrl(string returnUrl)
        {
            var context = Request.HttpContext;
            return WebUtility.UrlEncode($"{context.Request.Scheme}://{context.Request.Host}{returnUrl}");
        }

        [AllowAnonymous]
        public ActionResult Register(string returnUrl)
        {
            return Redirect($"{LoginUrl}/account/register?returnUrl={ GetEncodeReturlUrl(returnUrl)}");
        }

        [Authorize]
        public ActionResult LogOff(string returnUrl = "")
        {
            return Redirect($"{LoginUrl}/account/logoff?returnUrl={GetEncodeReturlUrl(returnUrl)}");
        }

        [Authorize]
        public ActionResult Manager()
        {
            return Redirect($"{LoginUrl}/manager");
        }

        #region Util
        public string LoginUrl => _securitySettings.LoginUrl.TrimEnd('/');
        #endregion
    }
}