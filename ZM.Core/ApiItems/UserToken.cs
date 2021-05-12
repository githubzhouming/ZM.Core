using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZM.Core.ApiItems
{
    public class UserToken
    {
        private readonly Guid adminRole = new Guid("00000000-0000-0000-0000-000000000001");

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

        public List<Guid> roles { get; set; }

        public bool IsAdmin ()=> roles.Where(x => x == adminRole).Any();

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
