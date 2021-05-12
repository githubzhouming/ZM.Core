using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ZM.Core.Entitys
{
    public abstract class EntityBase
    {
        
        public abstract Guid Id { get; set; }

        public virtual string GetTableName ()=> this.GetType().Name;

        public DateTime? CreatedOn { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public Guid? CreatedBy { get; set; }
        public Guid? ModifiedBy { get; set; }
        public Guid? OwnerUserId { get; set; }

        public bool IsDeleted { get; set; } = false;
    }
}
