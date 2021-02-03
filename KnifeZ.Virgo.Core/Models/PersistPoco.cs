using System.ComponentModel.DataAnnotations;

namespace KnifeZ.Virgo.Core
{
    public interface IPersistPoco
    {
        bool IsValid { get; set; }
    }
    /// <summary>
    /// 所有持久化model的基类，所有的不应被物理删除的model都应该继承这个类
    /// </summary>
    public abstract class PersistPoco : BasePoco
    {
        /// <summary>
        /// IsValid
        /// </summary>
        [Display(Name = "IsValid")]
        public bool IsValid { get; set; } = true;
    }
}
