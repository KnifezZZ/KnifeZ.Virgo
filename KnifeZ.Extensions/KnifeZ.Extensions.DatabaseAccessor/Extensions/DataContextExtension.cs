using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using KnifeZ.Extensions.DatabaseAccessor;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;

namespace KnifeZ.Extensions
{
    /// <summary>
    /// DataContext相关扩展函数
    /// </summary>
    public static class DataContextExtension
    {
        /// <summary>
        /// 排序
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="baseQuery"></param>
        /// <param name="sortInfo"></param>
        /// <param name="defaultSorts"></param>
        /// <returns></returns>
        public static IOrderedQueryable<T> Sort<T>(this IQueryable<T> baseQuery, string sortInfo, params SortInfo[] defaultSorts) where T : TopBasePoco
        {
            List<SortInfo> info = new List<SortInfo>();
            IOrderedQueryable<T> rv = null;
            if (string.IsNullOrEmpty(sortInfo))
            {
                if (defaultSorts == null || defaultSorts.Length == 0)
                {
                    ParameterExpression pe = Expression.Parameter(typeof(T));
                    var idproperty = typeof(T).GetSingleProperty("ID");
                    Expression pro = Expression.Property(pe, idproperty);
                    Type proType = typeof(Guid);
                    Expression final = Expression.Call(
                                                   typeof(Queryable),
                                                   "OrderBy",
                                                   new Type[] { typeof(T), proType },
                                                   baseQuery.Expression,
                                                   Expression.Lambda(pro, new ParameterExpression[] { pe }));
                    rv = baseQuery.Provider.CreateQuery<T>(final) as IOrderedQueryable<T>;
                    return rv;
                }
                else
                {
                    info.AddRange(defaultSorts);
                }
            }
            else
            {
                var temp = JsonSerializer.Deserialize<List<SortInfo>>(sortInfo);
                info.AddRange(temp);
            }
            foreach (var item in info)
            {
                ParameterExpression pe = Expression.Parameter(typeof(T));
                var idproperty = typeof(T).GetSingleProperty(item.Property);
                Expression pro = Expression.Property(pe, idproperty);
                Type proType = typeof(T).GetSingleProperty(item.Property).PropertyType;
                if (item.Direction == SortDir.Asc)
                {
                    if (rv == null)
                    {
                        Expression final = Expression.Call(
                               typeof(Queryable),
                               "OrderBy",
                               new Type[] { typeof(T), proType },
                               baseQuery.Expression,
                               Expression.Lambda(pro, new ParameterExpression[] { pe }));
                        rv = baseQuery.Provider.CreateQuery<T>(final) as IOrderedQueryable<T>;
                    }
                    else
                    {
                        Expression final = Expression.Call(
                               typeof(Queryable),
                               "ThenBy",
                               new Type[] { typeof(T), proType },
                               rv.Expression,
                               Expression.Lambda(pro, new ParameterExpression[] { pe }));
                        rv = rv.Provider.CreateQuery<T>(final) as IOrderedQueryable<T>;
                    }
                }
                if (item.Direction == SortDir.Desc)
                {
                    if (rv == null)
                    {
                        Expression final = Expression.Call(
                               typeof(Queryable),
                               "OrderByDescending",
                               new Type[] { typeof(T), proType },
                               baseQuery.Expression,
                               Expression.Lambda(pro, new ParameterExpression[] { pe }));
                        rv = baseQuery.Provider.CreateQuery<T>(final) as IOrderedQueryable<T>;
                    }
                    else
                    {
                        Expression final = Expression.Call(
                               typeof(Queryable),
                               "ThenByDescending",
                               new Type[] { typeof(T), proType },
                               rv.Expression,
                               Expression.Lambda(pro, new ParameterExpression[] { pe }));
                        rv = rv.Provider.CreateQuery<T>(final) as IOrderedQueryable<T>;
                    }
                }
            }
            return rv;
        }
        /// <summary>
        /// 检查ID是否存在
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="baseQuery"></param>
        /// <param name="val"></param>
        /// <param name="member"></param>
        /// <returns></returns>
        public static IQueryable<T> CheckID<T>(this IQueryable<T> baseQuery, object val, MemberExpression member = null)
        {
            ParameterExpression pe = Expression.Parameter(typeof(T));
            PropertyInfo idproperty;
            if (member == null)
            {
                idproperty = typeof(T).GetSingleProperty("ID");
            }
            else
            {
                idproperty = typeof(T).GetSingleProperty(member.Member.Name);
            }
            Expression peid = Expression.Property(pe, idproperty);
            var convertid = PropertyHelper.ConvertValue(val, idproperty.PropertyType);
            return baseQuery.Where(Expression.Lambda<Func<T, bool>>(Expression.Equal(peid, Expression.Constant(convertid)), pe));
        }

        public static IQueryable<T> CheckIDs<T>(this IQueryable<T> baseQuery, List<string> val, MemberExpression member = null)
        {
            if (val == null)
            {
                return baseQuery;
            }
            ParameterExpression pe = Expression.Parameter(typeof(T));
            PropertyInfo idproperty = null;
            if (member == null)
            {
                idproperty = typeof(T).GetSingleProperty("ID");
            }
            else
            {
                idproperty = typeof(T).GetSingleProperty(member.Member.Name);
            }
            Expression peid = Expression.Property(pe, idproperty);
            var exp = val.GetContainIdExpression(typeof(T), pe, peid).Body;
            return baseQuery.Where(Expression.Lambda<Func<T, bool>>(exp, pe));
        }

        public static IQueryable<T> CheckNotNull<T>(this IQueryable<T> baseQuery, Expression<Func<T, object>> member)
        {
            return baseQuery.CheckNotNull<T>(member.GetPropertyName());
        }

        public static IQueryable<T> CheckNotNull<T>(this IQueryable<T> baseQuery, string member)
        {
            ParameterExpression pe = Expression.Parameter(typeof(T));
            PropertyInfo idproperty = typeof(T).GetSingleProperty(member);
            Expression peid = Expression.Property(pe, idproperty);
            return baseQuery.Where(Expression.Lambda<Func<T, bool>>(Expression.NotEqual(peid, Expression.Constant(null)), pe));
        }


        public static IQueryable<T> CheckNull<T>(this IQueryable<T> baseQuery, Expression<Func<T, object>> member)
        {
            ParameterExpression pe = Expression.Parameter(typeof(T));
            PropertyInfo idproperty = typeof(T).GetSingleProperty(member.GetPropertyName());
            Expression peid = Expression.Property(pe, idproperty);
            return baseQuery.Where(Expression.Lambda<Func<T, bool>>(Expression.Equal(peid, Expression.Constant(null)), pe));
        }


        public static IQueryable<T> CheckWhere<T, S>(this IQueryable<T> baseQuery, S val, Expression<Func<T, bool>> where)
        {
            if (val == null)
            {
                return baseQuery;
            }
            else if (val is string s && string.IsNullOrEmpty(s))
            {
                return baseQuery;
            }
            else
            {
                if (typeof(IList).IsAssignableFrom(val.GetType()))
                {
                    if (((IList)val).Count == 0)
                    {
                        return baseQuery;
                    }
                }
                return baseQuery.Where(where);
            }
        }

        public static IQueryable<T> CheckEqual<T>(this IQueryable<T> baseQuery, string val, Expression<Func<T, string>> field)
        {
            if (string.IsNullOrEmpty(val))
            {
                return baseQuery;
            }
            else
            {
                val = val.Trim();
                var equal = Expression.Equal(field.Body, Expression.Constant(val));
                var where = Expression.Lambda<Func<T, bool>>(equal, field.Parameters[0]);
                return baseQuery.Where(where);
            }
        }

        public static IQueryable<T> CheckEqual<T, S>(this IQueryable<T> baseQuery, S? val, Expression<Func<T, S?>> field)
            where S : struct
        {
            if (val == null)
            {
                return baseQuery;
            }
            else
            {
                var equal = Expression.Equal(Expression.PropertyOrField(field.Body, "Value"), Expression.Constant(val));
                var where = Expression.Lambda<Func<T, bool>>(equal, field.Parameters[0]);
                return baseQuery.Where(where);
            }
        }

        public static IQueryable<T> CheckEqual<T, S>(this IQueryable<T> baseQuery, S val, Expression<Func<T, S?>> field)
    where S : struct
        {
            S? a = val;
            return baseQuery.CheckEqual(a, field);
        }


        public static IQueryable<T> CheckBetween<T, S>(this IQueryable<T> baseQuery, S? valMin, S? valMax, Expression<Func<T, S?>> field, bool includeMin = true, bool includeMax = true)
    where S : struct
        {
            if (valMin == null && valMax == null)
            {
                return baseQuery;
            }
            else
            {
                IQueryable<T> rv = baseQuery;
                if (valMin != null)
                {
                    BinaryExpression exp1 = !includeMin ? Expression.GreaterThan(Expression.PropertyOrField(field.Body, "Value"), Expression.Constant(valMin)) : Expression.GreaterThanOrEqual(Expression.PropertyOrField(field.Body, "Value"), Expression.Constant(valMin));
                    rv = rv.Where(Expression.Lambda<Func<T, bool>>(exp1, field.Parameters[0]));
                }
                if (valMax != null)
                {
                    BinaryExpression exp2 = !includeMax ? Expression.LessThan(Expression.PropertyOrField(field.Body, "Value"), Expression.Constant(valMax)) : Expression.LessThanOrEqual(Expression.PropertyOrField(field.Body, "Value"), Expression.Constant(valMax));
                    rv = rv.Where(Expression.Lambda<Func<T, bool>>(exp2, field.Parameters[0]));
                }
                return rv;
            }
        }

        public static IQueryable<T> CheckBetween<T, S>(this IQueryable<T> baseQuery, S valMin, S valMax, Expression<Func<T, S?>> field, bool includeMin = true, bool includeMax = true)
where S : struct
        {
            S? a = valMin;
            S? b = valMax;
            return CheckBetween(baseQuery, a, b, field, includeMin, includeMax);
        }

        public static IQueryable<T> CheckBetween<T, S>(this IQueryable<T> baseQuery, S? valMin, S valMax, Expression<Func<T, S?>> field, bool includeMin = true, bool includeMax = true)
where S : struct
        {
            S? a = valMin;
            S? b = valMax;
            return CheckBetween(baseQuery, a, b, field, includeMin, includeMax);
        }

        public static IQueryable<T> CheckBetween<T, S>(this IQueryable<T> baseQuery, S valMin, S? valMax, Expression<Func<T, S?>> field, bool includeMin = true, bool includeMax = true)
where S : struct
        {
            S? a = valMin;
            S? b = valMax;
            return CheckBetween(baseQuery, a, b, field, includeMin, includeMax);
        }

        public static IQueryable<T> CheckContain<T>(this IQueryable<T> baseQuery, string val, Expression<Func<T, string>> field, bool ignoreCase = true)
        {
            if (string.IsNullOrEmpty(val))
            {
                return baseQuery;
            }
            else
            {
                val = val.Trim();
                Expression exp = null;
                if (ignoreCase == true)
                {
                    var tolower = Expression.Call(field.Body, "ToLower", null);
                    exp = Expression.Call(tolower, "Contains", null, Expression.Constant(val.ToLower()));
                }
                else
                {
                    exp = Expression.Call(field.Body, "Contains", null, Expression.Constant(val));

                }
                var where = Expression.Lambda<Func<T, bool>>(exp, field.Parameters[0]);
                return baseQuery.Where(where);
            }
        }

        public static IQueryable<T> CheckContain<T, S>(this IQueryable<T> baseQuery, List<S> val, Expression<Func<T, S>> field)
        {
            if (val == null || val.Count == 0 || (val.Count == 1 && val[0] == null))
            {
                return baseQuery;
            }
            else
            {
                Expression exp = Expression.Call(Expression.Constant(val), "Contains", null, field.Body);

                var where = Expression.Lambda<Func<T, bool>>(exp, field.Parameters[0]);
                return baseQuery.Where(where);
            }
        }

        public static IQueryable<string> DynamicSelect<T>(this IQueryable<T> baseQuery, string fieldName)
        {
            ParameterExpression pe = Expression.Parameter(typeof(T));
            var idproperty = typeof(T).GetSingleProperty(fieldName);
            Expression pro = Expression.Property(pe, idproperty);
            Expression tostring = Expression.Call(pro, "ToString", new Type[] { });
            Type proType = typeof(string);
            Expression final = Expression.Call(
                                           typeof(Queryable),
                                           "Select",
                                           new Type[] { typeof(T), proType },
                                           baseQuery.Expression,
                                           Expression.Lambda(tostring, new ParameterExpression[] { pe }));
            var rv = baseQuery.Provider.CreateQuery<string>(final) as IOrderedQueryable<string>;
            return rv;
        }



        public static string GetTableName<T>(this IDataContext self)
        {
            return self.Model.FindEntityType(typeof(T)).GetTableName();
        }

        /// <summary>
        /// 通过模型和模型的某个List属性的名称来判断List的字表中关联到主表的主键名称
        /// </summary>
        /// <typeparam name="T">主表Model</typeparam>
        /// <param name="self">DataContext</param>
        /// <param name="listFieldName">主表中的子表List属性名称</param>
        /// <returns>主键名称</returns>
        public static string GetFKName<T>(this IDataContext self, string listFieldName) where T : class
        {
            return GetFKName(self, typeof(T), listFieldName);
        }

        /// <summary>
        /// 通过模型和模型的某个List属性的名称来判断List的字表中关联到主表的主键名称
        /// </summary>
        /// <param name="self">DataContext</param>
        /// <param name="sourceType">主表model类型</param>
        /// <param name="listFieldName">主表中的子表List属性名称</param>
        /// <returns>主键名称</returns>
        public static string GetFKName(this IDataContext self, Type sourceType, string listFieldName)
        {
            try
            {
                var test = self.Model.FindEntityType(sourceType).GetReferencingForeignKeys().Where(x => x.PrincipalToDependent?.Name == listFieldName).FirstOrDefault();
                if (test != null && test.Properties.Count > 0)
                {
                    return test.Properties[0].Name;
                }
                else
                {
                    return "";
                }
            }
            catch
            {
                return "";
            }
        }


        /// <summary>
        /// 通过子表模型和模型关联到主表的属性名称来判断该属性对应的主键名称
        /// </summary>
        /// <typeparam name="T">子表Model</typeparam>
        /// <param name="self">DataContext</param>
        /// <param name="FieldName">关联主表的属性名称</param>
        /// <returns>主键名称</returns>
        public static string GetFKName2<T>(this IDataContext self, string FieldName) where T : class
        {
            return GetFKName2(self, typeof(T), FieldName);
        }

        /// <summary>
        /// 通过模型和模型关联到主表的属性名称来判断该属性对应的主键名称
        /// </summary>
        /// <param name="self">DataContext</param>
        /// <param name="sourceType">子表model类型</param>
        /// <param name="FieldName">关联主表的属性名称</param>
        /// <returns>主键名称</returns>
        public static string GetFKName2(this IDataContext self, Type sourceType, string FieldName)
        {
            try
            {
                var test = self.Model.FindEntityType(sourceType).GetForeignKeys().Where(x => x.DependentToPrincipal?.Name == FieldName).FirstOrDefault();
                if (test != null && test.Properties.Count > 0)
                {
                    return test.Properties[0].Name;
                }
                else
                {
                    return "";
                }
            }
            catch
            {
                return "";
            }
        }

        public static string GetFieldName<T>(this IDataContext self, Expression<Func<T, object>> field)
        {
            string pname = field.GetPropertyName();
            return self.GetFieldName<T>(pname);
        }


        public static string GetFieldName<T>(this IDataContext self, string fieldname)
        {
            var rv = self.Model.FindEntityType(typeof(T)).FindProperty(fieldname);
            return rv?.GetColumnName(new Microsoft.EntityFrameworkCore.Metadata.StoreObjectIdentifier());
        }

        public static string GetPropertyNameByFk(this IDataContext self, Type sourceType, string fkname)
        {
            try
            {
                var test = self.Model.FindEntityType(sourceType).GetForeignKeys().Where(x => x.DependentToPrincipal?.ForeignKey?.Properties[0]?.Name == fkname).FirstOrDefault();
                if (test != null && test.Properties.Count > 0)
                {
                    return test.DependentToPrincipal.Name;
                }
                else
                {
                    return "";
                }
            }
            catch
            {
                return "";
            }
        }


        public static Expression<Func<TModel, bool>> GetContainIdExpression<TModel>(this List<string> Ids, Expression peid = null)
        {
            ParameterExpression pe = Expression.Parameter(typeof(TModel));
            var rv = Ids.GetContainIdExpression(typeof(TModel), pe, peid) as Expression<Func<TModel, bool>>;
            return rv;
        }

        public static LambdaExpression GetContainIdExpression(this List<string> Ids, Type modeltype, ParameterExpression pe, Expression peid = null)
        {
            if (Ids == null)
            {
                Ids = new List<string>();
            }
            if (peid == null)
            {
                peid = Expression.Property(pe, modeltype.GetProperties().Where(x => x.Name.ToLower() == "id").FirstOrDefault());
            }
            else
            {
                ChangePara cp = new ChangePara();
                peid = cp.Change(peid, pe);
            }
            List<object> newids = new List<object>();
            foreach (var item in Ids)
            {
                newids.Add(PropertyHelper.ConvertValue(item, peid.Type));
            }
            Expression dpleft = Expression.Constant(newids, typeof(IEnumerable<object>));
            Expression dpleft2 = Expression.Call(typeof(Enumerable), "Cast", new Type[] { peid.Type }, dpleft);
            Expression dpleft3 = Expression.Call(typeof(Enumerable), "ToList", new Type[] { peid.Type }, dpleft2);
            Expression dpcondition = Expression.Call(typeof(Enumerable), "Contains", new Type[] { peid.Type }, dpleft3, peid);
            var rv = Expression.Lambda(typeof(Func<,>).MakeGenericType(modeltype, typeof(bool)), dpcondition, pe);
            return rv;
        }

        /// <summary>
        /// 开始一个事务，当使用同一IDataContext时，嵌套的两个事务不会引起冲突，当嵌套的事务执行时引起的异常会通过回滚方法向上层抛出异常
        /// </summary>
        /// <param name="self">DataContext</param>
        /// <returns>可用的事务实例</returns>
        public static Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction BeginTransaction(this IDataContext self)
        {
            if (self == null)
                throw new ArgumentNullException(nameof(self));
            if (self.Database == null)
                throw new ArgumentNullException(nameof(self.Database));
            if (@"Microsoft.EntityFrameworkCore.InMemory".Equals(self.Database.ProviderName, StringComparison.OrdinalIgnoreCase))
                return FakeNestedTransaction.DefaultTransaction;
            return self.Database.CurrentTransaction == null ? self.Database.BeginTransaction() : FakeNestedTransaction.DefaultTransaction;
        }

        /// <summary>
        /// 开始一个事务，当使用同一IDataContext时，嵌套的两个事务不会引起冲突，当嵌套的事务执行时引起的异常会通过回滚方法向上层抛出异常
        /// </summary>
        /// <param name="self">DataContext</param>
        /// <param name="isolationLevel"></param>
        /// <returns>可用的事务实例</returns>
        public static Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction BeginTransaction(this IDataContext self, System.Data.IsolationLevel isolationLevel)
        {
            if (self == null)
                throw new ArgumentNullException(nameof(self));
            if (self.Database == null)
                throw new ArgumentNullException(nameof(self.Database));
            if (@"Microsoft.EntityFrameworkCore.InMemory".Equals(self.Database.ProviderName, StringComparison.OrdinalIgnoreCase))
                return FakeNestedTransaction.DefaultTransaction;
            return self.Database.CurrentTransaction == null ? self.Database.BeginTransaction(isolationLevel) : FakeNestedTransaction.DefaultTransaction;
        }
    }

    public static class DbCommandExtension
    {
        public static void AddParameter(this DbCommand command)
        {

        }
    }

    internal class FakeNestedTransaction : Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction
    {
        internal static readonly FakeNestedTransaction DefaultTransaction = new FakeNestedTransaction();

        private FakeNestedTransaction() { }

        public void Dispose()
        {
        }

        public void Commit()
        {
        }

        public void Rollback()
        {
            throw new TransactionInDoubtException("an exception occurs while executing the nested transaction or processing the results");
        }

        public Task CommitAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task RollbackAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public ValueTask DisposeAsync()
        {
            throw new NotImplementedException();
        }

        public Guid TransactionId => Guid.Empty;
    }
}
