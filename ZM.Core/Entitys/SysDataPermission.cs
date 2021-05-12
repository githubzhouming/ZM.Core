using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using ZM.Core.ApiItems;
using ZM.Core.DbContexts;

namespace ZM.Core.Entitys
{
    public partial class SysDataPermission : EntityBase
    {
        [Column(name: "SysDataPermissionId")]
        public override Guid Id { get; set; }

        [Column(TypeName ="varchar(100)")]
        public string EntityName { get; set; }

        public string DataPermissionWhereStr { get; set; }


        public EntityConditions GetEntityConditions()
        {
            if (string.IsNullOrEmpty(DataPermissionWhereStr)) { return null; }
            return JsonConvert.DeserializeObject<EntityConditions>(DataPermissionWhereStr);
        }

        public static EntityConditions ParesEntityConditions(string str)
        {
            if (string.IsNullOrEmpty(str)) { return null; }
            return JsonConvert.DeserializeObject<EntityConditions>(str);
        }

        public static string EntityConditionsToString(EntityConditions entityConditions)
        {
            return JsonConvert.SerializeObject(entityConditions);
        }
    }
}
