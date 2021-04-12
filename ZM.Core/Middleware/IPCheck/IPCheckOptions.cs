using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;

using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Net.Http;
using System.Text;
using ZM.Core.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using ZM.Core.Options;
using ZM.Core.DBContexts;
using ZM.Core.Entitys;

namespace ZM.Core.Middleware
{
    public class IPCheckOptions
    {
        private readonly DbContext _dbContext;
        private readonly HttpOptions _httpOptions;
        public IPCheckOptions(IServiceCollection services)
        {
            var iServiceProvider = services.BuildServiceProvider();
            _dbContext = iServiceProvider.GetService<EntityContext>();
            _httpOptions= iServiceProvider.GetService<IOptions<HttpOptions>>()?.Value;
        }

        private bool isMatch(string value, IEnumerable<string> strList)
        {
            foreach (var pattern in strList)
            {
                if (Regex.IsMatch(value, pattern, RegexOptions.IgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }
        public virtual async Task<(bool,string)> CheakIp(HttpContext context)
        {
            string url = context.Request.GetAbsoluteUri();
            string path = context.Request.Path.ToString().ToLower();
            string method = context.Request.Method.ToLower();
            string ip = GetClientIp(context);

            {//
                //静态文件类型
                string pattern = _httpOptions.IgnoreExpressionPattern;
                if (Regex.IsMatch(path, pattern, RegexOptions.IgnoreCase))
                {
                    return (true, ip);
                }
            }

            var userToken = context.getUserToken();
            var requestKey = context.getRequestKey();
            var item = new RequestLog()
            {
                IP = ip,
                Method = method,
                Path = url,
            };
            var entityEntry = _dbContext.Entry(item);
            entityEntry.State = EntityState.Added;
            var result = await _dbContext.SaveChangesAsync();

            //ip不存在直接返回false
            if (string.IsNullOrEmpty(ip))
            {
                return (false,ip);
            }
            return (true,ip);

        }

        private static StringBuilder getHead(HttpContext context)
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach (var head in context.Request.Headers)
            {
                stringBuilder.AppendLine($"{head.Key}:{head.Value}");
            }
            return stringBuilder;
        }
        private static string GetClientIp(HttpContext context)
        {
            //如果一个 HTTP 请求到达服务器之前，经过了三个代理 Proxy1、Proxy2、Proxy3
            //，IP 分别为 IP1、IP2、IP3，用户真实 IP 为 IP0
            //结果为 X-Forwarded-For: IP0, IP1, IP2
            var ip = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
            if (!string.IsNullOrEmpty(ip))
            {
                if (ip.IndexOf(",") != -1)
                {
                    var spArr = ip.Replace(" ", string.Empty).Split(',');
                    ip = spArr[0];
                }
            }
            if (string.IsNullOrEmpty(ip))
            {
                //ip = context.Connection.RemoteIpAddress.ToString();
                ip = context.Connection.RemoteIpAddress.MapToIPv4().ToString();
                // ip = context.Connection.RemoteIpAddress.MapToIPv6().ToString();
            }
            return ip;
        }
    }
}