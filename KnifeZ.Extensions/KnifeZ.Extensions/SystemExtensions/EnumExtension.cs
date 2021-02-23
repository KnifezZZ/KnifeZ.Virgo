using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

namespace KnifeZ.Extensions
{

    /// <summary>
    /// 枚举扩展函数
    /// </summary>
    public static class EnumExtension
    {
        #region 将枚举类型转化为下拉列表
        /// <summary>
        /// 将枚举类型转化为下拉列表
        /// </summary>
        /// <param name="self">枚举类型</param>
        /// <param name="value">value</param>
        /// <param name="pleaseSelect">pleaseSelect</param>
        /// <returns>下拉菜单数据列表</returns>
        public static List<ComboSelectListItem> ToListItems (this Type self, object value = null)
        {
            string[] names = null;
            Array values = null;
            //如果是枚举
            if (self.IsEnum)
            {
                names = Enum.GetNames(self);
                values = Enum.GetValues(self);
            }
            //如果是nullable的枚举
            if (self.IsGenericType && self.GenericTypeArguments[0].IsEnum)
            {
                names = Enum.GetNames(self.GenericTypeArguments[0]);
                values = Enum.GetValues(self.GenericTypeArguments[0]);
            }
            //生成下拉菜单数据
            List<ComboSelectListItem> rv = new List<ComboSelectListItem>();
            if (names != null)
            {
                for (int i = 0; i < names.Length; i++)
                {
                    var name = names[i];
                    var newitem = new ComboSelectListItem { Text = PropertyHelper.GetEnumDisplayName(self, name), Value = ((int)values.GetValue(i)).ToString() };
                    if (value is string)
                    {
                        if (value != null)
                        {
                            string v = value.ToString();
                            var vs = v.Split(',');
                            foreach (var item in vs)
                            {
                                if (item != null && (newitem.Value.ToString() == item.ToString()) || name == item.ToString())
                                {
                                    newitem.Selected = true;
                                    break;
                                }

                            }
                        }
                    }
                    else if (value is IEnumerable it)
                    {
                        foreach (var item in it)
                        {
                            if (item != null && name == item.ToString())
                            {
                                newitem.Selected = true;
                                break;
                            }

                        }
                    }
                    else
                    {
                        if (value != null && name == value.ToString())
                        {
                            newitem.Selected = true;
                        }
                    }
                    rv.Add(newitem);
                }
            }
            return rv;
        }
        #endregion


        /// <summary>
        /// 获得枚举的Description(反射)
        /// </summary>
        /// <param name="value">枚举值</param>
        /// <param name="nameInstead">当枚举值没有定义DescriptionAttribute，是否使用枚举名代替，默认是使用</param>
        /// <returns>枚举的Description</returns>
        public static string GetDescription(this Enum value, Boolean nameInstead = true)
        {
            Type type = value.GetType();
            string name = Enum.GetName(type, value);
            if (name == null)
            {
                return null;
            }

            FieldInfo field = type.GetField(name);
            DescriptionAttribute attribute = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as DescriptionAttribute;

            if (attribute == null && nameInstead == true)
            {
                return name;
            }
            return attribute?.Description;
        }

        /// <summary>
        /// 把枚举转换为键值对集合
        /// </summary>
        /// <param name="enumType">枚举类型</param>
        /// <returns>以枚举值为key，枚举文本为value的键值对集合</returns>
        public static Dictionary<int, string> EnumToDictionaryKeysValue(Type enumType)
        {
            if (!enumType.IsEnum)
            {
                throw new ArgumentException("传入的参数必须是枚举类型！", "enumType");
            }
            Dictionary<int, String> enumDic = new Dictionary<int, string>();
            Array enumValues = Enum.GetValues(enumType);
            foreach (Enum enumValue in enumValues)
            {
                Int32 key = Convert.ToInt32(enumValue);
                String value = enumValue.ToString();
                enumDic.Add(key, value);
            }
            return enumDic;
        }

        /// <summary>
        /// 获取枚举下一个值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="src"></param>
        /// <returns></returns>
        public static T Next<T>(this T src) where T : struct
        {
            if (!typeof(T).IsEnum) throw new ArgumentException(String.Format("Argument {0} is not an Enum", typeof(T).FullName));

            T[] Arr = (T[])Enum.GetValues(src.GetType());
            int j = Array.IndexOf<T>(Arr, src) + 1;
            return (Arr.Length == j) ? Arr[0] : Arr[j];
        }
    }
}
