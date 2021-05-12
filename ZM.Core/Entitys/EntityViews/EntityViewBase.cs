using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ZM.Core.Entitys
{
    public abstract class EntityViewBase
    {
        public abstract Guid Id { get; set; }
        public virtual string GetTableName ()=> this.GetType().Name;
    }
}
