using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ZM.Core.DBContexts;
using ZM.Core.Entitys;

namespace ZM.Core.Plugins
{
    public static class EntityDatapPermissionExtensions
    {
        public static IServiceCollection AddEntityDatapPermission(this IServiceCollection services, Func<IServiceProvider, IEntityDatapPermission> setupAction)
        {
            services.AddSingleton<IEntityDatapPermission>(setupAction);
            return services;
        }
        public static IServiceCollection AddEntityDatapPermissionDefault(this IServiceCollection services)
        {
            var entityContext= services.BuildServiceProvider().GetRequiredService<EntityContext>();
            services.AddEntityDatapPermission((i) => new EntityDatapPermissionDefault(entityContext));
            return services;
        }
    }
}
