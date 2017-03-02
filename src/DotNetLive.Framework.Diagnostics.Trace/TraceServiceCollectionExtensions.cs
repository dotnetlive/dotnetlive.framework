using System;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.DependencyInjection;
using DotNetLive.Framework.Diagnostics.Trace;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Extension methods for setting up Elm services in an <see cref="IServiceCollection" />.
    /// </summary>
    public static class TraceServiceCollectionExtensions
    {
        /// <summary>
        /// Adds error logging middleware services to the specified <see cref="IServiceCollection" />.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection" /> to add services to.</param>
        /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
        public static IServiceCollection AddTrace(this IServiceCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            services.AddOptions();
            services.TryAddSingleton<TraceStore>();
            services.TryAddSingleton<TraceLoggerProvider>();

            return services;
        }

        /// <summary>
        /// Adds error logging middleware services to the specified <see cref="IServiceCollection" />.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection" /> to add services to.</param>
        /// <param name="setupAction">An <see cref="Action{ElmOptions}"/> to configure the provided <see cref="ElmOptions"/>.</param>
        /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
        public static IServiceCollection AddTrace(this IServiceCollection services, Action<TraceOptions> setupAction)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (setupAction == null)
            {
                throw new ArgumentNullException(nameof(setupAction));
            }

            services.AddTrace();
            services.Configure(setupAction);

            return services;
        }
    }
}