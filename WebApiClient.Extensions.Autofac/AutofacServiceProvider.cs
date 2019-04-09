using Autofac;
using System;

namespace WebApiClient.Extensions.Autofac
{
    /// <summary>
    /// 表示Autofac实现的ServiceProvider
    /// </summary>
    class AutofacServiceProvider : IServiceProvider
    {
        /// <summary>
        /// 组件上下文
        /// </summary>
        private readonly IComponentContext context;

        /// <summary>
        /// Autofac实现的ServiceProvider
        /// </summary>
        /// <param name="context">组件上下文</param>
        public AutofacServiceProvider(IComponentContext context)
        {
            this.context = context.Resolve<IComponentContext>();
        }

        /// <summary>
        /// 获取服务
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetService<T>()
        {
            return (T)this.GetService(typeof(T));
        }

        /// <summary>
        /// 获取服务
        /// </summary>
        /// <param name="serviceType">服务类型</param>
        /// <returns></returns>
        public object GetService(Type serviceType)
        {
            return this.context.ResolveOptional(serviceType);
        }
    }
}
