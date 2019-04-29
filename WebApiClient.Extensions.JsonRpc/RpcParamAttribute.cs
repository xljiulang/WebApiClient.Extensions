using System;
using System.Threading.Tasks;
using WebApiClient.Contexts;

namespace WebApiClient.Attributes
{
    /// <summary>
    /// 表示Rpc请求的一个参数
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = true)]
    public class RpcParamAttribute : Attribute, IApiParameterAttribute
    {
        /// <summary>
        /// 请求前
        /// </summary>
        /// <param name="context">请求上下文</param>
        /// <param name="parameter">参数</param>
        /// <returns></returns>
        public Task BeforeRequestAsync(ApiActionContext context, ApiParameterDescriptor parameter)
        {
            var jsonRpcContent = context.RequestMessage.Content as JsonRpcContent;
            if (jsonRpcContent == null)
            {
                throw new HttpApiConfigException($"请为接口方法{context.ApiActionDescriptor.Name}修饰{nameof(JsonRpcMethodAttribute)}");
            }
            else
            {
                jsonRpcContent.AddParameter(parameter);
            }
            return JsonRpc.CompletedTask;
        }
    }
}
