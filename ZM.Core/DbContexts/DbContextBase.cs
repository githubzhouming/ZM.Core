using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using ZM.Core.Extensions;
using ZM.Core.Options;

namespace ZM.Core.DbContexts
{
    public class DbContextBase: DbContext
    {
        //public DbContextBase()
        //{
        //    SavingChanges += PreSave;
        //}

        //public DbContextBase(DbContextOptions<DbContext> options)
        //    : base(options)
        //{
        //    SavingChanges += PreSave;
        //}
        public DbContextBase(DbContextOptions options)
            : base(options)
        {
            SavingChanges += PreSave;
        }
        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    if (!optionsBuilder.IsConfigured)
        //    {
        //        throw new Exception("数据库连接字符串未定义");
        //    }

        //}


        public virtual void  PreSave(object sender, SavingChangesEventArgs e)
        {
            var httpContext = HttpOptions.GetContext();
            var userToken= httpContext.getUserToken();
            var userId = userToken?.userid;
            var context = (sender as DbContextBase);
            if (context != null)
            {
                var entitys = context.ChangeTracker.Entries();
                foreach (var entry in entitys)
                {
                    var thisEntity = (entry.Entity as EntityBase);
                    if (thisEntity == null) { continue; }
                    if (entry.State == EntityState.Added)
                    {
                        thisEntity.Id = Guid.NewGuid();
                        thisEntity.CreatedOn =thisEntity.ModifiedOn= DateTime.Now;
                        thisEntity.CreatedBy = thisEntity.ModifiedBy = thisEntity.OwnerUserId = userId;
                    }
                    else if (entry.State == EntityState.Modified)
                    {
                        thisEntity.ModifiedOn = DateTime.Now;
                        thisEntity.ModifiedBy =  userId;
                    }
                }
            }
            
        }
    }
}
