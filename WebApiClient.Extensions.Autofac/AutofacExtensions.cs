
using Autofac;
using System;
using System.Linq;
using System.Reflection;

namespace WebApiClient.Extensions.Autofac
{
    /// <summary>
    /// 基于Autofac的扩展
    /// </summary>
    public static class AutofacExtensions
    {
        /// <summary>
        /// RegisterHttpApi方法名称
        /// </summary>
        private const string registerHttpApiMethName = "RegisterHttpApi";

        /// <summary>
        /// ConfigureHttpApiConfig方法名称
        /// </summary>
        private const string configureHttpApiConfigMethName = "ConfigureHttpApiConfig";

        /// <summary>
        /// 注册HttpApi
        /// 返回HttpApi工厂创建器
        /// </summary>
        /// <typeparam name="TInterface">接口类型</typeparam>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static HttpApiFactoryBuilder<TInterface> RegisterHttpApi<TInterface>(this ContainerBuilder builder)
            where TInterface : class, IHttpApi
        {
            return new HttpApiFactoryBuilder<TInterface>(builder);
        }

        /// <summary>
        /// 通过Type类型注册到autofa
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="type">接口类型</param>
        /// <param name="configOptions">配置项</param>
        public static void RegisterApiByType(this ContainerBuilder builder,Type type, Action<HttpApiConfig> configOptions)
        {
            var registerApi = typeof(WebApiClient.Extensions.Autofac.AutofacExtensions).GetMethod(registerHttpApiMethName).MakeGenericMethod(type);
            var configureHttpApiConfig = registerApi.Invoke(null, new[] { builder });

            var configMethon = configureHttpApiConfig.GetType().GetMethod(configureHttpApiConfigMethName, new[] { typeof(System.Action<WebApiClient.HttpApiConfig>) });
            configMethon.Invoke(configureHttpApiConfig, new[] { configOptions });
        }

        /// <summary>
        /// 通过程序集注册类型
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="assembly"></param>
        /// <param name="configOptions"></param>
        public static void RegisterApiByAssembly(this ContainerBuilder builder, Assembly assembly, Action<HttpApiConfig> configOptions)
        {
            builder.RegisterApiByAssembly(new Assembly[] { assembly }, configOptions);
        }

            /// <summary>
            /// 通过程序集注册类型
            /// </summary>
            /// <param name="builder"></param>
            /// <param name="assembliesArray"></param>
            /// <param name="configOptions"></param>
            public static void RegisterApiByAssembly(this ContainerBuilder builder,Assembly[] assembliesArray, Action<HttpApiConfig> configOptions)
        {
            foreach(var assemblies in assembliesArray)
            {
                registerApiByAssembly(builder, assemblies, configOptions);
            }
        }

        private static void registerApiByAssembly(ContainerBuilder builder,Assembly assemblies, Action<HttpApiConfig> configOptions)
        {
            var httpApiType = typeof(IHttpApi);

            var httpApiList = assemblies.GetTypes().Where(p => httpApiType.IsAssignableFrom(p)).ToList();

            foreach (var httpApi in httpApiList)
            {
                builder.RegisterApiByType(httpApi,  configOptions);
            }
        }
    }
}
