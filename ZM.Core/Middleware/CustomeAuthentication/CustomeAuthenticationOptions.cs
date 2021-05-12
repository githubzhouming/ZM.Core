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
using ZM.Core.DBContexts;
using ZM.Core.Entitys;
using ZM.Core.Utilities;
using Newtonsoft.Json;

namespace ZM.Core.Middleware
{
    public class CustomeAuthenticationOptions
    {
        private readonly IDistributedCache _cache;
        private readonly HttpOptions _httpOptions;
        private readonly DbContext _dbContext;
        public CustomeAuthenticationOptions(IServiceCollection services)
        {
            var iServiceProvider = services.BuildServiceProvider();
            _cache = iServiceProvider.GetService<IDistributedCache>();
            _httpOptions = iServiceProvider.GetService<IOptions<HttpOptions>>()?.Value;
            _dbContext = iServiceProvider.GetService<EntityContext>();
        }

        public virtual bool ignorePath(HttpContext context)
        {
            var userToken = context.getUserToken();
            if (userToken.IsAdmin())
            {
                return true;
            }
            string path = context.Request.Path.Value.ToLower();
            //静态文件类型
            string pattern = _httpOptions.IgnoreExpressionPattern;
            if (Regex.IsMatch(path, pattern, RegexOptions.IgnoreCase))
            {
                return true;
            }

            if (!userToken.roles.Any())
            {
                return false;
                
            }
            var actionPathStr= CacheHelper.GetUserActionPath(_cache, userToken.userid.ToString());
            List<string> actionPaths = null;
            if (string.IsNullOrEmpty(actionPathStr))
            {
                var filter = EntityFrameworkEx.GetFilterExpression<SysRoleActionView, Guid>("Id", userToken.roles);
                actionPaths = _dbContext.Set<SysRoleActionView>().Where(filter).Select(p => p.ActionPath).Distinct().ToList();
            }
            else
            {
                actionPaths = JsonConvert.DeserializeObject<List<string>>(actionPathStr);
            }

            foreach (var pa in actionPaths)
            {
                if (Regex.IsMatch(path, pa, RegexOptions.IgnoreCase))
                {
                    return true;
                }
            }

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