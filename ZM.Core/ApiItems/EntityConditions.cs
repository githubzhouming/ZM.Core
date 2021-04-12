using System;
using System.Collections.Generic;
using System.Text;

namespace ZM.Core.ApiItems
{
    /// <summary>
    /// where 查询 条件
    /// </summary>
    public class EntityConditions
    {
        /// <summary>
        /// 字段名称
        /// </summary>
        public string Key { get; set; }
        /// <summary>
        /// 值
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// 操作类型
        /// </summary>
        public string Operator { get; set; }

        public List<EntityConditions> OrConditions { get; set; }

        public List<EntityConditions> AndConditions { get; set; }
    }
    /// <summary>
    /// order 排序条件
    /// </summary>
    public class EntityOrder
    {
        public string Key { get; set; }
        public string Order { get; set; }

        public const string Desc = "desc";
    }

    public class EntityPage
    {
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    public class AdvancedSearch
    {
        public EntityPage entityPage { get; set; }
        public string[] selectFileds { get; set; }
        public EntityConditions entityConditions { get; set; }

        public EntityOrder[] entityOrders { get; set; }
    }
}
