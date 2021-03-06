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
    /// 人员
    /// </summary>
    public partial class SysUser:EntityBase
    {
        [Column(name:"SysUserId")]
        public override Guid Id { get; set; }
        [Column(TypeName = "varchar(20)")]
        public string Name { get; set; }
        [Column(TypeName = "varchar(200)")]
        public string Password { get; set; }
        
    }
}
