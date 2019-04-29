namespace WebApiClient
{
    /// <summary>
    /// 表示JsonRpc的异常
    /// </summary>
    public class JsonRpcException : HttpApiException
    {
        /// <summary>
        /// 获取异常内容
        /// </summary>
        public JsonRpcError Error { get; }

        /// <summary>
        /// JsonRpc异常
        /// </summary>
        /// <param name="error">错误内容</param>
        public JsonRpcException(JsonRpcError error)
            : base(error?.Message)
        {
            this.Error = error;
        }
    }
}
