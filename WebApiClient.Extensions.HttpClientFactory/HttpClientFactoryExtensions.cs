using Microsoft.Extensions.DependencyInjection;
using System;

namespace WebApiClient.Extensions.HttpClientFactory
{
    /// <summary>
    /// 基于HttpClientFactory的扩展
    /// </summary>
    public static class HttpClientFactoryExtensions
    {
        /// <summary>
        /// 添加HttpApiClient的别名HttpClient
        /// </summary>
        /// <typeparam name="TInterface">接口类型</typeparam>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IHttpClientBuilder AddHttpApiTypedClient<TInterface>(this IServiceCollection services)
            where TInterface : class, IHttpApi
        {
            return services.AddHttpApiTypedClient<TInterface>(c => { });
        }

        /// <summary>
        /// 添加HttpApiClient的别名HttpClient
        /// </summary>
        /// <typeparam name="TInterface">接口类型</typeparam>
        /// <param name="services"></param>
        /// <param name="configOptions">配置选项</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns></returns>
        public static IHttpClientBuilder AddHttpApiTypedClient<TInterface>(this IServiceCollection services, Action<HttpApiConfig> configOptions)
            where TInterface : class, IHttpApi
        {
            if (configOptions == null)
            {
                throw new ArgumentNullException(nameof(configOptions));
            }

            return services.AddHttpApiTypedClient<TInterface>((c, p) => configOptions.Invoke(c));
        }

        /// <summary>
        /// 添加HttpApiClient的别名HttpClient
        /// </summary>
        /// <typeparam name="TInterface">接口类型</typeparam>
        /// <param name="services"></param>
        /// <param name="configOptions">配置选项</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns></returns>
        public static IHttpClientBuilder AddHttpApiTypedClient<TInterface>(this IServiceCollection services, Action<HttpApiConfig, IServiceProvider> configOptions)
            where TInterface : class, IHttpApi
        {
            if (configOptions == null)
            {
                throw new ArgumentNullException(nameof(configOptions));
            }

            return services
                .AddHttpClient(typeof(TInterface).ToString())
                .AddTypedClient((httpClient, provider) =>
                {
                    var httpApiConfig = new HttpApiConfig(httpClient)
                    {
                        ServiceProvider = provider
                    };
                    configOptions.Invoke(httpApiConfig, provider);
                    return HttpApi.Create<TInterface>(httpApiConfig);
                });
        }
    }
}
