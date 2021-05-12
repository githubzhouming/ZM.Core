using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ZM.Core.ApiItems
{
    public class EntityInfo
    {
        public string Name { get; set; }
        public IEnumerable<EntitypPropertyInfo> PropertyInfos = new List<EntitypPropertyInfo>();

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
        public static EntityInfo Parse(string userTokenStr)
        {
            return JsonConvert.DeserializeObject<EntityInfo>(userTokenStr);
        }
    }
    public class EntitypPropertyInfo
    {
        public string Name { get; set; }
        public string TypeName { get; set; }
    }
}
