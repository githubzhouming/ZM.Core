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
    public partial class SysAction:EntityBase
    {
        [Column(name: "SysActionId")]
        public override Guid Id { get; set; }
        [Column(TypeName = "varchar(20)")]
        public string Name { get; set; }
        [Column(TypeName = "varchar(200)")]
        public string ActionPath { get; set; }
        
    }
}
