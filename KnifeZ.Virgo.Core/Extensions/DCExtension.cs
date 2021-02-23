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
using KnifeZ.Virgo.Core.Support.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using KnifeZ.Extensions;

namespace KnifeZ.Virgo.Core.Extensions
{
    /// <summary>
    /// DC相关扩展函数
    /// </summary>
    public static class DCExtension
    {
        #region 树形下拉
        /// <summary>
        /// 查询数据源，并转化成TreeSelectListItem列表
        /// </summary>
        /// <typeparam name="T">数据源类型</typeparam>
        /// <param name="baseQuery">基础查询</param>
        /// <param name="knifeVirgo">context</param>
        /// <param name="whereCondition">条件语句</param>
        /// <param name="textField">表达式用来获取Text字段对应的值</param>
        /// <param name="valueField">表达式用来获取Value字段对应的值，不指定则默认使用Id字段</param>
        /// <param name="iconField">表达式用来获取icon字段对应的值</param>
        /// <param name="urlField">表达式用来获取Url字段对应的值</param>
        /// <param name="tagField">表达式用来获取Tag字段对应的值</param>
        /// <param name="expandField">表达式用来获取Expanded字段对应的值，指示节点是否展开</param>
        /// <param name="ignorDataPrivilege">忽略数据权限判断</param>
        /// <param name="SortByName">是否根据Text字段排序，默认为是</param>
        /// <returns>SelectListItem列表</returns>
        public static List<TreeSelectListItem> GetTreeSelectListItems<T>(this IQueryable<T> baseQuery
            , VirgoContext knifeVirgo
            , Expression<Func<T, string>> textField
            , Expression<Func<T, string>> valueField = null
            , Expression<Func<T, string>> iconField = null
            , Expression<Func<T, string>> urlField = null
            , Expression<Func<T, string>> tagField = null
            , Expression<Func<T, bool>> expandField = null
            , bool ignorDataPrivilege = false
            , bool SortByName = true)
            where T : TreePoco
        {
            var dps = knifeVirgo?.LoginUserInfo?.DataPrivileges;
            var query = baseQuery.AsNoTracking();

            //如果没有指定忽略权限，则拼接权限过滤的where条件
            if (ignorDataPrivilege == false)
            {
                query = AppendSelfDPWhere(query, knifeVirgo, dps);
            }
            if (typeof(IPersistPoco).IsAssignableFrom(typeof(T)))
            {
                var mod = new IsValidModifier();
                var newExp = mod.Modify(query.Expression);
                query = query.Provider.CreateQuery<T>(newExp) as IOrderedQueryable<T>;
            }

            //处理后面要使用的expression
            //if (valueField == null)
            //{
            valueField = x => x.GetID().ToString().ToLower();
            //}
            Expression<Func<T, string>> parentField = x => x.ParentId.ToString().ToLower();

            //定义PE
            ParameterExpression pe = Expression.Parameter(typeof(T));
            ChangePara cp = new ChangePara();

            //创建新类，形成类似 new SimpleTreeTextAndValue() 的表达式
            NewExpression newItem = Expression.New(typeof(TreeSelectListItem));

            //绑定Text字段，形成类似 Text = textField 的表达式
            var textMI = typeof(TreeSelectListItem).GetMember("Text")[0];
            MemberBinding textBind = Expression.Bind(textMI, cp.Change(textField.Body, pe));

            //绑定Value字段，形成类似 Value = valueField 的表达式
            var valueMI = typeof(TreeSelectListItem).GetMember("Id")[0];
            MemberBinding valueBind = Expression.Bind(valueMI, cp.Change(valueField.Body, pe));

            //绑定ParentId字段，形成类似 Value = valueField 的表达式
            var parentMI = typeof(TreeSelectListItem).GetMember("ParentId")[0];
            MemberBinding parentBind = Expression.Bind(parentMI, cp.Change(parentField.Body, pe));

            //绑定Url字段，形成类似 Value = valueField 的表达式
            MemberBinding urlBind = null;
            var urlMI = typeof(TreeSelectListItem).GetMember("Url")[0];
            if (urlField != null)
            {
                urlBind = Expression.Bind(urlMI, cp.Change(urlField.Body, pe));
            }
            else
            {
                urlBind = Expression.Bind(urlMI, Expression.Constant(string.Empty));
            }

            //绑定icon字段，形成类似 ICon = iconField 的表达式
            MemberBinding iconBind = null;
            var iconMI = typeof(TreeSelectListItem).GetMember("ICon")[0];
            if (iconField != null)
            {
                iconBind = Expression.Bind(iconMI, cp.Change(iconField.Body, pe));
            }
            else
            {
                iconBind = Expression.Bind(iconMI, Expression.Constant(string.Empty));
            }

            //绑定Tag字段，形成类似 Value = valueField 的表达式
            MemberBinding tagBind = null;
            var tagMI = typeof(TreeSelectListItem).GetMember("Tag")[0];
            if (tagField != null)
            {
                tagBind = Expression.Bind(tagMI, cp.Change(tagField.Body, pe));
            }
            else
            {
                tagBind = Expression.Bind(tagMI, Expression.Constant(""));
            }

            //绑定Tag字段，形成类似 Value = valueField 的表达式
            MemberBinding expandBind = null;
            var expandMI = typeof(TreeSelectListItem).GetMember("Expended")[0];
            if (expandField != null)
            {
                expandBind = Expression.Bind(expandMI, cp.Change(expandField.Body, pe));
            }
            else
            {
                expandBind = Expression.Bind(expandMI, Expression.Constant(false));
            }

            //合并创建新类和绑定字段的表达式，形成类似 new SimpleTextAndValue{ Text = textField, Value = valueField} 的表达式
            MemberInitExpression init = Expression.MemberInit(newItem, textBind, valueBind, iconBind, parentBind, urlBind, tagBind, expandBind);

            //将最终形成的表达式转化为Lambda，形成类似 x=> new SimpleTextAndValue { Text = x.textField, Value = x.valueField} 的表达式
            var lambda = Expression.Lambda<Func<T, TreeSelectListItem>>(init, pe);

            List<TreeSelectListItem> rv = null;

            //根据Text对下拉菜单数据排序
            if (SortByName == true)
            {
                rv = query.Select(lambda).OrderBy(x => x.Text).ToList();
            }
            else
            {
                rv = query.Select(lambda).ToList();
            }

            rv.ForEach(x =>
            {
                x.Children = rv.Where(y => y.ParentId == x.Id).ToList();
            });
            return rv.Where(x => string.IsNullOrEmpty(x.ParentId)).ToList();
        }

        #endregion

        #region 下拉
        /// <summary>
        /// 查询数据源，并转化成SelectListItem列表
        /// </summary>
        /// <typeparam name="T">数据源类型</typeparam>
        /// <param name="baseQuery">基础查询</param>
        /// <param name="knifeVirgo">Virgo Context</param>
        /// <param name="textField">SelectListItem中Text字段对应的值</param>
        /// <param name="valueField">SelectListItem中Value字段对应的值，默认为Id列</param>
        /// <param name="ignorDataPrivilege">忽略数据权限判断</param>
        /// <param name="SortByName">是否根据Text字段排序，默认为是</param>
        /// <returns>SelectListItem列表</returns>
        public static List<ComboSelectListItem> GetSelectListItems<T>(this IQueryable<T> baseQuery
            , VirgoContext knifeVirgo
            , Expression<Func<T, string>> textField
            , Expression<Func<T, object>> valueField = null
            , bool ignorDataPrivilege = false
            , bool SortByName = true)
            where T : TopBasePoco
        {
            var dps = knifeVirgo?.LoginUserInfo?.DataPrivileges;
            var query = baseQuery.AsNoTracking();

            //如果value字段为空，则默认使用Id字段作为value值
            if (valueField == null)
            {
                valueField = x => x.GetID();
            }

            //如果没有指定忽略权限，则拼接权限过滤的where条件
            if (ignorDataPrivilege == false)
            {
                query = AppendSelfDPWhere(query, knifeVirgo, dps);
            }

            if (typeof(IPersistPoco).IsAssignableFrom(typeof(T)))
            {
                var mod = new IsValidModifier();
                var newExp = mod.Modify(query.Expression);
                query = query.Provider.CreateQuery<T>(newExp) as IOrderedQueryable<T>;
            }


            //定义PE
            ParameterExpression pe = Expression.Parameter(typeof(T));
            ChangePara cp = new ChangePara();
            //创建新类，形成类似 new SimpleTextAndValue() 的表达式
            NewExpression newItem = Expression.New(typeof(ComboSelectListItem));

            //绑定Text字段，形成类似 Text = textField 的表达式
            var textMI = typeof(ComboSelectListItem).GetMember("Text")[0];
            MemberBinding textBind = Expression.Bind(textMI, cp.Change(textField.Body, pe));


            //绑定Value字段，形成类似 Value = valueField 的表达式
            var valueMI = typeof(ComboSelectListItem).GetMember("Value")[0];
            MemberBinding valueBind = Expression.Bind(valueMI, cp.Change(valueField.Body, pe));

            //如果是树形结构，给ParentId赋值
            MemberBinding parentBind = null;
            var parentMI = typeof(ComboSelectListItem).GetMember("ParentId")[0];
            if (typeof(TreePoco<>).IsAssignableFrom(typeof(T)))
            {
                var parentMember = Expression.MakeMemberAccess(pe, typeof(TreePoco<>).GetSingleProperty("ParentId"));
                var p = Expression.Call(parentMember, "ToString", Array.Empty<Type>());
                //var p1 = Expression.Call(p, "ToLower", new Type[] { });
                parentBind = Expression.Bind(parentMI, p);
            }
            else
            {
                parentBind = Expression.Bind(parentMI, Expression.Constant(string.Empty));
            }

            //合并创建新类和绑定字段的表达式，形成类似 new SimpleTextAndValue{ Text = textField, Value = valueField} 的表达式
            MemberInitExpression init = Expression.MemberInit(newItem, textBind, valueBind, parentBind);

            //将最终形成的表达式转化为Lambda，形成类似 x=> new SimpleTextAndValue { Text = x.textField, Value = x.valueField} 的表达式
            var lambda = Expression.Lambda<Func<T, ComboSelectListItem>>(init, pe);


            IQueryable<ComboSelectListItem> rv = null;
            //根据Text对下拉菜单数据排序
            if (SortByName == true)
            {
                rv = query.Select(lambda).OrderBy(x => x.Text);
            }
            else
            {
                rv = query.Select(lambda);
            }

            return rv.ToList();
        }

        #endregion

        /// <summary>
        /// 拼接本表的数据权限过滤
        /// </summary>
        /// <typeparam name="T">数据类</typeparam>
        /// <param name="query">源query</param>
        /// <param name="knifeVirgo">Virgo context</param>
        /// <param name="dps">数据权限列表</param>
        /// <returns>拼接好where条件的query</returns>
        private static IQueryable<T> AppendSelfDPWhere<T>(IQueryable<T> query, VirgoContext knifeVirgo, List<SimpleDataPri> dps) where T : TopBasePoco
        {
            var dpsSetting = knifeVirgo?.DataPrivilegeSettings;
            ParameterExpression pe = Expression.Parameter(typeof(T));
            Expression peid = Expression.Property(pe, typeof(T).GetSingleProperty("ID"));
            //循环数据权限，加入到where条件中，达到自动过滤的效果
            if (dpsSetting?.Where(x => x.ModelName == query.ElementType.Name).SingleOrDefault() != null)
            {
                //如果dps参数是空，则生成 1!=1 这种错误的表达式，这样就查不到任何数据了
                if (dps == null)
                {
                    query = query.Where(Expression.Lambda<Func<T, bool>>(Expression.NotEqual(Expression.Constant(1), Expression.Constant(1)), pe));
                }
                else
                {
                    //在dps中找到和baseQuery源数据表名一样的关联id
                    var ids = dps.Where(x => x.TableName == query.ElementType.Name).Select(x => x.RelateId).ToList();
                    if (ids == null || ids.Count == 0)
                    {
                        query = query.Where(Expression.Lambda<Func<T, bool>>(Expression.NotEqual(Expression.Constant(1), Expression.Constant(1)), pe));
                    }
                    else
                    {
                        if (!ids.Contains(null))
                        {
                            query = query.Where(ids.GetContainIdExpression<T>());
                        }
                    }
                }
            }
            return query;
        }

        /// <summary>
        /// 为查询语句添加关联表的权限过滤
        /// </summary>
        /// <typeparam name="T">源数据类</typeparam>
        /// <param name="baseQuery">源Query</param>
        /// <param name="knifeVirgo"></param>
        /// <param name="IdFields">关联表外键</param>
        /// <returns>修改后的查询语句</returns>
        public static IQueryable<T> DPWhere<T>(this IQueryable<T> baseQuery, VirgoContext knifeVirgo, params Expression<Func<T, object>>[] IdFields) where T : TopBasePoco
        {
            var dps = knifeVirgo?.LoginUserInfo?.DataPrivileges;
            //循环所有关联外键
            List<string> tableNameList = new List<string>();
            foreach (var IdField in IdFields)
            {

                //将外键 Id 用.分割，循环生成指向最终id的表达式，比如x=> x.a.b.Id
                var fieldName = IdField.GetPropertyName(false);
                //获取关联的类
                string typename;
                //如果外键名称不是‘id’，则根据model层的命名规则，它应该是xxxId，所以抹掉最后的 Id 应该是关联的类名
                if (fieldName.ToLower() != "id")
                {
                    fieldName = fieldName.Remove(fieldName.Length - 2);
                    typename = IdField.GetPropertyInfo().DeclaringType.GetSingleProperty(fieldName).PropertyType.Name;
                }
                //如果是 Id，则本身就是关联的类
                else
                {
                    typename = typeof(T).Name;
                }
                tableNameList.Add(typename);

            }
            //var test = DPWhere(baseQuery, dps, tableNameList, IdFields);
            return DPWhere(baseQuery, knifeVirgo, tableNameList, IdFields);
        }

        #region AddBy YOUKAI 20160310
        /// <summary>
        /// 为查询语句添加关联表的权限过滤
        /// </summary>
        /// <typeparam name="T">源数据类</typeparam>
        /// <param name="baseQuery">源Query</param>
        /// <param name="knifeVirgo">context</param>
        /// <param name="tableName">关联数据权限的表名,如果关联外键为自身，则参数第一个为自身</param>
        /// <param name="IdFields">关联表外键</param>
        /// <returns>修改后的查询语句</returns>
        public static IQueryable<T> DPWhere<T>(this IQueryable<T> baseQuery, VirgoContext knifeVirgo, List<string> tableName, params Expression<Func<T, object>>[] IdFields) where T : TopBasePoco
        {
            var dps = knifeVirgo?.LoginUserInfo?.DataPrivileges;

            // var dpsSetting = BaseVM.AllDPS;
            ParameterExpression pe = Expression.Parameter(typeof(T));
            Expression left1 = Expression.Constant(1);
            Expression right1 = Expression.Constant(1);
            Expression trueExp = Expression.Equal(left1, right1);
            Expression falseExp = Expression.NotEqual(left1, right1);
            Expression finalExp = null;
            int tindex = 0;
            //循环所有关联外键
            foreach (var IdField in IdFields)
            {
                bool mtm = false;
                Expression exp = trueExp;
                //将外键Id用.分割，循环生成指向最终id的表达式，比如x=> x.a.b.Id
                var fullname = IdField.GetPropertyName();
                string[] splits = fullname.Split('.');
                int leftindex = splits[0].IndexOf('[');
                if (leftindex > 0)
                {
                    mtm = true;
                    splits[0] = splits[0].Substring(0, leftindex);
                }
                Expression peid = Expression.MakeMemberAccess(pe, pe.Type.GetSingleProperty(splits[0]));
                Type middletype = null;
                if (mtm)
                {
                    middletype = peid.Type.GetGenericArguments()[0];

                }
                else
                {
                    for (int i = 1; i < splits.Length; i++)
                    {
                        peid = Expression.MakeMemberAccess(peid, peid.Type.GetSingleProperty(splits[i]));
                    }
                    middletype = (peid as MemberExpression).Member.DeclaringType;
                }
                //如果dps为空，则拼接一个返回假的表达式，这样就查询不出任何数据
                if (dps == null)
                {
                    exp = falseExp;
                }
                else
                {
                    var fieldName = IdField.GetPropertyName(false);
                    //如果外键名称不是‘id’，则根据model层的命名规则，它应该是xxxId，所以抹掉最后的 Id 应该是关联的类名
                    if (fieldName.ToLower() != "id")
                    {
                        fieldName = fieldName.Remove(fieldName.Length - 2);
                        var typeinfo = middletype.GetSingleProperty(fieldName);
                        //var IsTableName = tableName?.Where(x => x == fieldName).FirstOrDefault();
                        var IsTableName = tableName?.Where(x => x.ToLower() == typeinfo.PropertyType.Name.ToLower()).FirstOrDefault();
                        if (string.IsNullOrEmpty(IsTableName))
                        {
                            continue;
                        }
                        fieldName = IsTableName;
                        //typename = PropertyHelper.GetPropertyInfo(IdField).DeclaringType.GetProperty(fieldName).PropertyType.Name;
                    }
                    //如果是Id，则本身就是关联的类
                    else
                    {
                        fieldName = tableName[tindex];
                    }
                    var dpsSetting = knifeVirgo.DataPrivilegeSettings;

                    //循环系统设定的数据权限，如果没有和关联类一样的表，则跳过
                    if (dpsSetting.Where(x => x.ModelName == fieldName).SingleOrDefault() == null)
                    {
                        continue;
                    }
                    //获取dps中关联到关联类的id列表
                    var ids = dps.Where(x => x.TableName == fieldName).Select(x => x.RelateId).ToList();
                    //如果没有关联的id，则拼接一个返回假的where，是语句查询不到任何数据
                    if (ids == null || ids.Count == 0)
                    {
                        exp = falseExp;
                        //if (peid.Type == typeof(Guid))
                        //{
                        //    exp = Expression.Equal(peid, Expression.Constant(Guid.NewGuid()));
                        //}
                        //else
                        //{
                        //    exp = Expression.Equal(peid, Expression.Constant(null));
                        //}
                    }
                    //如果有关联 Id
                    else
                    {
                        //如果关联 Id 不包含null，则生成类似 x=> ids.Contains(x.a.b.Id) 这种条件
                        //如果关联 Id 包括null，则代表可以访问所有数据，就不需要再拼接where条件了
                        if (!ids.Contains(null))
                        {
                            if (mtm == true)
                            {
                                ParameterExpression midpe = Expression.Parameter(middletype);
                                Expression middleid = Expression.PropertyOrField(midpe, IdField.GetPropertyName(false));

                                var queryable = Expression.Call(
                                     typeof(Queryable),
                                     "AsQueryable",
                                     new Type[] { middletype },
                                     peid);

                                List<Guid> ddd = new List<Guid>();

                                exp = Expression.Call(
                                     typeof(Queryable),
                                     "Any",
                                     new Type[] { middletype },
                                     queryable,
                                     Expression.Lambda(typeof(Func<,>).MakeGenericType(middletype, typeof(bool)), ids.GetContainIdExpression(middletype, midpe, middleid).Body, new ParameterExpression[] { midpe }));

                            }
                            else
                            {
                                exp = ids.GetContainIdExpression(typeof(T), pe, peid).Body;
                            }
                        }
                    }
                }
                //把所有where里的条件用And拼接在一起
                if (finalExp == null)
                {
                    finalExp = exp;
                }
                else
                {
                    finalExp = Expression.OrElse(finalExp, exp);
                }
                tindex++;
            }
            //如果没有进行任何修改，则还返回baseQuery
            if (finalExp == null)
            {
                return baseQuery;
            }
            else
            {
                //返回加入了where条件之后的baseQuery
                var query = baseQuery.Where(Expression.Lambda<Func<T, bool>>(finalExp, pe));
                return query;
            }
        }
        #endregion
    }
}
