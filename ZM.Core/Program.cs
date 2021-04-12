using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Web;
using System;
using System.Configuration;
using System.Net;
using ZM.Core.Options;

namespace ZM.Test
{

    class Program
    {
        public static void Main(string[] args)
        {
            var logger = NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();
            try
            {
                logger.Debug("init main");
                CreateHostBuilder(args).Build().Run();
            }
            catch (Exception exception)
            {
                //NLog: catch setup errors
                logger.Error(exception, "Stopped program because of exception");
                throw;
            }
            finally
            {
                // Ensure to flush and stop internal timers/threads before application-exit (Avoid segmentation fault on Linux)
                NLog.LogManager.Shutdown();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            //.ConfigureAppConfiguration((hostingContext, config) =>
            //{
            //})
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.ConfigureKestrel((context, options) =>
                {
                    var httpOptions = new HttpOptions();
                    context.Configuration.GetSection(HttpOptions.Position).Bind(httpOptions);
                    //设置应用服务器Kestrel请求体最大为128MB
                    options.Limits.MaxRequestBodySize = httpOptions.FileSizeLimit;
                    options.ConfigureEndpointDefaults(listenOptions =>
                    {
                        //((System.Net.IPEndPoint)listenOptions.EndPoint).Port = 5001;
                    });
                });
                webBuilder.UseStartup<Startup>();
            })
            .ConfigureLogging(logging =>
            {
                logging.ClearProviders();
                logging.SetMinimumLevel(LogLevel.Trace);
                logging.AddConsole();
            })
                .UseNLog()
                ; // NLog: setup NLog for Dependency injection

    }
}
