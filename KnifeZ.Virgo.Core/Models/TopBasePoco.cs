
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using KnifeZ.Virgo.Core.Extensions;

namespace KnifeZ.Virgo.Core
{
    /// <summary>
    /// TopBasePoco
    /// </summary>
    public class TopBasePoco
    {
        private Guid _id;

        /// <summary>
        /// Id
        /// </summary>
        [Key]
        public Guid ID
        {
            get; set;
        }

        /// <summary>
        /// 是否选中
        /// 标识当前行数据是否被选中
        /// </summary>
        [NotMapped]
        [JsonIgnore]
        public bool Checked { get; set; }

        /// <summary>
        /// BatchError
        /// </summary>
        [NotMapped]
        [JsonIgnore]
        public string BatchError { get; set; }

        /// <summary>
        /// ExcelIndex
        /// </summary>
        [NotMapped]
        [JsonIgnore]
        public long ExcelIndex { get; set; }

        public object GetID ()
        {
            var idpro = this.GetType().GetSingleProperty("ID");
            var id = idpro.GetValue(this);
            return id;
        }

        public Type GetIDType ()
        {
            var idpro = this.GetType().GetSingleProperty("ID");
            return idpro.PropertyType;
        }

        public void SetID (object id)
        {
            var idpro = this.GetType().GetSingleProperty("ID");
            idpro.SetValue(this, id.ConvertValue(idpro.PropertyType));

        }
    }
}
