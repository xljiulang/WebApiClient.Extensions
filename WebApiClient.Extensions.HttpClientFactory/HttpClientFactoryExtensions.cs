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
            return services.AddHttpApiTypedClient<TInterface>(default(Action<HttpApiConfig, IServiceProvider>));
        }

        /// <summary>
        /// 添加HttpApiClient的别名HttpClient
        /// </summary>
        /// <typeparam name="TInterface">接口类型</typeparam>
        /// <param name="services"></param>
        /// <param name="config">http接口的配置</param>
        /// <returns></returns>
        public static IHttpClientBuilder AddHttpApiTypedClient<TInterface>(this IServiceCollection services, Action<HttpApiConfig> config)
            where TInterface : class, IHttpApi
        {
            return services.AddHttpApiTypedClient<TInterface>((c, p) => config?.Invoke(c));
        }

        /// <summary>
        /// 添加HttpApiClient的别名HttpClient
        /// </summary>
        /// <typeparam name="TInterface">接口类型</typeparam>
        /// <param name="services"></param>
        /// <param name="config">http接口的配置</param>
        /// <returns></returns>
        public static IHttpClientBuilder AddHttpApiTypedClient<TInterface>(this IServiceCollection services, Action<HttpApiConfig, IServiceProvider> config)
            where TInterface : class, IHttpApi
        {
            return services
                .AddHttpClient<TInterface>()
                .AddTypedClient((httpClient, provider) =>
                {
                    var httpApiConfig = new HttpApiConfig(httpClient);
                    config?.Invoke(httpApiConfig, provider);
                    return HttpApiClient.Create<TInterface>(httpApiConfig);
                });
        }
    }
}
