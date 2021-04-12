using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.Text;

namespace ZM.Core.Options
{
    public class HttpOptions
    {
        /// <summary>
        /// 配置名称
        /// </summary>
        public const string Position = "HttpOptions";
        /// <summary>
        /// 加密key
        /// </summary>
        public string TokenKey { get; set; }
        /// <summary>
        /// 存储加密的名称
        /// </summary>
        public string TokenName { get; set; }
        /// <summary>
        /// 文件最大 比特
        /// </summary>
        public long FileSizeLimit { get; set; }
        /// <summary>
        /// 缓存时间 秒
        /// </summary>
        public long CacheExpiredSecond { get; set; }
        /// <summary>
        /// 忽略检查的文件后缀名
        /// 样例 "\\.(html|htm|css|js|json|xml|txt|gif|png|jpg|jpeg|ico)($|\\?)";
        /// </summary>
        public string IgnoreExpressionPattern { get; set; }
        private DistributedCacheEntryOptions _options;

        public DistributedCacheEntryOptions GetDistributedCacheEntryOptions()
        {
            if (_options == null)
            {
                _options = new DistributedCacheEntryOptions();
                _options.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(CacheExpiredSecond);
            }
            return _options;
        }
        public static IHttpContextAccessor Accessor;
        public static HttpContext GetContext()
        {
            return Accessor.HttpContext;
        }
    }
}
