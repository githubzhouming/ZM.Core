using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using ZM.Core.Options;

namespace ZM.Core.Middleware
{
    public static class IPCheckMiddlewareExtensions
    {
        /// <summary>
        /// 使用IP检查
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseIPCheckMiddleware(
            this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<IPCheckMiddleware>();
        }
        /// <summary>
        /// 注册IP检查
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static void AddIIPCheckOptions(this IServiceCollection services)
        {
            var iServiceProvider = services.BuildServiceProvider();
            var configuration = iServiceProvider.GetService<IConfiguration>();
            services.Configure<HttpOptions>(configuration.GetSection(HttpOptions.Position));
            IPCheckOptions options = new IPCheckOptions(services);
            services.AddSingleton<IPCheckOptions>(options);

        }
    }
}