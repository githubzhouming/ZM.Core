using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZM.Core.DbContexts;

namespace ZM.Core.Entitys
{
    
    public partial class SysRoleDataPermissionView : EntityViewBase
    {
        [Column(name: "SysRoleDataPermissionViewId")]
        public override Guid Id { get; set; }
        public Guid SysRoleId { get; set; }
        public Guid SysDataPermissionId { get; set; }
        public string EntityName { get; set; }
        public string DataPermissionWhereStr { get; set; }
        public string RoleName { get; set; }
    }
}
