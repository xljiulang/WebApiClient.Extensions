
using Autofac;

namespace WebApiClient.Extensions.Autofac
{
    /// <summary>
    /// 基于Autofac的扩展
    /// </summary>
    public static class AutofacExtensions
    {
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
    }
}
