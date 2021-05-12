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
    public partial class SysRoleDataPermission : EntityBase
    {
        [Column(name: "SysRoleDataPermissionId")]
        public override Guid Id { get; set; }
        public Guid SysDataPermissionId { get; set; }
        public Guid SysRoleId { get; set; }
    }
}
