using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ZM.Core.DBContexts;
using ZM.Core.Entitys;

namespace ZM.Core.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class SysUserController : EntityController<SysUser, SysUserController>
    {
        private readonly ILogger<SysUserController> _logger;
        private readonly EntityContext _context;
        private readonly IDistributedCache _cache;

        public SysUserController(EntityContext context, ILogger<SysUserController> logger, IDistributedCache cache, IConfiguration config)
            : base(logger, context, cache)
        {
            _logger = logger;
            _cache = cache;
            _context = context;
        }
    }
}
