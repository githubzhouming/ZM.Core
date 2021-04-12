using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using ZM.Core.Options;

namespace ZM.Core.Middleware
{
    public static class CustomeAuthenticationMiddlewareExtensions
    {
        /// <summary>
        /// 使用请求路径检查
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseCustomeAuthenticationMiddleware(
            this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<CustomeAuthenticationMiddleware>();
        }
        /// <summary>
        /// 注册 请求路径检查
        /// </summary>
        /// <param name="services"></param>
        public static void AddCustomeAuthentication(this IServiceCollection services)
        {
            var iServiceProvider = services.BuildServiceProvider();
            var configuration = iServiceProvider.GetService<IConfiguration>();
            services.Configure<HttpOptions>(configuration.GetSection(HttpOptions.Position));

            CustomeAuthenticationOptions options = new CustomeAuthenticationOptions(services);

            services.AddSingleton<CustomeAuthenticationOptions>(options);
        }
    }
}