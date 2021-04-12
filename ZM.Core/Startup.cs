using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using NLog.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZM.Core.DBContexts;
using ZM.Core.Middleware;
using Microsoft.AspNetCore.Http;
using ZM.Core.Options;

namespace ZM.Test
{
    public class Startup
    {
        readonly string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            _env = env;
        }

        public IConfiguration Configuration { get; }
        private readonly Microsoft.AspNetCore.Hosting.IWebHostEnvironment _env;

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<FormOptions>(options =>
            {
                options.MultipartBodyLengthLimit = long.MaxValue;
            });

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = _env.ApplicationName, Version = "v1" });
            });
            services.AddSwaggerGenNewtonsoftSupport(); // explicit opt-in - needs to be placed after AddSwaggerGen()

            #region 跨域配置
            var coresUrls = Configuration.GetValue<string>("CoresUrls");
            if (string.IsNullOrEmpty(coresUrls))
            {
                coresUrls = Configuration["AllowedHosts"];
            }
            services.AddCors(options =>
            {
                options.AddPolicy(MyAllowSpecificOrigins,
                builder =>
                {
                    builder.WithOrigins(coresUrls.Split(','))
                                        .AllowAnyHeader()
                                        .AllowAnyMethod()
                                        .SetIsOriginAllowedToAllowWildcardSubdomains();
                });
            });
            #endregion
            #region 数据库配置

            var pgSqlString = Configuration.GetValue<string>("PgSqlStr");
            var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddNLog(Configuration)
                .SetMinimumLevel(LogLevel.Trace)
                .AddConsole()
                ;
            });

            services.AddEntityFrameworkNpgsql();
            services.AddDbContextPool<EntityContext>((serviceProvider, optionsBuilder) => {
                optionsBuilder.UseNpgsql(pgSqlString)
                .UseLoggerFactory(loggerFactory)
                //.UseInternalServiceProvider(serviceProvider)
                ;
            }, 1024);
            //services.AddTransient<EntityContext>((iServiceProvider) =>
            //{

            //    var dbContextOption = new DbContextOptions<DbContext>();
            //    var loggerFactory = LoggerFactory.Create(builder =>
            //    {
            //        builder.AddNLog(Configuration)
            //        .SetMinimumLevel(LogLevel.Trace)
            //        .AddConsole()
            //        ;
            //    });
            //    var dbContextOptionBuilder = new DbContextOptionsBuilder<DbContext>(dbContextOption)
            //            //.UseMySQL(mySqlString)
            //            .UseNpgsql(pgSqlString)
            //            //.UseSqlServer(sqlServerString)
            //            .UseLoggerFactory(loggerFactory)
            //            ;
            //    var _dbContext = new EntityContext(dbContextOptionBuilder.Options);
            //    return _dbContext;
            //});
            #endregion
            #region Redis Session
            var RedisConnStr = Configuration.GetValue<string>("RedisStr");


            //
            // services.AddDistributedRedisCache(options =>
            // {
            //     options.Configuration = RedisConnStr;
            //     options.InstanceName = _env.ApplicationName;
            // });
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = RedisConnStr;
                options.InstanceName = _env.ApplicationName;
            });
            //添加Session并设置过期时长
            //Session 过期时长分钟

            // services.AddSession(options => { options.IdleTimeout = TimeSpan.FromMinutes(sessionOutTime); });

            #endregion

            #region 自定义中间件
            services.AddHttpContextAnalysis();
            services.AddIIPCheckOptions();
            services.AddCustomeAuthentication();
            #endregion

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHttpContextAccessor accessor)
        {
            HttpOptions.Accessor = accessor;
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(
                    c => 
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", $"{env.ApplicationName} v1")
                    
                    );
                
            }
            #region 自定义中间件
            app.UseHttpContextAnalysisMiddleware();
            app.UseIPCheckMiddleware();
            app.UseCustomeAuthenticationMiddleware();
            #endregion

            //app.UseHttpsRedirection();

            {
                DefaultFilesOptions options = new DefaultFilesOptions();
                options.DefaultFileNames.Clear();
                options.DefaultFileNames.Add("index.html");
                app.UseDefaultFiles(options);
            }
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(
                    Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"))
                    ,
                RequestPath = ""
            });
            app.UseRouting();
            app.UseCors(MyAllowSpecificOrigins);

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}