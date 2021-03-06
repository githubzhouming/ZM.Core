using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ZM.Core.DBContexts;
using ZM.Core.Entitys;
using ZM.Core.Plugins;

namespace ZM.Core.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class SysUserController : EntityController<SysUser, SysUserController>
    {
        private readonly ILogger<SysUserController> _logger;
        private readonly EntityContext _context;
        private readonly IDistributedCache _cache;

        public SysUserController(EntityContext context, ILogger<SysUserController> logger, IDistributedCache cache, IConfiguration config, IEntityDatapPermission entityDatapPermission)
            : base(logger, context, cache, entityDatapPermission)
        {
            _logger = logger;
            _cache = cache;
            _context = context;
        }
    }
}
