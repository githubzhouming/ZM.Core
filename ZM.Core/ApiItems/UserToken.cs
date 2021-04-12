using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ZM.Core.ApiItems
{
    public class UserToken
    {
        /// <summary>
        /// 用户系统id
        /// </summary>
        /// <value></value>
        public Guid userid { get; set; }
        /// <summary>
        /// 用户登录名
        /// </summary>
        /// <value></value>
        public string username { get; set; }
        /// <summary>
        /// timestamp 秒
        /// </summary>
        /// <value></value>
        public long? timestamp { get; set; }
        /// <summary>
        /// token的唯一值
        /// </summary>
        /// <value></value>
        public string tokenkey { get; set; }
        /// <summary>
        /// 签名
        /// </summary>
        /// <value></value>
        public string sign { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
        public static UserToken Parse(string userTokenStr)
        {
            return JsonConvert.DeserializeObject<UserToken>(userTokenStr);
        }
    }
}
