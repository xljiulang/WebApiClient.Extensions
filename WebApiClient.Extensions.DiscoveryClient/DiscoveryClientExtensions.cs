using Microsoft.Extensions.DependencyInjection;
using Steeltoe.Common.Discovery;
using System;
using WebApiClient.Extensions.HttpClientFactory;

namespace WebApiClient.Extensions.DiscoveryClient
{
    /// <summary>
    /// 基于HttpClientFactory的DiscoveryClient扩展
    /// </summary>
    public static class DiscoveryClientExtensions
    {
        /// <summary>
        /// 添加Discovery的别名HttpClient
        /// </summary>
        /// <typeparam name="TInterface">接口类型</typeparam>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IHttpClientBuilder AddDiscoveryTypedClient<TInterface>(this IServiceCollection services)
            where TInterface : class, IHttpApi
        {
            return services.AddDiscoveryTypedClient<TInterface>(c => { });
        }

        /// <summary>
        /// 添加Discovery的别名HttpClient
        /// </summary>
        /// <typeparam name="TInterface">接口类型</typeparam>
        /// <param name="services"></param>
        /// <param name="configOptions">配置选项</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns></returns>
        public static IHttpClientBuilder AddDiscoveryTypedClient<TInterface>(this IServiceCollection services, Action<HttpApiConfig> configOptions)
            where TInterface : class, IHttpApi
        {
            if (configOptions == null)
            {
                throw new ArgumentNullException(nameof(configOptions));
            }

            return services.AddDiscoveryTypedClient<TInterface>((c, p) => configOptions.Invoke(c));
        }

        /// <summary>
        /// 添加Discovery的别名HttpClient
        /// </summary>
        /// <typeparam name="TInterface">接口类型</typeparam>
        /// <param name="services"></param>
        /// <param name="configOptions">配置选项</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns></returns>
        public static IHttpClientBuilder AddDiscoveryTypedClient<TInterface>(this IServiceCollection services, Action<HttpApiConfig, IServiceProvider> configOptions)
            where TInterface : class, IHttpApi
        {
            if (configOptions == null)
            {
                throw new ArgumentNullException(nameof(configOptions));
            }

            return services
                .AddHttpApiTypedClient<TInterface>(configOptions)
                .ConfigurePrimaryHttpMessageHandler(provider =>
                {
                    var discoveryClient = provider.GetService<IDiscoveryClient>();
                    return new DiscoveryHttpClientHandler(discoveryClient);
                });
        }
    }
}
