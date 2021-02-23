using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KnifeZ.Extensions.DatabaseAccessor;
using KnifeZ.Virgo.Core;
using KnifeZ.Virgo.Core.Extensions;
using KnifeZ.Extensions;

namespace KnifeZ.Virgo.Mvc.Model
{
    public enum FieldInfoType { Normal, One2Many, Many2Many }

    public class VFieldInfo
    {
        public string FieldName { get; set; }
        public string FieldDes { get; set; }
        public string LinkedType { get; set; }

        public bool IsSearcherField { get; set; }

        public bool IsListField { get; set; }

        public bool IsFormField { get; set; }

        public bool IsImportField { get; set; }
        public bool IsBatchField { get; set; }

        public FieldInfoType InfoType
        {
            get
            {
                if (string.IsNullOrEmpty(LinkedType))
                {
                    return FieldInfoType.Normal;
                }
                else
                {
                    if (string.IsNullOrEmpty(SubIdField))
                    {
                        return FieldInfoType.One2Many;
                    }
                    else
                    {
                        return FieldInfoType.Many2Many;
                    }
                }
            }
        }

        /// <summary>
        /// 字段关联的类名
        /// </summary>
        public string SubField { get; set; }
        /// <summary>
        /// 多对多关系时，记录中间表关联到主表的字段名称
        /// </summary>
        public string SubIdField { get; set; }

        public string GetField (IDataContext DC, Type modelType)
        {
            if (this.InfoType == FieldInfoType.One2Many)
            {
                var fk = DC.GetFKName2(modelType, this.FieldName);
                return fk;
            }
            else
            {
                return this.FieldName;
            }
        }

        public string GetFKType (IDataContext DC, Type modelType)
        {
            Type fktype = null;
            if (this.InfoType == FieldInfoType.One2Many)
            {
                var fk = this.GetField(DC, modelType);
                fktype = modelType.GetProperties().Where(x => x.Name == fk).Select(x => x.PropertyType).FirstOrDefault();
            }
            if (this.InfoType == FieldInfoType.Many2Many)
            {
                var middletype = modelType.GetProperties().Where(x => x.Name == this.FieldName).Select(x => x.PropertyType).FirstOrDefault();
                fktype = middletype.GetGenericArguments()[0].GetProperties().Where(x => x.Name == this.SubIdField).Select(x => x.PropertyType).FirstOrDefault();
            }
            var typename = "string";

            if (fktype == typeof(short) || fktype == typeof(short?))
            {
                typename = "short";
            }
            if (fktype == typeof(int) || fktype == typeof(int?))
            {
                typename = "int";
            }
            if (fktype == typeof(long) || fktype == typeof(long?))
            {
                typename = "long";
            }
            if (fktype == typeof(Guid) || fktype == typeof(Guid?))
            {
                typename = "Guid";
            }

            return typename;

        }
    }
}
