using DotNetLive.Framework.DependencyManagement;
using DotNetLive.Framework.WebApi.WebFramework.Filters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace DotNetLive.Framework.WebApi.WebApi.DependencyRegister
{
    public class MvcDependencyRegister : IDependencyRegister
    {
        public ExecuteOrderType ExecuteOrder => ExecuteOrderType.Higher;

        public void Register(IServiceCollection services, IConfigurationRoot configuration, IServiceProvider serviceProvider)
        {
            services.AddMvc(setupAction =>
            {
                setupAction.Filters.Add(new GlobalExceptionAttribute() { Order = 99 });
                setupAction.Filters.Add(new GlobalDbTransactionAttribute() { Order = 0 });
            });

            // Add memory cache services.
            services.AddMemoryCache(setup =>
            {
                setup.ExpirationScanFrequency = TimeSpan.FromMinutes(1);
            });

            services.AddDistributedMemoryCache();
        }
    }
}
