namespace WebApiClient
{
    /// <summary>
    /// 表示JsonRpc的请求返回结果
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    public class JsonRpcResult<TResult>
    {
        /// <summary>
        /// 请求id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// jsonrpc版本号
        /// </summary>
        public string JsonRpc { get; set; }

        /// <summary>
        /// 结果值
        /// </summary>
        public TResult Result { get; set; }

        /// <summary>
        /// 错误内容
        /// </summary>
        public JsonRpcError Error { get; set; }

        /// <summary>
        /// 确保Rpc结果没有错误
        /// </summary>
        /// <exception cref="JsonRpcException"></exception>
        /// <returns></returns>
        public JsonRpcResult<TResult> EnsureResultNoError()
        {
            if (this.Error != null)
            {
                throw new JsonRpcException(this.Error);
            }
            return this;
        }
    }
}

