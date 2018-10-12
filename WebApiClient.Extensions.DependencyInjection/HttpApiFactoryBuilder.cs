using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;

namespace WebApiClient.Extensions.DependencyInjection
{
    /// <summary>
    /// HttpApi实例工厂创建器
    /// </summary>
    /// <typeparam name="TInterface"></typeparam>
    public class HttpApiFactoryBuilder<TInterface> where TInterface : class, IHttpApi
    {
        private Action<IServiceProvider, HttpApiConfig> configAction;

        private Func<IServiceProvider, HttpMessageHandler> handlerFunc;

        private TimeSpan lifeTime = TimeSpan.FromMinutes(2d);

        private TimeSpan cleanupInterval = TimeSpan.FromSeconds(10d);

        private bool keepCookieContainer = true;

        /// <summary>
        /// HttpApi实例工厂创建器
        /// </summary>
        /// <param name="services"></param>
        public HttpApiFactoryBuilder(IServiceCollection services)
        {
            services.AddSingleton<IHttpApiFactory<TInterface>, HttpApiFactory<TInterface>>(p =>
            {
                return new HttpApiFactory<TInterface>()
                .ConfigureHttpApiConfig(c => this.configAction?.Invoke(p, c))
                .ConfigureHttpMessageHandler(() => this.handlerFunc?.Invoke(p))
                .SetLifetime(this.lifeTime)
                .SetCleanupInterval(this.cleanupInterval)
                .SetKeepCookieContainer(this.keepCookieContainer);
            });

            services.AddTransient(p =>
            {
                var factory = p.GetRequiredService<IHttpApiFactory<TInterface>>();
                return factory.CreateHttpApi();
            });
        }

        /// <summary>
        /// 配置HttpApiConfig
        /// </summary>
        /// <param name="configAction">配置委托</param>
        /// <returns></returns>
        public HttpApiFactoryBuilder<TInterface> ConfigureHttpApiConfig(Action<IServiceProvider, HttpApiConfig> configAction)
        {
            this.configAction = configAction;
            return this;
        }

        /// <summary>
        /// 配置HttpMessageHandler的创建
        /// </summary>
        /// <param name="handlerFunc">创建委托</param>
        /// <returns></returns>
        public HttpApiFactoryBuilder<TInterface> ConfigureHttpMessageHandler(Func<IServiceProvider, HttpMessageHandler> handlerFunc)
        {
            this.handlerFunc = handlerFunc;
            return this;
        }

        /// <summary>
        /// 置HttpApi实例的生命周期
        /// </summary>
        /// <param name="lifeTime">生命周期</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <returns></returns>
        public HttpApiFactoryBuilder<TInterface> SetLifetime(TimeSpan lifeTime)
        {
            if (lifeTime <= TimeSpan.Zero)
            {
                throw new ArgumentOutOfRangeException(nameof(lifeTime));
            }
            this.lifeTime = lifeTime;
            return this;
        }

        /// <summary>
        /// 获取或设置清理过期的HttpApi实例的时间间隔
        /// </summary>
        /// <param name="interval">时间间隔</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <returns></returns>
        public HttpApiFactoryBuilder<TInterface> SetCleanupInterval(TimeSpan interval)
        {
            if (interval <= TimeSpan.Zero)
            {
                throw new ArgumentOutOfRangeException(nameof(interval));
            }
            this.cleanupInterval = interval;
            return this;
        }

        /// <summary>
        /// 设置是否维护使用一个CookieContainer实例 该实例为首次创建时的CookieContainer
        /// </summary>
        /// <param name="keep">true维护使用一个CookieContainer实例</param>
        /// <returns></returns>
        public HttpApiFactoryBuilder<TInterface> SetKeepCookieContainer(bool keep)
        {
            this.keepCookieContainer = keep;
            return this;
        }
    }
}
