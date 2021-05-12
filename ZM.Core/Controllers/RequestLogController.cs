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
    public class RequestLogController : EntityController<RequestLog, RequestLogController>
    {
        private readonly ILogger<RequestLogController> _logger;
        private readonly EntityContext _context;
        private readonly IDistributedCache _cache;

        public RequestLogController(EntityContext context, ILogger<RequestLogController> logger, IDistributedCache cache, IConfiguration config, IEntityDatapPermission entityDatapPermission)
            : base(logger, context, cache, entityDatapPermission)
        {
            _logger = logger;
            _cache = cache;
            _context = context;
        }
    }
}
