using System.Threading;
using System.Threading.Tasks;

namespace WebApiClient
{
    /// <summary>
    /// 提供JsonRpc的一些常量
    /// </summary>
    static class JsonRpc
    {
        /// <summary>
        /// id值
        /// </summary>
        private static int @id = 0;

        /// <summary>
        /// 内容类型描述
        /// </summary>
        public static readonly string ContentType = "application/json-rpc";

#if NET45
        /// <summary>
        /// 表示已完成的任务
        /// </summary>
        public static readonly Task CompletedTask = Task.FromResult<object>(null);
#else
        /// <summary>
        /// 表示已完成的任务
        /// </summary>
        public static readonly Task CompletedTask = Task.CompletedTask;
#endif

        /// <summary>
        /// 返回新的id
        /// </summary>
        /// <returns></returns>
        public static int NewId()
        {
            return Interlocked.Increment(ref @id);
        }
    }
}
