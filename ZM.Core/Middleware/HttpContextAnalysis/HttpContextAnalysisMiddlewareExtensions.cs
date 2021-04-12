using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;

using Microsoft.Extensions.Configuration;
using ZM.Core.DbContexts;
using ZM.Core.Options;

namespace ZM.Core.Middleware
{
    public static class HttpContextAnalysisMiddlewareExtensions
    {
        /// <summary>
        /// 解析HttpContex
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseHttpContextAnalysisMiddleware(
            this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<HttpContextAnalysisMiddleware>();
        }
        /// <summary>
        /// 注册分析token
        /// </summary>
        /// <param name="services"></param>
        public static void AddHttpContextAnalysis(this IServiceCollection services){
            var iServiceProvider = services.BuildServiceProvider();
            var configuration = iServiceProvider.GetService<IConfiguration>();
            services.Configure<HttpOptions>(configuration.GetSection(HttpOptions.Position));
            
            HttpContextAnalysisOptions options=new HttpContextAnalysisOptions(services);

            services.AddSingleton<HttpContextAnalysisOptions>(options);
        }
    }
}