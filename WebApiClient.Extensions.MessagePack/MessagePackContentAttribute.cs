using MessagePack;
using MessagePack.Resolvers;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using WebApiClient.Contexts;

namespace WebApiClient.Attributes
{
    /// <summary>
    /// 表示使用MessagePack序列化参数值作为请求内容
    /// </summary>
    public class MessagePackContentAttribute : HttpContentAttribute
    {
        /// <summary>
        /// 格式化解析实例
        /// </summary>
        private IFormatterResolver formatterResolver = ContractlessStandardResolver.Instance;

        /// <summary>
        /// 获取或设置请求的内容类型
        /// 默认为application/x-msgpack
        /// </summary>
        public string ContentType { get; set; } = "application/x-msgpack";

        /// <summary>
        /// 获取或设置IFormatterResolver的类型
        /// 默认为ContractlessStandardResolver
        /// </summary>
        public Type FormatterResolverType
        {
            get => this.formatterResolver.GetType();
            set => this.formatterResolver = Activator.CreateInstance(value) as IFormatterResolver;
        }

        /// <summary>
        /// 设置请求Content
        /// </summary>
        /// <param name="context"></param>
        /// <param name="parameter"></param>
        protected override void SetHttpContent(ApiActionContext context, ApiParameterDescriptor parameter)
        {
            var bytes = MessagePackSerializer.NonGeneric.Serialize(
                parameter.ParameterType,
                parameter.Value,
                this.formatterResolver);

            var content = new ByteArrayContent(bytes);
            content.Headers.ContentType = new MediaTypeHeaderValue(this.ContentType);
            context.RequestMessage.Content = content;
        }
    }
}
