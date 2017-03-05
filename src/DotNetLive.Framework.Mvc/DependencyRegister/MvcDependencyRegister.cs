using DotNetLive.Framework.DependencyManagement;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace DotNetLive.Framework.Mvc.DependencyRegister
{
    public class MvcDependencyRegister : IDependencyRegister
    {
        public ExecuteOrderType ExecuteOrder => ExecuteOrderType.Higher;

        public void Register(IServiceCollection services, IConfigurationRoot configuration, IServiceProvider serviceProvider)
        {
            // Add memory cache services.
            services.AddMemoryCache(setup =>
            {
                setup.ExpirationScanFrequency = TimeSpan.FromMinutes(1);
            });

            services.AddDistributedMemoryCache();
        }
    }
}
