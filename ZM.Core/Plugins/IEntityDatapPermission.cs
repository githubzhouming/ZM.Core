using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ZM.Core.DbContexts;
using ZM.Core.Entitys;

namespace ZM.Core.Plugins
{
    public interface IEntityDatapPermission 
    {
        /// <summary>
        /// 检测是否通过，原因描述
        /// </summary>
        /// <param name="entityBase"></param>
        /// <param name="entityDatapPermissionEnum"></param>
        /// <returns></returns>
        public (bool isChecked,string msg) CheckDataPermission<TEntity>(TEntity entityBase, EntityDatapPermissionEnum entityDatapPermissionEnum) where TEntity : EntityBase;
        /// <summary>
        /// 检测是否通过，原因描述
        /// </summary>
        /// <param name="entityBase"></param>
        /// <returns></returns>
        public virtual (bool isChecked, string msg) CheckDataWrite<TEntity>(TEntity entityBase) where TEntity : EntityBase
        {
            return CheckDataPermission(entityBase, EntityDatapPermissionEnum.Write);
        }
        public virtual (bool isChecked, string msg) CheckDataWriteAdd<TEntity>(TEntity entityBase) where TEntity : EntityBase
        {
            return CheckDataPermission(entityBase, EntityDatapPermissionEnum.WriteAdd);
        }
        public virtual (bool isChecked, string msg) CheckDataWriteUpdate<TEntity>(TEntity entityBase) where TEntity : EntityBase
        {
            return CheckDataPermission(entityBase, EntityDatapPermissionEnum.WriteUpdate);
        }
        /// <summary>
        /// 检测是否通过，原因描述
        /// </summary>
        /// <param name="entityBase"></param>
        /// <returns></returns>
        public virtual (bool isChecked, string msg) CheckDataDelete<TEntity>(TEntity entityBase) where TEntity : EntityBase
        {
            return CheckDataPermission(entityBase, EntityDatapPermissionEnum.Delete);
        }


        /// <summary>
        /// 检测是否通过，原因描述
        /// </summary>
        /// <param name="entityBase"></param>
        /// <param name="entityDatapPermissionEnum"></param>
        /// <returns></returns>
        public (bool isChecked, string msg) CheckDataPermission<TEntity>(IEnumerable<TEntity> entityBases, EntityDatapPermissionEnum entityDatapPermissionEnum) where TEntity : EntityBase;
        /// <summary>
        /// 检测是否通过，原因描述
        /// </summary>
        /// <param name="entityBase"></param>
        /// <returns></returns>
        public virtual (bool isChecked, string msg) CheckDataWrite<TEntity>(IEnumerable<TEntity> entityBases) where TEntity : EntityBase
        {
            return CheckDataPermission(entityBases, EntityDatapPermissionEnum.Write);
        }
        public virtual (bool isChecked, string msg) CheckDataWriteAdd<TEntity>(IEnumerable<TEntity> entityBases) where TEntity : EntityBase
        {
            return CheckDataPermission(entityBases, EntityDatapPermissionEnum.WriteAdd);
        }
        public virtual (bool isChecked, string msg) CheckDataWriteUpdate<TEntity>(IEnumerable<TEntity> entityBases) where TEntity : EntityBase
        {
            return CheckDataPermission(entityBases, EntityDatapPermissionEnum.WriteUpdate);
        }
        /// <summary>
        /// 检测是否通过，原因描述
        /// </summary>
        /// <param name="entityBase"></param>
        /// <returns></returns>
        public virtual (bool isChecked, string msg) CheckDataDelete<TEntity>(IEnumerable<TEntity> entityBases) where TEntity : EntityBase
        {
            return CheckDataPermission(entityBases, EntityDatapPermissionEnum.Delete);
        }



        /// <summary>
        /// where条件信息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <returns></returns>
        public IQueryable<TEntity> PermissionWhere<TEntity>(IQueryable<TEntity> query) where TEntity : EntityBase;
    }
    public enum EntityDatapPermissionEnum
    {
        /// <summary>
        /// 读
        /// </summary>
        Read = 0b00000001,
        /// <summary>
        /// 新增写
        /// </summary>
        WriteAdd = 0b00000010,
        /// <summary>
        /// 修改写
        /// </summary>
        WriteUpdate = 0b00000100,
        /// <summary>
        /// 写
        /// </summary>
        Write = 0b00000110,

        /// <summary>
        /// 删除
        /// </summary>
        Delete = 0b00001000,
        /// <summary>
        /// 没有权限
        /// </summary>
        None = 0b00000000,
        /// <summary>
        /// 全部权限
        /// </summary>
        All= 0b11111111,
    }
}
