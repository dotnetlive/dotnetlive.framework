using System;
using System.Collections.Generic;
using System.Text;
using DotNetLive.Framework.Entities;
using DotNetLive.Framework.WebApiClient;

namespace DotNetLive.Framework.Mvc.UserIdentity
{
    public interface IUserApiClient
    {
        ApiResponse<Guid> Create(Account account);
        ApiResponse Update(Account account);
        ApiResponse<Account> GetAccount(Guid guid);
        ApiResponse<Account> GetAccountByNormalizedEmail(string normalizedUserEmail);
    }

    public class UserApiClient : IUserApiClient
    {
        public ApiResponse<Guid> Create(Account account)
        {
            throw new NotImplementedException();
        }

        public ApiResponse<Account> GetAccount(Guid guid)
        {
            throw new NotImplementedException();
        }

        public ApiResponse<Account> GetAccountByNormalizedEmail(string normalizedUserEmail)
        {
            throw new NotImplementedException();
        }

        public ApiResponse Update(Account account)
        {
            throw new NotImplementedException();
        }
    }
}
