using ProtoBuf;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using WebApiClient.Contexts;

namespace WebApiClient.Attributes
{
    /// <summary>
    /// 表示使用Protobuf反序列化回复内容作为返回值
    /// </summary>
    public class ProtobufReturnAttribute : ApiReturnAttribute
    {
        /// <summary>
        /// 获取或设置Accept请求头
        /// 默认为application/x-protobuf
        /// </summary>
        public string Accept { get; set; } = "application/x-protobuf";

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
            var stream = await context.ResponseMessage.Content.ReadAsStreamAsync();
            return Serializer.NonGeneric.Deserialize(
                context.ApiActionDescriptor.Return.DataType.Type,
                stream);
        }
    }
}
