
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;

using KnifeZ.Virgo.Core.Extensions;
using KnifeZ.Virgo.Core.Support.Json;
using System.Text.Json.Serialization;

namespace KnifeZ.Virgo.Core
{

    /// <summary>
    /// 所有ViewModel的基类，提供了基本的功能
    /// </summary>
    public class BaseVM : IBaseVM
    {
        /// <summary>
        /// BaseVM
        /// </summary>
        public BaseVM ()
        {
            FC = new Dictionary<string, object>();
        }

        #region Property

        [JsonIgnore]
        public VirgoContext KnifeVirgo { get; set; }

        private Guid _uniqueId;
        /// <summary>
        /// VM实例的Id
        /// </summary>
        [JsonIgnore]
        public string UniqueId
        {
            get
            {
                if (_uniqueId == Guid.Empty)
                {
                    _uniqueId = Guid.NewGuid();
                }
                return _uniqueId.ToString("N");
            }
        }

        private IDataContext _dc;
        /// <summary>
        /// 数据库环境
        /// </summary>
        [JsonIgnore]
        public IDataContext DC
        {
            get
            {
                if (_dc == null)
                {
                    return KnifeVirgo?.DC;
                }
                else
                {
                    return _dc;
                }
            }
            set
            {
                _dc = value;
            }
        }

        /// <summary>
        /// 获取VM的全名
        /// </summary>
        [JsonIgnore]
        public string VMFullName
        {
            get
            {
                var name = GetType().AssemblyQualifiedName;
                name = name.Substring(0, name.LastIndexOf(", Version="));
                return name;
            }
        }

        /// <summary>
        /// 获取VM所在Dll
        /// </summary>
        [JsonIgnore]
        public string CreatorAssembly
        {
            get; set;
        }

        /// <summary>
        /// 获取当前使用的连接字符串
        /// </summary>
        [JsonIgnore]
        public string CurrentCS { get => KnifeVirgo?.CurrentCS; }

        /// <summary>
        /// 记录Controller中传递过来的表单数据
        /// </summary>
        [JsonIgnore]
        public Dictionary<string, object> FC { get; set; }

        /// <summary>
        /// 获取配置文件的信息
        /// </summary>
        [JsonIgnore]
        public Configs ConfigInfo { get => KnifeVirgo?.ConfigInfo; }

        [JsonIgnore]
        public IDistributedCache Cache { get => KnifeVirgo?.Cache; }

        /// <summary>
        /// 当前登录人信息
        /// </summary>
        [JsonIgnore]
        public LoginUserInfo LoginUserInfo { get => KnifeVirgo?.LoginUserInfo; }

        /// <summary>
        /// 当前Url
        /// </summary>
        [JsonIgnore]
        public string CurrentUrl { get => KnifeVirgo?.BaseUrl; }

        /// <summary>
        /// Session信息
        /// </summary>
        [JsonIgnore]
        public ISessionService Session { get => KnifeVirgo?.Session; }

        /// <summary>
        /// Controller传递过来的ModelState信息
        /// </summary>
        [JsonIgnore]
        public IModelStateService MSD { get => KnifeVirgo?.MSD; }

        /// <summary>
        /// 用于保存删除的附件ID
        /// </summary>
        [JsonIgnore]
        public List<string> DeletedFileIds { get; set; } = new List<string>();

        [JsonIgnore]
        public string ControllerName { get; set; }

        [JsonIgnore]
        public IStringLocalizer Localizer { get => KnifeVirgo?.Localizer; }
        #endregion

        #region Event

        /// <summary>
        /// InitVM完成后触发的事件
        /// </summary>
        public event Action<IBaseVM> OnAfterInit;
        /// <summary>
        /// ReInitVM完成后触发的事件
        /// </summary>
        public event Action<IBaseVM> OnAfterReInit;

        #endregion

        #region Method

        /// <summary>
        /// 调用 InitVM 并触发 OnAfterInit 事件
        /// </summary>
        public void DoInit ()
        {
            InitVM();
            OnAfterInit?.Invoke(this);
        }

        /// <summary>
        /// 调用 ReInitVM 并触发 OnAfterReInit 事件
        /// </summary>
        public void DoReInit ()
        {
            ReInitVM();
            OnAfterReInit?.Invoke(this);
        }



        /// <summary>
        /// 初始化ViewModel，框架会在创建VM实例之后自动调用本函数
        /// </summary>
        protected virtual void InitVM ()
        {
        }

        /// <summary>
        /// 从新初始化ViewModel，框架会在验证失败时自动调用本函数
        /// </summary>
        protected virtual void ReInitVM ()
        {
            InitVM();
        }

        /// <summary>
        /// 验证函数，MVC会在提交数据的时候自动调用本函数
        /// </summary>
        /// <returns></returns>
        public virtual void Validate ()
        {
            return;
        }

        /// <summary>
        /// 将源VM的上数据库上下文，Session，登录用户信息，模型状态信息，缓存信息等内容复制到本VM中
        /// </summary>
        /// <param name="vm">复制的源</param>
        public void CopyContext (BaseVM vm)
        {
            KnifeVirgo = vm.KnifeVirgo;
            FC = vm.FC;
            CreatorAssembly = vm.CreatorAssembly;
        }

        #endregion

    }
}
