using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZM.Core.DbContexts;
using ZM.Core.Entitys;

namespace ZM.Core.DBContexts
{
    public class EntityContext : DbContextBase
    {
        //public EntityContext(DbContextOptions<DbContext> options)
        //    : base(options)
        //{
        //}
        public EntityContext(DbContextOptions options)
            : base(options)
        {
        }

        //public EntityContext() : base()
        //{
        //}

        public virtual DbSet<SysUser> sysUsers { get; set; }
        public virtual DbSet<RequestLog> RequestLogs { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.HasAnnotation("ProductVersion", "2.2.6-servicing-10079");

            modelBuilder.Entity<SysUser>(entity =>
            {

            });

        }
    }
}
