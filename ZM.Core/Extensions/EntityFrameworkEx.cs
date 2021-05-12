using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using ZM.Core.ApiItems;
using ZM.Core.DbContexts;
using ZM.Core.Entitys;
using ZM.Core.Utilities;

namespace ZM.Core.Extensions
{
    public static class EntityFrameworkEx
    {

        private const string _Contains = "like";
        private const string _Equal = "=";
        private const string _Greater = ">";
        private const string _GreaterEqual = ">=";
        private const string _Less = "<";
        private const string _LessEqual = "<=";
        private const string _NotEqual = "!=";
        private const string _In = "in";
        /// <summary>
        /// 全部查询
        /// </summary>
        private const string _All = "*";
        /// <summary>
        /// 全部不查询
        /// </summary>
        private const string _None = "!*";

        #region where
        public static IQueryable<T> QueryConditions<T>(this IQueryable<T> query, EntityConditions entityConditions) where T : class
        {
            var filter = ParserConditions<T>(entityConditions);
            return query.Where(filter);
        }

        public static Expression<Func<T, bool>> ParserConditions<T>(EntityConditions entityConditions) where T : class
        {
            var parameter = Expression.Parameter(typeof(T));
            //将条件转化成表达是的Body
            var queryExpression = ParseExpressionBody(parameter, entityConditions);

            return Expression.Lambda<Func<T, bool>>(queryExpression, parameter);
        }

        public static IQueryable<T> QueryConditions<T>(this IQueryable<T> query, IEnumerable<EntityConditions> entityConditionsList) where T : class
        {
            var filter = ParserConditions<T>(entityConditionsList);
            return query.Where(filter);
        }
        /// <summary>
        /// 多个条件用 OR 连接
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entityConditionsList"></param>
        /// <returns></returns>
        public static Expression<Func<T, bool>> ParserConditions<T>(IEnumerable< EntityConditions> entityConditionsList) where T : class
        {
            var expression = Expression.Equal(Expression.Constant(1), Expression.Constant(2)); ;

            var parameter = Expression.Parameter(typeof(T));
            foreach (var entityConditions in entityConditionsList)
            {
                //将条件转化成表达是的Body
                var queryExpression = ParseExpressionBody(parameter, entityConditions);
                expression=Expression.OrElse(expression, queryExpression);
            }
            return Expression.Lambda<Func<T, bool>>(expression, parameter);
        }
        private static Expression ParseExpressionBody(ParameterExpression parameter, EntityConditions condition)
        {

            Expression expression = ParseCondition(parameter, condition);//当前条件
            if (condition.AndConditions != null && condition.AndConditions.Count > 0)
            {
                foreach (var andCondition in condition.AndConditions)
                {
                    var andExpression = ParseExpressionBody(parameter, andCondition);
                    expression = Expression.AndAlso(expression, andExpression);
                }
            }

            if (condition.OrConditions != null && condition.OrConditions.Count > 0)
            {
                foreach (var orCondition in condition.OrConditions)
                {
                    var orExpression = ParseExpressionBody(parameter, orCondition);
                    expression = Expression.OrElse(expression, orExpression);
                }
            }

            return expression;
        }


        public static IQueryable<T> QueryIn<T,K>(this IQueryable<T> query, string Key,List<K> Value) where T : class
        {
            Expression<Func<T, bool>> filter = GetFilterExpression<T, K>(Key, Value);
            return query.Where(filter);
        }

        public static Expression<Func<T, bool>> GetFilterExpression<T, K>(string Key, List<K> Value) where T : class
        {
            var parameter = Expression.Parameter(typeof(T));
            var filterExpression = ParseCondition(parameter, Key, _In, Value);
            var filter = Expression.Lambda<Func<T, bool>>(filterExpression, parameter);
            return filter;
        }

        private static System.Reflection.MethodInfo _Like = null;
        private static Expression ParseCondition(ParameterExpression parameter, EntityConditions condition)
        {
            return ParseCondition(parameter, condition.Key, condition.Operator, condition.Value);
            //ParameterExpression p = parameter;
            //Expression key = Expression.Property(p, condition.Key);
            //switch (condition.Operator)
            //{
            //    case _Contains:
            //        if (_Like == null)
            //        {
            //            _Like = typeof(Microsoft.EntityFrameworkCore.DbFunctionsExtensions)
            //                               .GetMethod("Like", new Type[] {
            //            typeof(Microsoft.EntityFrameworkCore.DbFunctions)
            //            ,typeof(string)
            //            ,typeof(string) });
            //        }

            //        //like只支持 string
            //        return Expression.Call(_Like, Expression.Constant(EF.Functions), key, Expression.Constant(condition.Value));
            //    //return Expression.Call(key, typeof(string).GetMethod("Contains", new Type[] { typeof(string) }), value);
            //    case _Equal:
            //        return Expression.Equal(key, Expression.Convert(Expression.Constant(condition.Value.Parse(key.Type)), key.Type));
            //    case _Greater:
            //        return Expression.GreaterThan(key, Expression.Convert(Expression.Constant(condition.Value.Parse(key.Type)), key.Type));
            //    case _GreaterEqual:
            //        return Expression.GreaterThanOrEqual(key, Expression.Convert(Expression.Constant(condition.Value.Parse(key.Type)), key.Type));
            //    case _Less:
            //        return Expression.LessThan(key, Expression.Convert(Expression.Constant(condition.Value.Parse(key.Type)), key.Type));
            //    case _LessEqual:
            //        return Expression.LessThanOrEqual(key, Expression.Convert(Expression.Constant(condition.Value.Parse(key.Type)), key.Type));
            //    case _NotEqual:
            //        return Expression.NotEqual(key, Expression.Convert(Expression.Constant(condition.Value.Parse(key.Type)), key.Type));
            //    case _In:
            //        {
            //            if (condition.Value == null || string.IsNullOrEmpty(condition.Value.ToString()))
            //            {
            //                return Expression.Constant(false);
            //            }
            //            var valueArr = condition.Value.ToString().Split(',');
            //            Expression expression = Expression.Equal(Expression.Constant(1), Expression.Constant(2));
            //            foreach (var itemVal in valueArr)
            //            {
            //                // object ob=null;
            //                // if (key.Type == typeof(int)) {int convertValue = int.Parse(itemVal);ob=convertValue; }
            //                // else if (key.Type == typeof(int?)) {int? convertValue = int.Parse(itemVal); ob=convertValue;}
            //                // else if (key.Type == typeof(Guid)) {Guid convertValue = Guid.Parse(itemVal);ob=convertValue; }
            //                // else if (key.Type == typeof(Guid?)) {Guid? convertValue = Guid.Parse(itemVal); ob=convertValue;}
            //                // else if (key.Type == typeof(DateTime)) {DateTime convertValue = DateTime.Parse(itemVal); ob=convertValue;}
            //                // else if (key.Type == typeof(DateTime?)) {DateTime? convertValue = DateTime.Parse(itemVal); ob=convertValue;}
            //                // Expression _value = Expression.Constant(ob);
            //                Expression _value = Expression.Constant(itemVal.Parse(key.Type));
            //                Expression _right = Expression.Equal(key, Expression.Convert(_value, key.Type));

            //                expression = Expression.OrElse(expression, _right);
            //            }
            //            return expression;
            //        }
            //    default:
            //        throw new NotImplementedException("不支持此操作");
            //}
        }

        private static Expression ParseCondition([NotNull] ParameterExpression parameter, [NotNull] string Key, [NotNull] string Operator, [NotNull] object Value)
        {
            //var parameter = Expression.Parameter(typeof(T));
            if (Operator == _All)
            {
                return Expression.Constant(true);
            }
            if (Operator == _None)
            {
                return Expression.Constant(false);
            }
            if (string.IsNullOrEmpty(Value.ToString()))
            {
                return Expression.Constant(false);
            }

            var valueType = Value.GetType();
            Expression left = Expression.Property(parameter, Key);
            Expression rigth = null;
            if (Operator == _In)
            {
                List<Expression> rightIn = new List<Expression>();
                if (valueType == typeof(string))
                {
                    var valueArr = Value.ToString().Split(',');
                    foreach (var itemStr in valueArr)
                    {
                        if (left.Type == typeof(string))
                        {
                            rightIn.Add(Expression.Constant(itemStr));
                        }
                        else
                        {
                            rightIn.Add(Expression.Convert(Expression.Constant(itemStr.Parse(left.Type)), left.Type));
                        }
                    }
                    return ParseCondition(left, Operator, null, rightIn);


                }
                else
                {
                    ///List类型
                    if (valueType.IsGenericType)
                    {
                        var type = Value.GetType().GetGenericTypeDefinition();
                        if (type.HasImplementedRawGeneric(typeof(IList<>)))
                        {
                            var objList = (IList)Value;
                            for (int i = 0; i < objList.Count; i++)
                            {
                                rightIn.Add(Expression.Constant(objList[i]));
                            }
                            return ParseCondition(left, Operator, null, rightIn);
                        }
                    }
                    //Arrary 类型
                    else if (valueType.IsArray)
                    {
                        var objList = (object[])Value;
                        for (int i = 0; i < objList.Length; i++)
                        {
                            rightIn.Add(Expression.Constant(objList[i]));
                        }
                        return ParseCondition(left, Operator, null, rightIn);
                    }

                }
            }
            else
            {
                if ((Value == null) || left.Type.IsInstanceOfType(Value))
                {//相同类型不用转换
                    rigth = Expression.Constant(Value);
                }
                else
                {
                    rigth = Expression.Convert(Expression.Constant(Value.ToString().Parse(left.Type)), left.Type);
                }
                return ParseCondition(left, Operator, rigth, null);
            }

            return Expression.Constant(false);
        }

        public static Expression ParseCondition<T>([NotNull] string Key, [NotNull] string Operator, [NotNull] string Value)
        {
            var parameter = Expression.Parameter(typeof(T));
            return ParseCondition(parameter, Key, Operator, Value);
        }
        public static Expression ParseCondition<T>([NotNull] string Key, [NotNull] string Operator, [NotNull] object Value)
        {
            var parameter = Expression.Parameter(typeof(T));
            return ParseCondition(parameter, Key, Operator, Value);
        }

        private static Expression ParseCondition(Expression left, string Operator, Expression right, IEnumerable<Expression> rightIn)
        {
            switch (Operator)
            {
                case _Contains:
                    if (_Like == null)
                    {
                        _Like = typeof(Microsoft.EntityFrameworkCore.DbFunctionsExtensions)
                                           .GetMethod("Like", new Type[] {
                        typeof(Microsoft.EntityFrameworkCore.DbFunctions)
                        ,typeof(string)
                        ,typeof(string) });
                    }

                    //like只支持 string
                    return Expression.Call(_Like, Expression.Constant(EF.Functions), left, right);
                //return Expression.Call(key, typeof(string).GetMethod("Contains", new Type[] { typeof(string) }), value);
                case _Equal:
                    return Expression.Equal(left, right);
                case _Greater:
                    return Expression.GreaterThan(left, right);
                case _GreaterEqual:
                    return Expression.GreaterThanOrEqual(left, right);
                case _Less:
                    return Expression.LessThan(left, right);
                case _LessEqual:
                    return Expression.LessThanOrEqual(left, right);
                case _NotEqual:
                    return Expression.NotEqual(left, right);
                case _In:
                    {
                        if (rightIn == null || !rightIn.Any())
                        {
                            return Expression.Constant(false);
                        }
                        Expression expression = Expression.Equal(Expression.Constant(1), Expression.Constant(2));
                        foreach (var itemRight in rightIn)
                        {
                            var _rightExpression = Expression.Equal(left, itemRight);
                            expression = Expression.OrElse(expression, _rightExpression);
                        }
                        return expression;
                    }
                default:
                    throw new NotImplementedException("不支持此操作");
            }
        }
        #endregion
        #region order
        public static IQueryable<T> OrderConditions<T>(this IQueryable<T> query, IEnumerable<EntityOrder> entityOrders) where T : class
        {
            var t = typeof(T);
            var propertyInfos = t.GetProperties();
            foreach (var orderinfo in entityOrders)
            {
                var propertyInfo = propertyInfos.Where(x => x.Name.Equals(orderinfo.Key, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
                var parameter = Expression.Parameter(t);
                Expression propertySelector = Expression.Property(parameter, propertyInfo);

                var orderby = Expression.Lambda<Func<T, object>>(propertySelector, parameter);
                if (EntityOrder.Desc.Equals(orderinfo.Order, StringComparison.OrdinalIgnoreCase))
                {
                    query = query.OrderByDescending(orderby);
                }
                else
                {
                    query = query.OrderBy(orderby);
                }


            }
            return query;
        }

        #endregion
        #region page
        public static IQueryable<T> Pager<T>(this IQueryable<T> query, EntityPage entityPage)
        {
            return query.Skip((entityPage.PageIndex - 1) * entityPage.PageSize).Take(entityPage.PageSize);
        }
        #endregion
        #region select
        public static Expression<Func<TSource, TResult>> SelectBuild<TSource, TResult>(IEnumerable<string> properties)
        {
            Type sourceType = typeof(TSource);
            Dictionary<string, PropertyInfo> sourceProperties = sourceType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(prop =>
            {
                if (!prop.CanRead) { return false; }
                return properties.Contains(prop.Name, new MemberNameEqualityComparer());
            }).ToDictionary(p => p.Name, p => p);

            Type dynamicType = LinqRuntimeTypeBuilder.GetDynamicType(sourceProperties.Values);

            ParameterExpression sourceItem = Expression.Parameter(sourceType, "t");
            IEnumerable<MemberBinding> bindings = dynamicType.GetRuntimeProperties().Select(p => Expression.Bind(p, Expression.Property(sourceItem, sourceProperties[p.Name]))).OfType<MemberBinding>();

            return Expression.Lambda<Func<TSource, TResult>>(Expression.MemberInit(
                Expression.New(dynamicType.GetConstructor(Type.EmptyTypes)), bindings), sourceItem);

            // Expression<Func<TSource, TResult>> selector = null;
            // var tSourceType = typeof(TSource);
            // var left=Expression.Parameter(tSourceType,"x");

            // System.Linq.Expressions.NewExpression newExpression = System.Linq.Expressions.Expression.New(tSourceType);

            // var memberInfos = tSourceType.GetMembers(BindingFlags.Public | BindingFlags.Instance);
            // var memberInfoFilters = memberInfos.Where(x => properties.Contains(x.Name, new MemberNameEqualityComparer()));
            // var MemberBindings = memberInfoFilters.Select(x => 
            // System.Linq.Expressions.Expression.Bind(
            //          x,
            //          Expression.Property(left,x.Name)));

            // System.Linq.Expressions.MemberInitExpression memberInitExpression =
            //     System.Linq.Expressions.Expression.MemberInit(newExpression,MemberBindings);
            // selector= Expression.Lambda<Func<TSource, TResult>>(memberInitExpression,new ParameterExpression[]{left});
            // return selector;
        }
        /// <summary>
        /// 不区分大小写
        /// </summary>
        class MemberNameEqualityComparer : IEqualityComparer<string>
        {
            public bool Equals(string b1, string b2)
            {
                return string.Equals(b1, b2, StringComparison.InvariantCultureIgnoreCase);
            }
            public int GetHashCode(string bx)
            {
                return bx.GetHashCode();
            }
        }
        public static IQueryable<TResult> SelectByProperties<TSource, TResult>(this IQueryable<TSource> source, IEnumerable<string> properties)
        {
            var selector = SelectBuild<TSource, TResult>(properties);
            return source.Select(selector);
        }

        #endregion

        /// <summary>
        /// 高级查找
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbContext"></param>
        /// <param name="advancedSearch"></param>
        /// <returns></returns>
        public static IQueryable<dynamic> AdSearchEntity<T>(this DbContext dbContext, AdvancedSearch advancedSearch) where T : class
        {
            var queryDBSet = dbContext.Set<T>();
            return queryDBSet.AdSearchEntity(advancedSearch);
        }

        public static IQueryable<dynamic> AdSearchEntity<T>(this IQueryable<T> queryDBSet, AdvancedSearch advancedSearch) where T : class
        {
            var conditions = advancedSearch?.entityConditions;
            var selectFileds = advancedSearch?.selectFileds;
            var entityPage = advancedSearch?.entityPage;
            var entityOrders = advancedSearch?.entityOrders;

            IQueryable<T> queryWhere = null;
            IQueryable<T> queryOrder = null;
            IQueryable<T> queryPage = null;
            IQueryable<dynamic> querySelect = null;

            //where
            if (conditions == null)
            {
                queryWhere = queryDBSet.Where(x => 1 == 1);
            }
            else
            {
                queryWhere = queryDBSet.QueryConditions<T>(conditions);
            }
            //order
            if (entityOrders == null || entityOrders.Length == 0)
            {

                var propertyInfos= typeof(T).GetProperties();
                var pId = propertyInfos.Where(p => p.Name == "Id");
                if (pId.Any())
                {
                    queryOrder = queryWhere.OrderConditions(pId.Select(p=> new EntityOrder() {Key= p.Name,Order="asc" }));
                }
                else
                {
                    queryOrder = queryWhere.OrderConditions(propertyInfos.Take(1).Select(p => new EntityOrder() { Key = p.Name, Order = "asc" }));
                }
                
            }
            else
            {
                queryOrder = queryWhere.OrderConditions(entityOrders);
            }
            //page
            if (entityPage == null)
            {
                queryPage = queryOrder.Pager(new EntityPage());
            }
            else
            {
                queryPage = queryOrder.Pager(entityPage);
            }

            if (selectFileds == null || selectFileds.Length == 0)
            {
                querySelect = queryPage.Select<T, dynamic>(x => x);
            }
            else
            {
                querySelect = queryPage.SelectByProperties<T, dynamic>(selectFileds);
            }

            return querySelect;
        }
    }
}
