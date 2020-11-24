using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Mvc;

namespace KnifeZ.Virgo.Core.Extensions
{
    public static class ListVMExtension
    {
        /// <summary>
        /// 获取Jason格式的列表数据
        /// </summary>
        /// <param name="self">是否需要对数据进行Json编码</param>
        /// <returns>Json格式的数据</returns>
        public static string GetDataJson<T>(this IBasePagedListVM<T, BaseSearcher> self) where T : TopBasePoco, new()
        {
            var sb = new StringBuilder();
            self.GetHeaders();
            if (self.IsSearched == false)
            {
                self.DoSearch();
            }
            var el = self.GetEntityList().ToList();
            //如果列表主键都为0，则生成自增主键，避免主键重复
            if (el.All(x => x.ID == Guid.Empty))
            {
                el.ForEach(x => x.ID = Guid.NewGuid());
            }
            //循环生成列表数据
            for (int x = 0; x < el.Count; x++)
            {
                var sou = el[x];
                sb.Append(self.GetSingleDataJson(sou, x));
                if (x < el.Count - 1)
                {
                    sb.Append(",");
                }
            }
            return $"[{sb}]";
        }
        /// <summary>
        /// 生成单条数据的Json格式
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="self"></param>
        /// <param name="obj">数据</param>
        /// <param name="index">index</param>
        /// <returns>Json格式的数据</returns>
        public static string GetSingleDataJson<T>(this IBasePagedListVM<T, BaseSearcher> self, object obj, int index = 0) where T : TopBasePoco
        {
            var sb = new StringBuilder();
            if (obj is not T sou)
            {
                sou = self.CreateEmptyEntity();
            }
            var isSelected = self.GetIsSelected(sou);
            //循环所有列
            sb.Append('{');
            bool containsID = false;
            //bool addHiddenID = false;
            foreach (var baseCol in self.GetHeaders())
            {
                foreach (var col in baseCol.BottomChildren)
                {
                    if (col.ColumnType != GridColumnTypeEnum.Normal)
                    {
                        continue;
                    }
                    if (col.FieldName?.ToLower() == "id")
                    {
                        containsID = true;
                    }
                    //设定列名，如果是主键ID，则列名为id，如果不是主键列，则使用f0，f1,f2...这种方式命名，避免重复
                    var ptype = col.FieldType;
                    if (col.Field?.ToLower() == "children" && typeof(IEnumerable<T>).IsAssignableFrom(ptype))
                    {
                        var children = ((IEnumerable<T>)col.GetObject(obj))?.ToList();
                        if (children == null || children.Count == 0)
                        {
                            continue;
                        }
                    }
                    sb.Append($"\"{col.Field}\":");
                    var html = string.Empty;

                    //if (col.EditType == EditTypeEnum.Text || col.EditType == null)
                    //{
                    if (typeof(IEnumerable<T>).IsAssignableFrom(ptype))
                    {
                        var children = ((IEnumerable<T>)col.GetObject(obj))?.ToList();
                        if (children != null)
                        {
                            html = "[";
                            for (int i = 0; i < children.Count; i++)
                            {
                                var item = children[i];
                                html += self.GetSingleDataJson(item);
                                if (i < children.Count - 1)
                                {
                                    html += ",";
                                }
                            }
                            html += "]";
                        }
                        else
                        {
                            //html = "[]";
                        }
                    }
                    else
                    {
                        html = col.GetText(sou, false).ToString();

                        //如果列是布尔值，直接返回true或false，让前台生成CheckBox
                        if (ptype == typeof(bool) || ptype == typeof(bool?))
                        {
                            if (html != null && html != string.Empty)
                            {
                                html = html.ToLower();
                            }
                        }
                        //如果列是枚举，直接使用枚举的文本作为多语言的Key查询多语言文字
                        else if (ptype.IsEnumOrNullableEnum())
                        {
                            if (int.TryParse(html, out int enumvalue))
                            {
                                html = PropertyHelper.GetEnumDisplayName(ptype, enumvalue);
                            }
                        }
                        //If this column is a class or list, html will be set to a json string, sest inner to true to remove the "
                        if (true && ptype?.Namespace.Equals("System") == false && ptype?.IsEnumOrNullableEnum() == false)
                        {
                        }
                    }
                    //}
                    //else
                    //{
                    //    string val = col.GetText(sou).ToString();
                    //    string name = $"{self.DetailGridPrix}[{index}].{col.Field}";
                    //}
                    //if (string.IsNullOrEmpty(self.DetailGridPrix) == false && addHiddenID == false)
                    //{
                    //    html += $@"<input hidden name='{self.DetailGridPrix}[{index}].ID' value='{sou.GetID()}'/>";
                    //    addHiddenID = true;
                    //}
                    sb.Append(html);
                    sb.Append(',');
                }
            }
            sb.Append($"\"TempIsSelected\":\"{ (isSelected == true ? "1" : "0") }\"");
            if (containsID == false)
            {
                sb.Append($",\"ID\":\"{(sou as dynamic).ID}\"");
            }
            // 标识当前行数据是否被选中
            sb.Append($@",""LAY_CHECKED"":{sou.Checked.ToString().ToLower()}");
            sb.Append(string.Empty);
            sb.Append('}');
            return sb.ToString();
        }

        /// <summary>
        /// Get json format string of ListVM's search result
        /// </summary>
        /// <typeparam name="T">Model type</typeparam>
        /// <param name="self">a listvm</param>
        /// <param name="PlainText">true to return plain text, false to return formated html, such as checkbox,buttons ...</param>
        /// <returns>json string</returns>
        public static string GetJson<T>(this IBasePagedListVM<T, BaseSearcher> self, bool PlainText = true) where T : TopBasePoco, new()
        {
            return $@"{{""Data"":{self.GetDataJson()},""Count"":{self.Searcher.Count},""Page"":{self.Searcher.Page},""PageCount"":{self.Searcher.PageCount},""Msg"":""success"",""Code"":200}}";
        }

        public static string GetError<T>(this IBasePagedListVM<T, BaseSearcher> self) where T : TopBasePoco, new()
        {
            return $@"{{""Data"":{{}},""Count"":0,""Page"":0,""PageCount"":0,""Msg"":""{(self as BaseVM).MSD.GetFirstError()}"",""Code"":400}}";
        }


        /// <summary>
        /// 生成下载文件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="self"></param>
        /// <param name="ExportName"></param>
        /// <returns></returns>
        public static FileContentResult GetExportData<T>(this IBasePagedListVM<T, BaseSearcher> self, string ExportName = "") where T : TopBasePoco, new()
        {
            self.SearcherMode = self.Ids != null && self.Ids.Count > 0 ? ListVMSearchModeEnum.CheckExport : ListVMSearchModeEnum.Export;
            var data = self.GenerateExcel();
            string ContentType = self.ExportExcelCount > 1 ? "application/x-zip-compresse" : "application/vnd.ms-excel";
            ExportName = string.IsNullOrEmpty(ExportName) ? typeof(T).Name : ExportName;
            ExportName = self.ExportExcelCount > 1 ? $"Export_{ExportName}_{DateTime.Now.ToString("yyyyMMddHHmmssffff")}.zip" : $"Export_{ExportName}_{DateTime.Now.ToString("yyyyMMddHHmmssffff")}.xlsx";
            FileContentResult Result = new FileContentResult(data, ContentType);
            Result.FileDownloadName = ExportName;
            return Result;
        }
    }
}
