using ProtoBuf;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using WebApiClient.Contexts;

namespace WebApiClient.Attributes
{
    /// <summary>
    /// 表示使用Protobuf序列化参数值作为请求内容
    /// </summary>
    public class ProtobufContentAttribute : HttpContentAttribute
    {
        /// <summary>
        /// 获取或设置请求的内容类型
        /// 默认为application/x-protobuf
        /// </summary>
        public string ContentType { get; set; } = "application/x-protobuf";

        /// <summary>
        /// 设置请求Content
        /// </summary>
        /// <param name="context"></param>
        /// <param name="parameter"></param>
        protected override void SetHttpContent(ApiActionContext context, ApiParameterDescriptor parameter)
        {
            var stream = new MemoryStream();
            if (parameter.Value != null)
            {
                Serializer.NonGeneric.Serialize(stream, parameter.Value);
                stream.Position = 0L;
            }

            var content = new StreamContent(stream);
            content.Headers.ContentType = new MediaTypeHeaderValue(this.ContentType);
            context.RequestMessage.Content = content;
        }
    }
}
