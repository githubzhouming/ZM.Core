using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using ZM.Core.DbContexts;

namespace ZM.Core.Entitys
{
    /// <summary>
    /// 请求日志
    /// </summary>
    public partial class RequestLog:EntityBase
    {
        [Column("RequestLogId")]
        public override Guid Id { get; set; }
        [Column(TypeName = "varchar(20)")]
        public string IP { get; set; }
        [Column(TypeName = "varchar(4000)")]
        public string Path { get; set; }
        [Column(TypeName = "varchar(10)")]
        public string Method { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string RequestKey { get; set; }
    }
}
