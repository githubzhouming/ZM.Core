using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ZM.Core.ApiItems;
using ZM.Core.DBContexts;
using ZM.Core.Entitys;
using ZM.Core.Extensions;
using ZM.Core.Options;
using ZM.Core.Utilities;

namespace ZM.Core.Controllers
{
    [ApiController]
    public class TokenController : ControllerBase
    {
        private readonly DbContext _context;
        private readonly ILogger<TokenController> _logger;
        private readonly IDistributedCache _cache;
        private readonly HttpOptions _httpOptions;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public TokenController(EntityContext context, ILogger<TokenController> logger, IDistributedCache cache
            , IOptions<HttpOptions> httpOptions, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _logger = logger;
            _cache = cache;
            _httpOptions = httpOptions.Value;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpPost("getToken")]
        public virtual async Task<IActionResult> getLoginToken(UserToken item)
        {
            ApiResult customResult = new ApiResult();
            try
            {
                if (string.IsNullOrEmpty(item.username))
                {
                    customResult.resultCode = ResultCodeEnum.InvalidParameter;
                    customResult.resultBody = "The username is empty";
                    return BadRequest(customResult);
                }
                if (item.timestamp == null)
                {
                    customResult.resultCode = ResultCodeEnum.InvalidParameter;
                    customResult.resultBody = "The timestamp is empty";
                    return BadRequest(customResult);
                }
                if (string.IsNullOrEmpty(item.sign))
                {
                    customResult.resultCode = ResultCodeEnum.InvalidParameter;
                    customResult.resultBody = "The sign is empty";
                    return BadRequest(customResult);
                }
                var datetime1 = DateTimeHelper.ConvertTimestampSeconds(item.timestamp.Value);
                var dateNow = DateTime.Now;
                if (datetime1 < dateNow.AddMinutes(-5) || datetime1 > dateNow.AddMinutes(5))
                {
                    customResult.resultCode = ResultCodeEnum.InvalidParameter;
                    customResult.resultBody = "The timestamp over time";
                    return BadRequest(customResult);
                }
                
                var users = await _context.Set<SysUser>().Where(a => a.Name == item.username).ToArrayAsync();

                if (users == null || users.Length == 0)
                {
                    customResult.resultCode = ResultCodeEnum.InvalidParameter;
                    customResult.resultBody = "username does not exist";
                    return BadRequest(customResult);
                }
                if (users.Length > 1)
                {
                    customResult.resultCode = ResultCodeEnum.InvalidParameter;
                    customResult.resultBody = "username repetition";
                    return BadRequest(customResult);
                }
                var authInfo = users.First();
                if ((item.username + authInfo.Password + item.timestamp).MD5Encrypt() != item.sign)
                {
                    customResult.resultCode = ResultCodeEnum.InvalidParameter;
                    customResult.resultBody = "sign verification failed";
                    return BadRequest(customResult);
                }
                var tokenkey =await CacheHelper.GetUserTokenkeyAsync(_cache,authInfo.Id.ToString());
                var token =await CacheHelper.GetTokenAsync(_cache,tokenkey);
                UserToken userToken = null;
                var AESKey = _httpOptions.TokenKey;
                if (string.IsNullOrEmpty(token))
                {
                    userToken = new UserToken()
                    {
                        userid = authInfo.Id,
                        username = authInfo.Name,
                        timestamp = DateTimeHelper.GetTimeStampSeconds(),
                    };
                    tokenkey = userToken.tokenkey = userToken.ToString().MD5Encrypt();

                    token = userToken.ToString().AesEncrypt(AESKey);
                }
                else
                {
                    userToken = UserToken.Parse(token.AesDecrypt(AESKey));
                }

                //缓存信息

                await CacheHelper.SetTokenAsync(_cache,tokenkey,token,_httpOptions.GetDistributedCacheEntryOptions());
                await CacheHelper.SetUserTokenkeyAsync(_cache, userToken.userid.ToString(), tokenkey, _httpOptions.GetDistributedCacheEntryOptions());
                customResult.resultCode = 0;
                customResult.resultBody = new { tokenkey = tokenkey, userid = userToken.userid, username = item.username };

                var httpContext = _httpContextAccessor.HttpContext;
                httpContext.Response.Headers[_httpOptions.TokenName] = tokenkey;
                httpContext.Request.Headers[_httpOptions.TokenName] = tokenkey;



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
