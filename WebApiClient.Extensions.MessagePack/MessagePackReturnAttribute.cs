using MessagePack;
using MessagePack.Resolvers;
using System;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using WebApiClient.Contexts;

namespace WebApiClient.Attributes
{
    /// <summary>
    /// 表示使用MessagePack反序列化回复内容作为返回值
    /// </summary>
    public class MessagePackReturnAttribute : ApiReturnAttribute
    {
        /// <summary>
        /// 格式化解析实例
        /// </summary>
        private IFormatterResolver formatterResolver = ContractlessStandardResolver.Instance;

        /// <summary>
        /// 获取或设置Accept请求头
        /// 默认为application/x-msgpack
        /// </summary>
        public string Accept { get; set; } = "application/x-msgpack";

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
        /// 配置Accept请求头
        /// </summary>
        /// <param name="accept">Accept请求头</param>
        protected override void ConfigureAccept(HttpHeaderValueCollection<MediaTypeWithQualityHeaderValue> accept)
        {
            accept.TryParseAdd(this.Accept);
        }

        /// <summary>
        /// 获取数据结果
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        protected override async Task<object> GetTaskResult(ApiActionContext context)
        {
            var bytes = await context.ResponseMessage.Content.ReadAsByteArrayAsync();

            return MessagePackSerializer.NonGeneric.Deserialize(
                context.ApiActionDescriptor.Return.DataType.Type,
                bytes,
                this.formatterResolver);
        }
    }
}
