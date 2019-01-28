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
        private bool keepCookieContainer = true;

        private TimeSpan lifeTime = TimeSpan.FromMinutes(2d);

        private TimeSpan cleanupInterval = TimeSpan.FromSeconds(10d);

        private Action<HttpApiConfig, IServiceProvider> configOptions;

        private Func<IServiceProvider, HttpMessageHandler> handlerFactory;


        /// <summary>
        /// HttpApi实例工厂创建器
        /// </summary>
        /// <param name="services"></param>
        public HttpApiFactoryBuilder(IServiceCollection services)
        {
            services.AddSingleton<IHttpApiFactory<TInterface>, HttpApiFactory<TInterface>>(p =>
            {
                return new HttpApiFactory<TInterface>()
                    .SetLifetime(this.lifeTime)
                    .SetCleanupInterval(this.cleanupInterval)
                    .SetKeepCookieContainer(this.keepCookieContainer)
                    .ConfigureHttpMessageHandler(() => this.handlerFactory?.Invoke(p));
            });

            services.AddTransient(p =>
            {
                var factory = p.GetRequiredService<IHttpApiFactory<TInterface>>();
                factory.ConfigureHttpApiConfig(c =>
                {
                    c.ServiceProvider = p;
                    this.configOptions?.Invoke(c, p);
                });
                return factory.CreateHttpApi();
            });
        }

        /// <summary>
        /// 配置HttpApiConfig
        /// </summary>
        /// <param name="configOptions">配置选项</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns></returns>
        public HttpApiFactoryBuilder<TInterface> ConfigureHttpApiConfig(Action<HttpApiConfig> configOptions)
        {
            if (configOptions == null)
            {
                throw new ArgumentNullException(nameof(configOptions));
            }
            return this.ConfigureHttpApiConfig((c, p) => configOptions.Invoke(c));
        }


        /// <summary>
        /// 配置HttpApiConfig
        /// </summary>
        /// <param name="configOptions">配置选项</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns></returns>
        public HttpApiFactoryBuilder<TInterface> ConfigureHttpApiConfig(Action<HttpApiConfig, IServiceProvider> configOptions)
        {
            this.configOptions = configOptions ?? throw new ArgumentNullException(nameof(configOptions));
            return this;
        }

        /// <summary>
        /// 配置HttpMessageHandler的创建
        /// </summary>
        /// <param name="handlerFactory">创建委托</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns></returns>
        public HttpApiFactoryBuilder<TInterface> ConfigureHttpMessageHandler(Func<HttpMessageHandler> handlerFactory)
        {
            if (handlerFactory == null)
            {
                throw new ArgumentNullException(nameof(handlerFactory));
            }
            return this.ConfigureHttpMessageHandler(p => handlerFactory.Invoke());
        }

        /// <summary>
        /// 配置HttpMessageHandler的创建
        /// </summary>
        /// <param name="handlerFactory">创建委托</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns></returns>
        public HttpApiFactoryBuilder<TInterface> ConfigureHttpMessageHandler(Func<IServiceProvider, HttpMessageHandler> handlerFactory)
        {
            this.handlerFactory = handlerFactory ?? throw new ArgumentNullException(nameof(handlerFactory));
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
