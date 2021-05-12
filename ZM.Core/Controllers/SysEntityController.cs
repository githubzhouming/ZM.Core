using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using ZM.Core.ApiItems;
using ZM.Core.DbContexts;
using ZM.Core.DBContexts;
using ZM.Core.Entitys;
using ZM.Core.Extensions;
using ZM.Core.Options;
using ZM.Core.Utilities;

namespace ZM.Core.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class SysEntityController : ControllerBase
    {
        private readonly DbContext _context;
        private readonly ILogger<TokenController> _logger;
        private readonly IDistributedCache _cache;
        private readonly HttpOptions _httpOptions;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public SysEntityController(EntityContext context, ILogger<TokenController> logger, IDistributedCache cache
            , IOptions<HttpOptions> httpOptions, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _logger = logger;
            _cache = cache;
            _httpOptions = httpOptions.Value;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpGet("getEntitys")]
        public virtual async Task<IActionResult> getEntitys()
        {
            ApiResult customResult = new ApiResult();
            try
            {
                var httpContext = _httpContextAccessor.HttpContext;
                var userToken = httpContext.getUserToken();
                var jsonStr = await _cache.GetStringAsync("EntityBaseChildTypePropertyInfos" + userToken?.userid);
                if (string.IsNullOrEmpty(jsonStr))
                {
                    var childTypePropertyInfos = ClassHelper.GetChildTypePropertyInfos(typeof(EntityBase));
                    var result = childTypePropertyInfos.Select(x => new EntityInfo()
                    {
                        Name = x.Key.Name
                        ,
                        PropertyInfos = x.Value?.Select(p => new EntitypPropertyInfo()
                        {
                            Name = p.Name
                            ,
                            TypeName =p.PropertyType.HasImplementedRawGeneric(typeof(Nullable<>))? p.PropertyType.GetGenericArguments()[0].Name:  p.PropertyType.Name
                        })
                    });
                    customResult.resultBody = result;
                    await _cache.SetAsync("EntityBaseChildTypePropertyInfos" + userToken?.userid, DataHelper.StringToBytes(JsonConvert.SerializeObject(result)), _httpOptions.GetDistributedCacheEntryOptions());
                }
                else
                {
                    customResult.resultBody = JsonConvert.DeserializeObject<IEnumerable<EntityInfo>>(jsonStr);
                }

                customResult.resultCode = 0;




                return Ok(customResult);

            }
            catch (Exception ex)
            {
                customResult.resultCode = ResultCodeEnum.Exception;
                customResult.resultBody = ex.ToString();
                return BadRequest(customResult);
            }
        }


    }
}
