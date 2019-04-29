using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using WebApiClient.Attributes;
using WebApiClient.Contexts;

namespace WebApiClient
{
    /// <summary>
    /// 表示JsonRpc请求内容
    /// </summary>
    class JsonRpcContent : HttpContent
    {
        /// <summary>
        /// 请求上下文
        /// </summary>
        private readonly ApiActionContext context;

        /// <summary>
        /// 请求方法特性
        /// </summary>
        private readonly JsonRpcMethodAttribute jsonRpcMethod;

        /// <summary>
        /// 请求参数集合
        /// </summary>
        private readonly List<ApiParameterDescriptor> jsonRpcParameters = new List<ApiParameterDescriptor>();

        /// <summary>
        /// 请求二进制内容
        /// </summary>
        private byte[] byteArrayContent;

        /// <summary>
        /// JsonRpc请求内容
        /// </summary>
        /// <param name="context">请求上下文</param>
        /// <param name="jsonRpcMethod">请求方法特性</param>
        public JsonRpcContent(ApiActionContext context, JsonRpcMethodAttribute jsonRpcMethod)
        {
            this.context = context;
            this.jsonRpcMethod = jsonRpcMethod;
            this.Headers.ContentType = new MediaTypeHeaderValue(jsonRpcMethod.ContentType ?? JsonRpc.ContentType);
        }

        /// <summary>
        /// 添加请求参数
        /// </summary>
        /// <param name="parameter">请求参数</param>
        public void AddParameter(ApiParameterDescriptor parameter)
        {
            this.jsonRpcParameters.Add(parameter);
        }

        /// <summary>
        /// 序列化到目标流
        /// </summary>
        /// <param name="stream">目标流</param>
        /// <param name="context"></param>
        /// <returns></returns>
        protected override Task SerializeToStreamAsync(Stream stream, TransportContext context)
        {
            if (this.byteArrayContent == null)
            {
                this.byteArrayContent = this.CreateByteArrayContent();
            }
            return stream.WriteAsync(this.byteArrayContent, 0, this.byteArrayContent.Length);
        }

        /// <summary>
        /// 计算数据长度
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        protected override bool TryComputeLength(out long length)
        {
            this.byteArrayContent = this.CreateByteArrayContent();
            length = this.byteArrayContent.Length;
            return true;
        }

        /// <summary>
        /// 创建请求数据内容
        /// </summary>
        /// <returns></returns>
        private byte[] CreateByteArrayContent()
        {
            var parameters = this.jsonRpcMethod.ParamsStyle == JsonRpcParamsStyle.Array ?
              (object)this.jsonRpcParameters.Select(item => item.Value).ToArray() :
              (object)this.jsonRpcParameters.ToDictionary(item => item.Name, item => item.Value);

            var jsonRpcRequest = new JsonRpcRequest
            {
                Id = JsonRpc.NewId(),
                Params = parameters,
                Method = this.jsonRpcMethod.Method ?? this.context.ApiActionDescriptor.Name
            };

            var options = this.context.HttpApiConfig.FormatOptions;
            var json = this.context.HttpApiConfig.JsonFormatter.Serialize(jsonRpcRequest, options);
            return Encoding.UTF8.GetBytes(json);
        }
    }
}
