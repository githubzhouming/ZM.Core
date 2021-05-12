using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using ZM.Core.ApiItems;
using ZM.Core.Entitys;
using ZM.Core.Extensions;
using ZM.Core.Options;

namespace ZM.Core.Plugins
{
    public class EntityDatapPermissionDefault : IEntityDatapPermission
    {
        private readonly DbContext _context;

        public EntityDatapPermissionDefault(DbContext context)
        {
            _context = context;
        }

        private bool CheckDataDB<TEntity>(TEntity entityBase, List<SysRoleDataPermissionView> sysUserRoleActions) where TEntity : EntityBase
        {

            var queryDBSet = _context.Set<TEntity>();
            IQueryable<TEntity> queryWhere = null;
            var entityConditionsList = sysUserRoleActions.Select(p => SysDataPermission.ParesEntityConditions(p.DataPermissionWhereStr));
            queryWhere = queryDBSet.QueryConditions(entityConditionsList);
            //for (var i = 0; i < sysUserRoleActions.Count; i++)
            //{
            //    var entityConditions = SysDataPermission.ParesEntityConditions(sysUserRoleActions[i].DataPermissionWhereStr);
            //    if (i == 0) { queryWhere = queryDBSet.QueryConditions(entityConditions); }
            //    else { queryWhere = queryWhere.QueryConditions(entityConditions); }
            //}

            var result = queryWhere.Where(x => x.Id == entityBase.Id).ToList();
            return result.Any();

        }
        private bool CheckDataRuntime<TEntity>(TEntity entityBase, List<SysRoleDataPermissionView> sysUserRoleActions) where TEntity : EntityBase
        {
            List<TEntity> entities = new List<TEntity>();
            entities.Add(entityBase);
            IEnumerable<TEntity> result = entities;
            var entityConditionsList = sysUserRoleActions.Select(p => SysDataPermission.ParesEntityConditions(p.DataPermissionWhereStr));
            result= result.Where(EntityFrameworkEx.ParserConditions<TEntity>(entityConditionsList).Compile());
            //for (var i = 0; i < sysUserRoleActions.Count; i++)
            //{
            //    var entityConditions = SysDataPermission.ParesEntityConditions(sysUserRoleActions[i].DataPermissionWhereStr);
            //    result = result.Where(EntityFrameworkEx.ParserConditions<TEntity>(entityConditions).Compile());
            //}

            return result.Any();
        }
        public (bool isChecked, string msg) CheckDataPermission<TEntity>(TEntity entityBase, EntityDatapPermissionEnum entityDatapPermissionEnum) where TEntity : EntityBase
        {
            var userToken = HttpOptions.getUserToken();
            if (userToken.IsAdmin()) { return (true, string.Empty); }

            var EntityName = entityBase.GetType().Name;
            //var sysUserRoleActions = _context.Set<SysUserRoleDataPermissionView>().Where(x => x.SysUserId == userToken.userid && x.EntityName == EntityName).ToList();
            var sysUserRoleActions = getSysUserRoleDataPermissionViewData<TEntity>();
            if (!sysUserRoleActions.Any())
            {
                return (false, $"{EntityName} entity does not have data permissions");
            }
            switch (entityDatapPermissionEnum)
            {
                case EntityDatapPermissionEnum.Read:
                    if (!CheckDataDB(entityBase, sysUserRoleActions))
                    {
                        return (false, $"{EntityName} entity does not have data read permissions with ID equal to {entityBase.Id}");
                    }
                    break;
                case EntityDatapPermissionEnum.WriteAdd:
                    if (!CheckDataRuntime(entityBase, sysUserRoleActions))
                    {
                        return (false, $"{EntityName} entity does not have data read permissions with ID equal to {entityBase.Id}");
                    }
                    break;
                case EntityDatapPermissionEnum.WriteUpdate:
                    if (!CheckDataDB(entityBase, sysUserRoleActions))
                    {
                        return (false, $"{EntityName} entity does not have data read permissions with ID equal to {entityBase.Id}");
                    }
                    break;
                case EntityDatapPermissionEnum.Write:
                    return (false, "Write is not support");
                    //break;
                case EntityDatapPermissionEnum.Delete:
                    if (!CheckDataDB(entityBase, sysUserRoleActions))
                    {
                        return (false, $"{EntityName} entity does not have data read permissions with ID equal to {entityBase.Id}");
                    }
                    break;
                case EntityDatapPermissionEnum.None:
                    return (false, "None is not support");
                    //break;
                case EntityDatapPermissionEnum.All:
                    return (false, "All is not support");
                    //break;
                default:
                    return (false, "default is not support");
                    //break;
            }




            return (true, string.Empty);
        }


        private bool CheckDataDB<TEntity>(IEnumerable<TEntity> entityBases, List<SysRoleDataPermissionView> sysUserRoleActions) where TEntity : EntityBase
        {

            var queryDBSet = _context.Set<TEntity>();
            IQueryable<TEntity> queryWhere = null;
            var entityConditionsList = sysUserRoleActions.Select(p => SysDataPermission.ParesEntityConditions(p.DataPermissionWhereStr));
            queryWhere = queryDBSet.QueryConditions(entityConditionsList);
            //for (var i = 0; i < sysUserRoleActions.Count; i++)
            //{
            //    var entityConditions = SysDataPermission.ParesEntityConditions(sysUserRoleActions[i].DataPermissionWhereStr);
            //    if (i == 0) { queryWhere = queryDBSet.QueryConditions(entityConditions); }
            //    else { queryWhere = queryWhere.QueryConditions(entityConditions); }
            //}
            var inList = entityBases.Select(x => x.Id).ToList();
            var result = queryWhere.QueryIn("Id", inList)
                .ToList();

            return result.Any() && result.Count() == entityBases.Count();

        }
        private bool CheckDataRuntime<TEntity>(IEnumerable<TEntity> entityBases, List<SysRoleDataPermissionView> sysUserRoleActions) where TEntity : EntityBase
        {

            IEnumerable<TEntity> result = entityBases;
            var entityConditionsList = sysUserRoleActions.Select(p => SysDataPermission.ParesEntityConditions(p.DataPermissionWhereStr));
            result = result.Where(EntityFrameworkEx.ParserConditions<TEntity>(entityConditionsList).Compile());
            //for (var i = 0; i < sysUserRoleActions.Count; i++)
            //{
            //    var entityConditions = SysDataPermission.ParesEntityConditions(sysUserRoleActions[i].DataPermissionWhereStr);
            //    result = result.Where(EntityFrameworkEx.ParserConditions<TEntity>(entityConditions).Compile());
            //}
            return result.Any() && result.Count() == entityBases.Count();
        }
        public (bool isChecked, string msg) CheckDataPermission<TEntity>(IEnumerable<TEntity> entityBases, EntityDatapPermissionEnum entityDatapPermissionEnum) where TEntity : EntityBase
        {
            var userToken = HttpOptions.getUserToken();
            if (userToken.IsAdmin()) { return (true, string.Empty); }
            var EntityName = typeof(TEntity).Name;
            //var sysUserRoleActions = _context.Set<SysUserRoleDataPermissionView>().Where(x => x.SysUserId == userToken.userid && x.EntityName == EntityName).ToList();
            var sysUserRoleActions = getSysUserRoleDataPermissionViewData<TEntity>();
            if (!sysUserRoleActions.Any())
            {
                return (false, $"{EntityName} entity does not have data permissions");
            }
            switch (entityDatapPermissionEnum)
            {
                case EntityDatapPermissionEnum.Read:
                    if (!CheckDataDB(entityBases, sysUserRoleActions))
                    {
                        return (false, $"{EntityName} entity does not have data read permissions with ID equal to {string.Join(',', entityBases.Select(x => x.Id.ToString()))}");
                    }
                    break;
                case EntityDatapPermissionEnum.WriteAdd:
                    if (!CheckDataRuntime(entityBases, sysUserRoleActions))
                    {
                        return (false, $"{EntityName} entity does not have data read permissions with ID equal to {string.Join(',', entityBases.Select(x => x.Id.ToString()))}");
                    }
                    break;
                case EntityDatapPermissionEnum.WriteUpdate:
                    if (!CheckDataDB(entityBases, sysUserRoleActions))
                    {
                        return (false, $"{EntityName} entity does not have data read permissions with ID equal to {string.Join(',', entityBases.Select(x => x.Id.ToString()))}");
                    }
                    break;
                case EntityDatapPermissionEnum.Write:
                    return (false, "Write is not support");
                    //break;
                case EntityDatapPermissionEnum.Delete:
                    if (!CheckDataDB(entityBases, sysUserRoleActions))
                    {
                        return (false, $"{EntityName} entity does not have data read permissions with ID equal to {string.Join(',', entityBases.Select(x => x.Id.ToString()))}");
                    }
                    break;
                case EntityDatapPermissionEnum.None:
                    return (false, "None is not support");
                    //break;
                case EntityDatapPermissionEnum.All:
                    return (false, "All is not support");
                    //break;
                default:
                    return (false, "default is not support");
                    //break;
            }


            return (true, string.Empty);
        }



        public IQueryable<TEntity> PermissionWhere<TEntity>(IQueryable<TEntity> query) where TEntity : EntityBase
        {
            var userToken = HttpOptions.getUserToken();
            if (userToken.IsAdmin()) { return query; }
            var sysUserRoleActions = getSysUserRoleDataPermissionViewData<TEntity>();
            if (!sysUserRoleActions.Any())
            {
                return query.Where(x => false);
            }

            var entityConditionsList = sysUserRoleActions.Select(p => SysDataPermission.ParesEntityConditions(p.DataPermissionWhereStr));
            query= query.QueryConditions(entityConditionsList);
            //for (var i = 0; i < sysUserRoleActions.Count; i++)
            //{
            //    var entityConditions = SysDataPermission.ParesEntityConditions(sysUserRoleActions[i].DataPermissionWhereStr);
            //    query = query.QueryConditions(entityConditions);
            //}

            return query;
        }

        private List<SysRoleDataPermissionView> getSysUserRoleDataPermissionViewData<TEntity>() where TEntity : EntityBase
        {
            //var testData = new List<SysRoleDataPermissionView>();
            //testData.Add(new SysRoleDataPermissionView()
            //{
            //    DataPermissionWhereStr = SysDataPermission.EntityConditionsToString(new EntityConditions()
            //    {
            //        Key = "Id",
            //        Value = "885f7df9-48bb-44d4-8d45-0d9c316f5bee",
            //        Operator = "=",
            //        OrConditions = new List<EntityConditions>() { new EntityConditions()
            //        {
            //            Key = "Id",
            //            Value  = Guid.NewGuid().ToString(),
            //            Operator = "=",
            //        } }
            //    })
            //});
            //testData.Add(new SysRoleDataPermissionView()
            //{
            //    DataPermissionWhereStr = SysDataPermission.EntityConditionsToString(new EntityConditions()
            //    {
            //        Key = "Id",
            //        Value = "885f7df9-48bb-44d4-8d45-0d9c316f5bee",
            //        Operator = "=",
            //        OrConditions = new List<EntityConditions>() { new EntityConditions()
            //        {
            //            Key = "Id",
            //            Value  = Guid.NewGuid().ToString(),
            //            Operator = "!=",
            //        } }
            //    })
            //});
            //return testData;
            var EntityName = typeof(TEntity).Name;
            var userToken = HttpOptions.getUserToken();

            var roles= _context.Set<SysUserRole>().Where(x => x.SysUserId == userToken.userid).Select(p=>p.SysRoleId).ToList();
           
            var filter = EntityFrameworkEx.GetFilterExpression<SysRoleDataPermissionView, Guid>("Id", roles);

            return _context.Set<SysRoleDataPermissionView>().Where(x=>x.EntityName == EntityName).Where(filter).ToList();
        }

        //class TEntityEqualityComparer : IEqualityComparer<EntityBase>
        //{
        //    public bool Equals(EntityBase b1, EntityBase b2)
        //    {
        //        if (b1 == null&&b2==null) { return true; }

        //        return Guid.Equals(b1?.Id, b2?.Id);
        //    }
        //    public int GetHashCode(EntityBase bx)
        //    {
        //        if (bx == null)
        //        {
        //            return int.MinValue;
        //        }
        //        return bx.Id.GetHashCode();
        //    }
        //}

    }
}
