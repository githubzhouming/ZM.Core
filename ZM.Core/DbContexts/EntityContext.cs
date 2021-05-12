using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using ZM.Core.DbContexts;
using ZM.Core.Entitys;

namespace ZM.Core.DBContexts
{
    public class EntityContext : DbContextBase
    {
        public EntityContext(DbContextOptions<EntityContext> options)
            : base(options)
        {
        }
        //public EntityContext(DbContextOptions options)
        //    : base(options)
        //{
        //}

        //public EntityContext() : base()
        //{
        //}

        public virtual DbSet<RequestLog> RequestLogs { get; set; }
        public virtual DbSet<SysAction> SysActions { get; set; }
        public virtual DbSet<SysDataPermission> SysDataPermissions { get; set; }
        public virtual DbSet<SysRole> SysRoles { get; set; }
        public virtual DbSet<SysRoleAction> SysRoleActions { get; set; }
        public virtual DbSet<SysRoleDataPermission> SysRoleDataPermissions { get; set; }
        public virtual DbSet<SysUser> SysUsers { get; set; }
        public virtual DbSet<SysUserRole> SysUserRoles { get; set; }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.HasAnnotation("ProductVersion", "2.2.6-servicing-10079");

            modelBuilder.Entity<SysUser>(entity =>
            {

            });

            modelBuilder.Entity<SysRoleActionView>(entity =>
            {
                entity.ToView("SysRoleActionView");
            });
            modelBuilder.Entity<SysRoleDataPermissionView>(entity =>
            {
                entity.ToView("SysRoleDataPermissionView");
            });

            // define your filter expression tree
            Expression<Func<EntityBase, bool>> filterExpr = eb => !eb.IsDeleted;
            foreach (var mutableEntityType in modelBuilder.Model.GetEntityTypes())
            {
                // check if current entity type is child of BaseModel
                if (mutableEntityType.ClrType.IsAssignableTo(typeof(EntityBase)))
                {
                    // modify expression to handle correct child type
                    var parameter = Expression.Parameter(mutableEntityType.ClrType);
                    var body = ReplacingExpressionVisitor.Replace(filterExpr.Parameters.First(), parameter, filterExpr.Body);
                    var lambdaExpression = Expression.Lambda(body, parameter);

                    // set filter
                    mutableEntityType.SetQueryFilter(lambdaExpression);
                }
            }
        }
    }
}
