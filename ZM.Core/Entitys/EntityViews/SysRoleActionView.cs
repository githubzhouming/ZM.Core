using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZM.Core.DbContexts;

namespace ZM.Core.Entitys
{
    
    public partial class SysRoleActionView : EntityViewBase
    {
        [Column(name: "SysRoleActionViewId")]
        public override Guid Id { get; set; }
        public Guid SysRoleId { get; set; }
        public Guid SysActionId { get; set; }
        public string ActionPath { get; set; }
        public string ActionName { get; set; }
        public string RoleName { get; set; }
    }
}
