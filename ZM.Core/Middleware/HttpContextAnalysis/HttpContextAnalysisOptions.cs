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

namespace ZM.Core.Middleware
{
    public class HttpContextAnalysisOptions
    {
        private readonly HttpOptions _httpOptions;
        private readonly IDistributedCache _cache;

        public HttpContextAnalysisOptions(IServiceCollection services)
        {
            var iServiceProvider = services.BuildServiceProvider();
            _cache = iServiceProvider.GetService<IDistributedCache>();
            _httpOptions = iServiceProvider.GetService<IOptionsSnapshot<HttpOptions>>().Value;
        }

        public void Analysis(HttpContext context)
        {
            var headers = context.Request.Headers;
            var tokekey = string.Empty;
            if (headers.ContainsKey(_httpOptions.TokenName))
            {
                tokekey = headers[_httpOptions.TokenName];
                
            }
            
            var token = _cache.GetString("token" + tokekey);
            UserToken userToken;
            if (string.IsNullOrEmpty(token))
            {
                userToken = new UserToken();
            }
            else
            {
                userToken = UserToken.Parse(token.AesDecrypt(_httpOptions.TokenKey));
            }
            context.setUserToken(userToken);
            context.setRequestKey(DateTimeHelper.GetTimeStampMilliseconds().ToString() + Guid.NewGuid().ToString("N"));
        }


    }
}