using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.EntityFrameworkCore;
using ZM.Core.ApiItems;
using ZM.Core.Extensions;
using ZM.Core.Utilities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using ZM.Core.Options;
using Newtonsoft.Json;
using ZM.Core.Entitys;
using ZM.Core.DBContexts;

namespace ZM.Core.Middleware
{
    public class HttpContextAnalysisOptions
    {
        private readonly HttpOptions _httpOptions;
        private readonly IDistributedCache _cache;
        private readonly DbContext _dbContext;

        public HttpContextAnalysisOptions(IServiceCollection services)
        {
            var iServiceProvider = services.BuildServiceProvider();
            _cache = iServiceProvider.GetService<IDistributedCache>();
            _httpOptions = iServiceProvider.GetService<IOptionsSnapshot<HttpOptions>>().Value;
            _dbContext = iServiceProvider.GetService<EntityContext>();
        }

        public void Analysis(HttpContext context)
        {
            var headers = context.Request.Headers;
            var tokekey = string.Empty;
            if (headers.ContainsKey(_httpOptions.TokenName))
            {
                tokekey = headers[_httpOptions.TokenName];
                
            }
            
            var token = CacheHelper.GetToken(_cache, tokekey);
            UserToken userToken;
            if (string.IsNullOrEmpty(token))
            {
                userToken = new UserToken();
            }
            else
            {
                userToken = UserToken.Parse(token.AesDecrypt(_httpOptions.TokenKey));
            }
            var userroles = CacheHelper.GetUserRoles(_cache,userToken.userid.ToString());
            if (string.IsNullOrEmpty(userroles))
            {
                var roles = _dbContext.Set<SysUserRole>().Where(x => x.SysUserId == userToken.userid).Select(p => p.SysRoleId).ToList();
                userToken.roles = roles;
                CacheHelper.SetUserRoles(_cache,userToken.userid.ToString(), JsonConvert.SerializeObject(roles), _httpOptions.GetDistributedCacheEntryOptions());
            }
            else
            {
                userToken.roles = JsonConvert.DeserializeObject<List<Guid>>(userroles);
            }

            context.setUserToken(userToken);
            context.setRequestKey(DateTimeHelper.GetTimeStampMilliseconds().ToString() + Guid.NewGuid().ToString("N"));
        }


    }
}