using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace KnifeZ.Extensions.DatabaseAccessor
{
    public class ConnectionStrings
    {
        /// <summary>
        /// 数据库标识名称
        /// </summary>
        public string Key { get; set; }
        /// <summary>
        /// 链接字符串
        /// </summary>
        public string Value { get; set; }
        /// <summary>
        /// 数据库类型
        /// </summary>
        public DBTypeEnum? DbType { get; set; }
        public string Version { get; set; }


        public string DbContext { get; set; }

        public ConstructorInfo DcConstructor;

        public IDataContext CreateDC()
        {
            if (DcConstructor == null)
            {
                var AllAssembly = UtilTools.GetAllAssembly();
                List<ConstructorInfo> cis = new List<ConstructorInfo>();
                if (AllAssembly != null)
                {
                    foreach (var ass in AllAssembly)
                    {
                        try
                        {
                            var t = ass.GetExportedTypes().Where(x => typeof(DbContext).IsAssignableFrom(x) && x.Name != "DbContext" && x.Name != "FrameworkContext" && x.Name != "EmptyContext").ToList();
                            foreach (var st in t)
                            {
                                var ci = st.GetConstructor(new Type[] { typeof(ConnectionStrings) });
                                if (ci != null)
                                {
                                    cis.Add(ci);
                                }
                            }
                        }
                        catch { }
                    }
                    string dcname = DbContext;
                    if (string.IsNullOrEmpty(dcname))
                    {
                        dcname = "DataContext";
                    }
                    DcConstructor = cis.Where(x => x.DeclaringType.Name.ToLower() == dcname.ToLower()).FirstOrDefault();
                    if (DcConstructor == null)
                    {
                        DcConstructor = cis.FirstOrDefault();
                    }
                }
            }
            return (IDataContext)DcConstructor?.Invoke(new object[] { this });
        }

    }
}
