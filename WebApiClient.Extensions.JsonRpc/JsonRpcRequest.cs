using WebApiClient.DataAnnotations;

namespace WebApiClient
{
    /// <summary>
    /// 表示JsonRpc的请求实体
    /// </summary>
    class JsonRpcRequest
    {
        /// <summary>
        /// jsonrpc
        /// 2.0
        /// </summary>
        [AliasAs("jsonrpc")]
        public string JsonRpc { get; set; } = "2.0";

        /// <summary>
        /// 请求的方法名
        /// </summary>
        [AliasAs("method")]
        public string Method { get; set; }

        /// <summary>
        /// 请求的参数数组
        /// </summary>
        [AliasAs("params")]
        public object Params { get; set; }

        /// <summary>
        /// 请求的id
        /// </summary>
        [AliasAs("id")]
        public int Id { get; set; }
    }
}
