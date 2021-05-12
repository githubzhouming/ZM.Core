using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZM.Core.DbContexts;

namespace ZM.Core.Entitys
{
    public partial class SysUserRole : EntityBase
    {
        [Column(name: "SysUserRoleId")]
        public override Guid Id { get; set; }
        public Guid SysUserId { get; set; }
        public Guid SysRoleId { get; set; }
        
    }
}
