using System;
using System.Collections.Generic;
using System.Drawing;

namespace KnifeZ.Virgo.Core
{

    /// <summary>
    /// Grid Column Type Enum
    /// </summary>
    public enum GridColumnTypeEnum
    {
        /// <summary>
        /// 正常列
        /// </summary>
        Normal = 0,
        /// <summary>
        /// 空列
        /// </summary>
        Space,
        /// <summary>
        /// 操作列
        /// </summary>
        Action
    }

    /// <summary>
    /// IGridColumn
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IGridColumn<out T>
    {
        /// <summary>
        /// 表头类型
        /// </summary>
        GridColumnTypeEnum ColumnType { get; set; }

        /// <summary>
        /// 设定字段名
        /// </summary>
        string Field { get; set; }

        /// <summary>
        /// 标题名称
        /// </summary>
        string Title { get; set; }


        /// <summary>
        /// 隐藏列
        /// </summary>
        bool? Hide { get; set; }

        /// <summary>
        /// 子列
        /// </summary>
        IEnumerable<IGridColumn<T>> Children { get; }

        /// <summary>
        /// 底层子列数量
        /// </summary>
        int ChildrenLength { get; }
        List<ComboSelectListItem> ListItems { get; set; }

        #region 只读属性 生成 Excel 及其 表头用

        /// <summary>
        /// 最大子列数量
        /// </summary>
        int MaxChildrenCount { get; }

        /// <summary>
        /// 多表头的最大层数
        /// </summary>
        int MaxLevel { get; }

        /// <summary>
        /// 最下层列
        /// </summary>
        IEnumerable<IGridColumn<T>> BottomChildren { get; }

        /// <summary>
        /// 最大深度
        /// </summary>
        int MaxDepth { get; }

        #endregion


        #region 暂时没有用

        string Id { get; set; }

        /// <summary>
        /// 是否需要分组
        /// </summary>
        bool NeedGroup { get; set; }

        bool IsLocked { get; set; }

        bool Sortable { get; set; }
        /// <summary>
        /// 是否允许多行
        /// </summary>
        bool AllowMultiLine { get; set; }
        /// <summary>
        /// 是否填充
        /// </summary>
        int? Flex { get; set; }

        Type FieldType { get; }

        string FieldName { get; }

        /// <summary>
        /// 获取内容
        /// </summary>
        /// <param name="source">源数据</param>
        /// <param name="needFormat">是否适用format</param>
        /// <returns>内容</returns>
        object GetText(object source, bool needFormat = true);

        object GetObject(object source);
        /// <summary>
        /// 获取前景色
        /// </summary>
        /// <param name="source">源数据</param>
        /// <returns>前景色</returns>
        string GetForeGroundColor(object source);
        /// <summary>
        /// 获取背景色
        /// </summary>
        /// <param name="source">源数据</param>
        /// <returns>背景色</returns>
        string GetBackGroundColor(object source);
        bool HasFormat();
        #endregion
    }

}
