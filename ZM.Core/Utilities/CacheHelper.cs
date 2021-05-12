using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ZM.Core.Utilities
{
    public static class CacheHelper
    {
        public const string _Tokenkey = "Tokenkey_";
        public const string _Token = "Token_";
        public const string _UserRoles = "UserRoles_";

        public const string _ActionPath = "ActionPath_";

        #region 
        private static Task<string> GetKeyAsync(IDistributedCache _cache, string key, string key2)
        {
            return _cache.GetStringAsync(key + key2);
        }
        private static string GetKey(IDistributedCache _cache, string key, string key2)
        {
            return _cache.GetString(key + key2);
        }
        private static Task SetKeyAsync(IDistributedCache _cache, string key, string key2, string value, DistributedCacheEntryOptions options = null)
        {
            if (options == null)
            {
                return _cache.SetStringAsync(key + key2, value);
            }
            else
            {
                return _cache.SetStringAsync(key + key2, value, options);
            }

        }
        private static void SetKey(IDistributedCache _cache, string key, string key2, string value, DistributedCacheEntryOptions options = null)
        {
            if (options == null)
            {
                _cache.SetString(key + key2, value);
            }
            else
            {
                _cache.SetString(key + key2, value, options);
            }
        }

        #endregion


        #region _tokenkey
        public static Task<string> GetUserTokenkeyAsync(IDistributedCache _cache, string key)
        {
            return GetKeyAsync(_cache, _Tokenkey, key);
        }
        public static string GetUserTokenkey(IDistributedCache _cache, string key)
        {
            return GetKey(_cache, _Tokenkey, key);
        }


        public static Task SetUserTokenkeyAsync(IDistributedCache _cache, string key, string value, DistributedCacheEntryOptions options = null)
        {
            return SetKeyAsync(_cache, _Tokenkey, key, value, options);
        }
        public static void SetUserTokenkey(IDistributedCache _cache, string key, string value, DistributedCacheEntryOptions options = null)
        {
            SetKey(_cache, _Tokenkey, key, value, options);
        }
        #endregion

        #region _token
        public static Task<string> GetTokenAsync(IDistributedCache _cache, string key)
        {
            return GetKeyAsync(_cache, _Token, key);
        }
        public static string GetToken(IDistributedCache _cache, string key)
        {
            return GetKey(_cache, _Token, key);
        }


        public static Task SetTokenAsync(IDistributedCache _cache, string key, string value, DistributedCacheEntryOptions options = null)
        {
            return SetKeyAsync(_cache, _Token, key, value, options);
        }
        public static void SetToken(IDistributedCache _cache, string key, string value, DistributedCacheEntryOptions options = null)
        {
            SetKey(_cache, _Token, key, value, options);
        }
        #endregion

        #region _userroles
        public static Task<string> GetUserRolesAsync(IDistributedCache _cache, string key)
        {
            return GetKeyAsync(_cache, _UserRoles, key);
        }
        public static string GetUserRoles(IDistributedCache _cache, string key)
        {
            return GetKey(_cache, _UserRoles, key);
        }


        public static Task SetUserRolesAsync(IDistributedCache _cache, string key, string value, DistributedCacheEntryOptions options = null)
        {
            return SetKeyAsync(_cache, _UserRoles, key, value, options);
        }
        public static void SetUserRoles(IDistributedCache _cache, string key, string value, DistributedCacheEntryOptions options = null)
        {
            SetKey(_cache, _UserRoles, key, value, options);
        }
        #endregion

        #region _ActionPath
        public static Task<string> GetUserActionPathAsync(IDistributedCache _cache, string key)
        {
            return GetKeyAsync(_cache, _ActionPath, key);
        }
        public static string GetUserActionPath(IDistributedCache _cache, string key)
        {
            return GetKey(_cache, _ActionPath, key);
        }


        public static Task SetUserActionPathAsync(IDistributedCache _cache, string key, string value, DistributedCacheEntryOptions options = null)
        {
            return SetKeyAsync(_cache, _ActionPath, key, value, options);
        }
        public static void SetUserActionPath(IDistributedCache _cache, string key, string value, DistributedCacheEntryOptions options = null)
        {
            SetKey(_cache, _ActionPath, key, value, options);
        }
        #endregion
    }
}
