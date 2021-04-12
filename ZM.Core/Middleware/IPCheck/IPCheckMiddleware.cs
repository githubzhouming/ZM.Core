using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

using System.Text;
using ZM.Core.Extensions;
using ZM.Core.ApiItems;
using Newtonsoft.Json;
using Microsoft.Extensions.Options;
using ZM.Core.Options;

namespace ZM.Core.Middleware
{
    /// <summary>
    /// Ip地址检查
    /// </summary>
    public class IPCheckMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<IPCheckMiddleware> _logger;
        private readonly IPCheckOptions _options;

        public IPCheckMiddleware(
            RequestDelegate next,
            ILogger<IPCheckMiddleware> logger,
            IPCheckOptions options)
        {
            _next = next;
            _logger = logger;
            _options = options;
        }

        public async Task InvokeAsync(HttpContext context)
        {
           
            var (isBlocked,ip) =  await _options.CheakIp(context);

            if (!isBlocked)
            {
                ApiResult customResult = new ApiResult();
                customResult.resultCode = ResultCodeEnum.InvalidIP;
                customResult.resultBody = $"URL is not on the whitelist: {ip}";
                _logger.LogDebug($"URL is not on the whitelist: {ip}");
                context.Response.ContentType = "application/json; charset=utf-8";
                await context.Response.WriteAsync(JsonConvert.SerializeObject(customResult));
                return;
            }

            await _next.Invoke(context);
        }
        

    }

}