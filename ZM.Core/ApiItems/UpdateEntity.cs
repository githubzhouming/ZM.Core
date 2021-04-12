using System;
using System.Collections.Generic;
using System.Text;
using ZM.Core.DbContexts;

namespace ZM.Core.ApiItems
{
    public class UpdateEntity<TEntity> where TEntity :EntityBase
    {
        public TEntity entity { get; set; }

        public string[] properties { get; set; }
    }
}
