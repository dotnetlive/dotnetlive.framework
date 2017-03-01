using DotNetLive.Framework.Security;

namespace DotNetLive.Framework.WebApiClient.Query
{
    public class LoginQuery : SessionQuery
    {
        public LoginQuery()
        {
            DeviceType = 1;
        }

        public LoginQuery(string sessionKey)
        {
            this.SessionKey = sessionKey;
        }

        public LoginQuery(LoginQuery query)
            : this()
        {
            if (query != null)
            {
                Email = query.Email;
                Password = query.Password;
                SessionKey = query.SessionKey;
                DeviceType = query.DeviceType;
            }
        }

        [Query(Name = "email")]
        public string Email { get; set; }

        public string Password { get; set; }

        [QueryAttribute(Name = "deviceType")]
        public int DeviceType { get; set; }

        [Query(Name = "passwordHash")]
        public string PasswordHash
        {
            // get { return Password; }
            get { return !string.IsNullOrEmpty(Password) ? MD5CryptoHelper.GetMD5Hash(Password) : null; }
        }

        /// <summary>
        /// 是否记住用户(延长sessionKey过期时间)
        /// </summary>
        [Query(Name = "isRemeber")]
        public bool IsRemeber { get; set; }
    }
}
