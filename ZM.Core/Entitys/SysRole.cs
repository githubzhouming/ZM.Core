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
    /// 角色
    /// </summary>
    public partial class SysRole:EntityBase
    {
        [Column(name: "SysRoleId")]
        public override Guid Id { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string Name { get; set; }

        public int SysState { get; set; }
        
    }
}
