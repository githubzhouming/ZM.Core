using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZM.Core.DbContexts;

namespace ZM.Core.Entitys
{
    /// <summary>
    /// 请求操作
    /// </summary>
    public partial class SysRoleAction:EntityBase
    {
        [Column(name: "SysRoleActionId")]
        public override Guid Id { get; set; }
        public Guid SysActionId { get; set; }
        public Guid SysRoleId { get; set; }
        
    }
}
