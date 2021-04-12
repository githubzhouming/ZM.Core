using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;

using Microsoft.EntityFrameworkCore;

using System.Threading.Tasks;
using ZM.Core.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ZM.Core.Options;
using Microsoft.Extensions.Options;

namespace ZM.Core.Middleware
{
    public class CustomeAuthenticationOptions
    {
        private readonly IDistributedCache _cache;
        private readonly HttpOptions _httpOptions;
        public CustomeAuthenticationOptions(IServiceCollection services)
        {
            var iServiceProvider = services.BuildServiceProvider();
            _cache = iServiceProvider.GetService<IDistributedCache>();
            _httpOptions = iServiceProvider.GetService<IOptions<HttpOptions>>()?.Value;
        }

        public virtual bool ignorePath(HttpContext context)
        {
            string path = context.Request.Path.Value.ToLower();
            //静态文件类型
            string pattern = _httpOptions.IgnoreExpressionPattern;
            if (Regex.IsMatch(path, pattern, RegexOptions.IgnoreCase))
            {
                return true;
            }
            //List<string> patternList = 
            //foreach (var pa in patternList)
            //{
            //    if (Regex.IsMatch(path, pa, RegexOptions.IgnoreCase))
            //    {
            //        return true;
            //    }
            //}

            return true;
        }
        public virtual bool checkAction(HttpContext context, out string result)
        {
            string path = context.Request.Path.Value.ToLower();
            string method = context.Request.Method.ToLower();
            result = $"{path},{method}";
            //var actionList = 
            //foreach (var pa in actionList)
            //{
            //    if (Regex.IsMatch(path, pa, RegexOptions.IgnoreCase))
            //    {
            //        return true;
            //    }
            //}
            return true;
        }

    }
}