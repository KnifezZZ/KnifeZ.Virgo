using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
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
        public static List<T> GetDataJson<T>(this IBasePagedListVM<T, BaseSearcher> self) where T : TopBasePoco, new()
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
            return el;
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
            Hashtable ht = new Hashtable
            {
                ["Data"] = self.GetDataJson(),
                ["Count"] = self.Searcher.Count,
                ["Page"] = self.Searcher.Page,
                ["PageCount"] = self.Searcher.PageCount,
                ["Msg"] = "success",
                ["Code"] = "200"
            };
            return JsonSerializer.Serialize(ht);
        }

        public static string GetError<T>(this IBasePagedListVM<T, BaseSearcher> self) where T : TopBasePoco, new()
        {
            return $@"{{""Data"":{{}},""Count"":0,""Page"":0,""PageCount"":0,""Msg"":""{(self as BaseVM).MSD.GetFirstError()}"",""Code"":400}}";
        }
    }
}
