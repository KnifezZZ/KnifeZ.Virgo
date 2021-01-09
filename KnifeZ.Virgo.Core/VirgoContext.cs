using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using KnifeZ.Virgo.Core.Auth;
using KnifeZ.Virgo.Core.Extensions;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using System.Reflection;
using KnifeZ.Virgo.Core.Support.Json;
using System.Threading.Tasks;
using System.Linq.Expressions;
using Microsoft.Extensions.Options;
using System.Text.RegularExpressions;

namespace KnifeZ.Virgo.Core
{
    public class VirgoContext
    {
        private HttpContext _httpContext;
        public HttpContext HttpContext { get => _httpContext; }

        private IServiceProvider _serviceProvider;
        public IServiceProvider ServiceProvider { get => _serviceProvider ?? _httpContext?.RequestServices; }


        private List<IDataPrivilege> _dps;
        public List<IDataPrivilege> DataPrivilegeSettings { get => _dps; }

        private Configs _configInfo;
        public Configs ConfigInfo { get => _configInfo; }

        private GlobalData _globaInfo;
        public GlobalData GlobaInfo { get => _globaInfo; }

        private IDistributedCache _cache;
        public IDistributedCache Cache
        {
            get
            {
                if (_cache == null)
                {
                    _cache = (IDistributedCache)HttpContext.RequestServices.GetService(typeof(IDistributedCache));
                }
                return _cache;
            }
        }

        public string CurrentCS { get; set; }

        public DBTypeEnum? CurrentDbType { get; set; }

        public ISessionService Session { get; set; }

        public IModelStateService MSD { get; set; }

        #region DataContext

        private IDataContext _dc;
        public IDataContext DC
        {
            get
            {
                if (_dc == null)
                {
                    _dc = this.CreateDC();
                }
                return _dc;
            }
            set
            {
                _dc = value;
            }
        }

        #endregion

        #region Current User

        private LoginUserInfo _loginUserInfo;
        public LoginUserInfo LoginUserInfo
        {
            get
            {
                if (HttpContext?.User?.Identity?.IsAuthenticated == true && _loginUserInfo == null) // 用户认证通过后，当前上下文不包含用户数据
                {
                    var userIdStr = HttpContext.User.Claims.SingleOrDefault(x => x.Type == AuthConstants.JwtClaimTypes.Subject).Value;
                    Guid userId = Guid.Parse(userIdStr);
                    var cacheKey = $"{GlobalConstants.CacheKey.UserInfo}:{userIdStr}";
                    _loginUserInfo = Cache.Get<LoginUserInfo>(cacheKey);
                    if (_loginUserInfo == null || _loginUserInfo.Id != userId)
                    {
                        try
                        {
                            _loginUserInfo = LoadUserFromDB(userId).Result;
                        }
                        catch { }
                        if (_loginUserInfo != null)
                        {
                            Cache.Add(cacheKey, _loginUserInfo);
                        }
                        else
                        {
                            //HttpContext.ChallengeAsync().Wait();
                            return null;
                        }
                    }
                }
                return _loginUserInfo;
            }
            set
            {
                if (value == null)
                {
                    Cache.Delete($"{GlobalConstants.CacheKey.UserInfo}:{_loginUserInfo?.Id}");
                    _loginUserInfo = value;
                }
                else
                {
                    _loginUserInfo = value;
                    Cache.Add($"{GlobalConstants.CacheKey.UserInfo}:{_loginUserInfo.Id}", value);
                }
            }
        }

        private IStringLocalizerFactory _stringLocalizerFactory;
        private IStringLocalizer _localizer;
        private ILoggerFactory _loggerFactory;
        public IStringLocalizer Localizer
        {
            get
            {
                if (_localizer == null && _stringLocalizerFactory != null)
                {
                    var programtype = Assembly.GetEntryAssembly().GetTypes().Where(x => x.Name == "Program").FirstOrDefault();
                    _localizer = _stringLocalizerFactory.Create(programtype);
                }
                return _localizer ?? KnifeZ.Virgo.Core.Program._localizer;
            }
        }
        /// <summary>
        /// 从数据库读取用户
        /// </summary>
        /// <param name="userId">用户ID，如果为空，则使用用户名和密码查询</param>
        /// <param name="itcode">用户名</param>
        /// <param name="password">密码</param>
        /// <returns>用户信息</returns>
        public virtual async Task<LoginUserInfo> LoadUserFromDB (Guid? userId, string itcode = null, string password = null)
        {
            FrameworkUserBase userInfo = null;
            if (DC == null)
            {
                return null;
            }
            if (userId.HasValue)
            {
                userInfo = await DC.Set<FrameworkUserBase>()
                                    .Include(x => x.UserRoles)
                                    .Include(x => x.UserGroups)
                                    .Where(x => x.ID == userId)
                                    .SingleOrDefaultAsync();
            }
            else
            {
                userInfo = await DC.Set<FrameworkUserBase>()
                                    .Include(x => x.UserRoles)
                                    .Include(x => x.UserGroups)
                                    .Where(x => x.ITCode.ToLower() == itcode.ToLower() && x.Password == Utils.GetMD5String(password) && x.IsValid)
                                    .SingleOrDefaultAsync();

            }
            LoginUserInfo rv = null;
            if (userInfo != null)
            {
                // 初始化用户信息
                var roleIDs = userInfo.UserRoles.Select(x => x.RoleId).ToList();
                var groupIDs = userInfo.UserGroups.Select(x => x.GroupId).ToList();


                var dataPris = await DC.Set<DataPrivilege>().AsNoTracking()
                                .Where(x => x.UserId == userInfo.ID || (x.GroupId != null && groupIDs.Contains(x.GroupId.Value)))
                                .Distinct()
                                .ToListAsync();
                ProcessTreeDp(dataPris);

                //查找登录用户的页面权限
                var funcPrivileges = await DC.Set<FunctionPrivilege>().AsNoTracking()
                    .Where(x => x.UserId == userInfo.ID || (x.RoleId != null && roleIDs.Contains(x.RoleId.Value)))
                    .Distinct()
                    .ToListAsync();

                var roles = DC.Set<FrameworkRole>().AsNoTracking().Where(x => roleIDs.Contains(x.ID)).ToList();
                var groups = DC.Set<FrameworkGroup>().AsNoTracking().Where(x => groupIDs.Contains(x.ID)).ToList();

                rv = new LoginUserInfo()
                {
                    Id = userInfo.ID,
                    ITCode = userInfo.ITCode,
                    Name = userInfo.Name,
                    PhotoId = userInfo.PhotoId,
                    Roles = roles.Select(x => new SimpleRole { ID = x.ID, RoleCode = x.RoleCode, RoleName = x.RoleName }).ToList(),
                    Groups = groups.Select(x => new SimpleGroup { ID = x.ID, GroupCode = x.GroupCode, GroupName = x.GroupName }).ToList(),
                    DataPrivileges = dataPris.Select(x => new SimpleDataPri { ID = x.ID, RelateId = x.RelateId, TableName = x.TableName, UserId = x.UserId, GroupId = x.GroupId }).ToList(),
                    FunctionPrivileges = funcPrivileges.Select(x => new SimpleFunctionPri { ID = x.ID, UserId = x.UserId, RoleId = x.RoleId, Allowed = x.Allowed, MenuItemId = x.MenuItemId }).ToList()
                };
            }
            return rv;
        }
        #endregion

        #region GUID
        public List<EncHash> EncHashs
        {
            get
            {
                return ReadFromCache<List<EncHash>>("EncHashs", () =>
                {
                    using (var dc = this.CreateDC())
                    {
                        return dc.Set<EncHash>().ToList();
                    }
                });
            }
        }
        #endregion

        #region URL
        public string BaseUrl { get; set; }
        #endregion

        public SimpleLog Log { get; set; }

        protected ILogger<ActionLog> Logger { get; set; }

        public VirgoContext (IOptions<Configs> _config, GlobalData _gd = null, IHttpContextAccessor _http = null,  List<IDataPrivilege> _dp = null, IDataContext dc = null, IStringLocalizerFactory stringLocalizer = null, ILoggerFactory loggerFactory = null)
        {
            _configInfo = _config.Value;
            _globaInfo = _gd ?? new GlobalData();
            _httpContext = _http?.HttpContext;
            _stringLocalizerFactory = stringLocalizer;
            _loggerFactory = loggerFactory;
            this.Logger = loggerFactory?.CreateLogger<ActionLog>();
            if (_httpContext == null)
            {
                MSD = new BasicMSD();
            }
            if (_dp == null)
            {
                _dp = new List<IDataPrivilege>();
            }
            _dps = _dp;
            if (dc is NullContext)
            {
                _dc = null;
            }
            else
            {
                _dc = dc;
            }
        }

        public void SetServiceProvider (IServiceProvider sp)
        {
            this._serviceProvider = sp;
        }


        public T ReadFromCache<T> (string key, Func<T> setFunc, int? timeout = null)
        {
            if (Cache.TryGetValue(key, out T rv) == false || rv == null)
            {
                T data = setFunc();
                if (timeout == null)
                {
                    Cache.Add(key, data);
                }
                else
                {
                    Cache.Add(key, data, new DistributedCacheEntryOptions()
                    {
                        SlidingExpiration = new TimeSpan(0, 0, timeout.Value)
                    });
                }
                return data;
            }
            else
            {
                return rv;
            }
        }

        public async Task RemoveUserCache (
            params string[] userIds)
        {
            foreach (var userId in userIds)
            {
                var key = $"{GlobalConstants.CacheKey.UserInfo}:{userId}";
                await Cache.DeleteAsync(key);
            }
        }


        #region CreateDC
        public virtual IDataContext CreateDC (bool isLog = false, string cskey = null)
        {
            string cs = cskey ?? CurrentCS;
            if (isLog == true)
            {
                if (ConfigInfo.ConnectionStrings?.Where(x => x.Key.ToLower() == "defaultlog").FirstOrDefault() != null)
                {
                    cs = "defaultlog";
                }
                else
                {
                    cs = "default";
                }
            }
            if (cs == null)
            {
                cs = "default";
            }
            var rv = ConfigInfo.ConnectionStrings.Where(x => x.Key.ToLower() == cs).FirstOrDefault().CreateDC();
            rv.IsDebug = ConfigInfo.IsQuickDebug;
            rv.SetLoggerFactory(_loggerFactory);
            return rv;
        }

        #endregion

        /// <summary>
        /// 判断某URL是否有权限访问
        /// </summary>
        /// <param name="url">url地址</param>
        /// <returns>true代表可以访问，false代表不能访问</returns>
        public bool IsAccessable (string url)
        {
            // 如果是调试 或者 url 为 null or 空字符串
            if (_configInfo.IsQuickDebug || string.IsNullOrEmpty(url))
            {
                return true;
            }
            //循环所有不限制访问的url，如果含有当前判断的url，则认为可以访问
            var publicActions = _globaInfo.AllAccessUrls;
            foreach (var au in publicActions)
            {
                if (new Regex(au + "[/\\?]?", RegexOptions.IgnoreCase).IsMatch(url))
                {
                    return true;
                }
            }
            //如果没有任何页面权限，则直接返回false
            if (LoginUserInfo?.FunctionPrivileges == null)
            {
                return false;
            }


            url = Regex.Replace(url, "/do(batch.*)", "/$1", RegexOptions.IgnoreCase);

            //如果url以#开头，一般是javascript使用的临时地址，不需要判断，直接返回true
            url = url.Trim();

            if (url.StartsWith("#"))
            {
                return true;
            }
            var menus = _globaInfo.AllMenus;
            var menu = Utils.FindMenu(url, GlobaInfo.AllMenus);
            //如果最终没有找到，说明系统菜单中并没有配置这个url，返回false
            if (menu == null)
            {
                return false;
            }
            //如果找到了，则继续验证其他权限
            else
            {
                return IsAccessable(menu, menus);
            }
        }

        /// <summary>
        /// 判断某菜单是否有权限访问
        /// </summary>
        /// <param name="menu">菜单项</param>
        /// <param name="menus">所有系统菜单</param>
        /// <returns>true代表可以访问，false代表不能访问</returns>
        public bool IsAccessable (SimpleMenu menu, List<SimpleMenu> menus)
        {
            //寻找当前菜单的页面权限
            var find = LoginUserInfo?.FunctionPrivileges.Where(x => x.MenuItemId == menu.ID && x.Allowed == true).FirstOrDefault();
            //如果能找到直接对应的页面权限
            if (find != null)
            {
                return true;
            }
            return false;
        }



        public void DoLog (string msg, ActionLogTypesEnum logtype = ActionLogTypesEnum.Normal)
        {
            var log = this.Log?.GetActionLog();
            if (log == null)
            {
                log = new ActionLog();
            }
            log.LogType = logtype;
            log.ActionTime = DateTime.Now;
            log.Remark = msg;
            LogLevel ll = LogLevel.Information;
            switch (logtype)
            {
                case ActionLogTypesEnum.Normal:
                    ll = LogLevel.Information;
                    break;
                case ActionLogTypesEnum.Exception:
                    ll = LogLevel.Error;
                    break;
                case ActionLogTypesEnum.Debug:
                    ll = LogLevel.Debug;
                    break;
                default:
                    break;
            }

            Logger?.Log<ActionLog>(ll, new EventId(), log, null, (a, b) =>
            {
                return $@"
===Log Start===
内容:{a.Remark}
地址:{a.ActionUrl}
时间:{a.ActionTime}
===Log End===
";
            });
        }


        #region CreateVM
        /// <summary>
        /// Create a ViewModel, and pass Session,cache,dc...etc to the viewmodel
        /// </summary>
        /// <param name="VMType">The type of the viewmodel</param>
        /// <param name="Id">If the viewmodel is a BaseCRUDVM, the data having this id will be fetched</param>
        /// <param name="Ids">If the viewmodel is a BatchVM, the BatchVM's Ids property will be assigned</param>
        /// <param name="values">properties of the viewmodel that you want to assign values</param>
        /// <param name="passInit">if true, the viewmodel will not call InitVM internally</param>
        /// <returns>ViewModel</returns>
        private BaseVM CreateVM (Type VMType, object Id = null, object[] Ids = null, Dictionary<string, object> values = null, bool passInit = false)
        {
            //Use reflection to create viewmodel
            var ctor = VMType.GetConstructor(Type.EmptyTypes);
            BaseVM rv = ctor.Invoke(null) as BaseVM;
            rv.KnifeVirgo = this;

            rv.FC = new Dictionary<string, object>();
            rv.CreatorAssembly = this.GetType().AssemblyQualifiedName;
            rv.ControllerName = this.HttpContext?.Request?.Path;
            if (HttpContext != null && HttpContext.Request != null)
            {
                try
                {
                    if (HttpContext.Request.QueryString != QueryString.Empty)
                    {
                        foreach (var key in HttpContext.Request.Query.Keys)
                        {
                            if (rv.FC.Keys.Contains(key) == false)
                            {
                                rv.FC.Add(key, HttpContext.Request.Query[key]);
                            }
                        }
                    }
                    if (HttpContext.Request.HasFormContentType)
                    {
                        var f = HttpContext.Request.Form;
                        foreach (var key in f.Keys)
                        {
                            if (rv.FC.Keys.Contains(key) == false)
                            {
                                rv.FC.Add(key, f[key]);
                            }
                        }
                    }
                }
                catch { }
            }
            //try to set values to the viewmodel's matching properties
            if (values != null)
            {
                foreach (var v in values)
                {
                    PropertyHelper.SetPropertyValue(rv, v.Key, v.Value, null, false);
                }
            }
            //if viewmodel is derrived from BaseCRUDVM<> and Id has value, call ViewModel's GetById method
            if (Id != null && rv is IBaseCRUDVM<TopBasePoco> cvm)
            {
                cvm.SetEntityById(Id);
            }
            //if viewmodel is derrived from IBaseBatchVM<>，set ViewMode's Ids property,and init it's ListVM and EditModel properties
            if (rv is IBaseBatchVM<BaseVM> temp)
            {
                temp.Ids = Array.Empty<string>();
                if (Ids != null)
                {
                    var tempids = new List<string>();
                    foreach (var iid in Ids)
                    {
                        tempids.Add(iid.ToString());
                    }
                    temp.Ids = tempids.ToArray();
                }
                if (temp.ListVM != null)
                {
                    temp.ListVM.CopyContext(rv);
                    temp.ListVM.Ids = Ids == null ? new List<string>() : temp.Ids.ToList();
                    temp.ListVM.SearcherMode = ListVMSearchModeEnum.Batch;
                    temp.ListVM.NeedPage = false;
                }
                if (temp.LinkedVM != null)
                {
                    temp.LinkedVM.CopyContext(rv);
                }
                if (temp.ListVM != null)
                {
                    //Remove the action columns from list
                    temp.ListVM.OnAfterInitList += (self) =>
                    {
                        self.RemoveActionColumn();
                        if (temp.ErrorMessage.Count > 0)
                        {
                            self.AddErrorColumn();
                        }
                    };
                    temp.ListVM.DoInitListVM();
                    if (temp.ListVM.Searcher != null)
                    {
                        var searcher = temp.ListVM.Searcher;
                        searcher.CopyContext(rv);
                        if (passInit == false)
                        {
                            searcher.DoInit();
                        }
                    }
                }
                temp.LinkedVM?.DoInit();
                //temp.ListVM.DoSearch();
            }
            //if the viewmodel is a ListVM, Init it's searcher
            if (rv is IBasePagedListVM<TopBasePoco, ISearcher> lvm)
            {
                var searcher = lvm.Searcher;
                searcher.CopyContext(rv);
                if (passInit == false)
                {
                    searcher.DoInit();
                }
                lvm.DoInitListVM();

            }
            if (rv is IBaseImport<BaseTemplateVM> tvm)
            {
                var template = tvm.Template;
                template.CopyContext(rv);
                template.DoInit();
            }

            //if passinit is not set, call the viewmodel's DoInit method
            if (passInit == false)
            {
                rv.DoInit();
            }
            return rv;
        }

        /// <summary>
        /// Create a ViewModel, and pass Session,cache,dc...etc to the viewmodel
        /// </summary>
        /// <typeparam name="T">The type of the viewmodelThe type of the viewmodel</typeparam>
        /// <param name="values">use Lambda to set viewmodel's properties,use && for multiply properties, for example CreateVM<Test>(values: x=>x.Field1=='a' && x.Field2 == 'b'); will set viewmodel's Field1 to 'a' and Field2 to 'b'</param>
        /// <param name="passInit">if true, the viewmodel will not call InitVM internally</param>
        /// <returns>ViewModel</returns>
        public T CreateVM<T> (Expression<Func<T, object>> values = null, bool passInit = false) where T : BaseVM
        {
            SetValuesParser p = new SetValuesParser();
            var dir = p.Parse(values);
            return CreateVM(typeof(T), null, Array.Empty<object>(), dir, passInit) as T;
        }

        /// <summary>
        /// Create a ViewModel, and pass Session,cache,dc...etc to the viewmodel
        /// </summary>
        /// <typeparam name="T">The type of the viewmodelThe type of the viewmodel</typeparam>
        /// <param name="Id">If the viewmodel is a BaseCRUDVM, the data having this id will be fetched</param>
        /// <param name="values">properties of the viewmodel that you want to assign values</param>
        /// <param name="passInit">if true, the viewmodel will not call InitVM internally</param>
        /// <returns>ViewModel</returns>
        public T CreateVM<T> (object Id, Expression<Func<T, object>> values = null, bool passInit = false) where T : BaseVM
        {
            SetValuesParser p = new SetValuesParser();
            var dir = p.Parse(values);
            return CreateVM(typeof(T), Id, Array.Empty<object>(), dir, passInit) as T;
        }

        /// <summary>
        /// Create a ViewModel, and pass Session,cache,dc...etc to the viewmodel
        /// </summary>
        /// <typeparam name="T">The type of the viewmodelThe type of the viewmodel</typeparam>
        /// <param name="Ids">If the viewmodel is a BatchVM, the BatchVM's Ids property will be assigned</param>
        /// <param name="values">use Lambda to set viewmodel's properties,use && for multiply properties, for example CreateVM<Test>(values: x=>x.Field1=='a' && x.Field2 == 'b'); will set viewmodel's Field1 to 'a' and Field2 to 'b'</param>
        /// <param name="passInit">if true, the viewmodel will not call InitVM internally</param>
        /// <returns>ViewModel</returns>
        public T CreateVM<T> (object[] Ids, Expression<Func<T, object>> values = null, bool passInit = false) where T : BaseVM
        {
            SetValuesParser p = new SetValuesParser();
            var dir = p.Parse(values);
            return CreateVM(typeof(T), null, Ids, dir, passInit) as T;
        }


        /// <summary>
        /// Create a ViewModel, and pass Session,cache,dc...etc to the viewmodel
        /// </summary>
        /// <typeparam name="T">The type of the viewmodelThe type of the viewmodel</typeparam>
        /// <param name="Ids">If the viewmodel is a BatchVM, the BatchVM's Ids property will be assigned</param>
        /// <param name="values">use Lambda to set viewmodel's properties,use && for multiply properties, for example CreateVM<Test>(values: x=>x.Field1=='a' && x.Field2 == 'b'); will set viewmodel's Field1 to 'a' and Field2 to 'b'</param>
        /// <param name="passInit">if true, the viewmodel will not call InitVM internally</param>
        /// <returns>ViewModel</returns>
        public T CreateVM<T> (Guid[] Ids, Expression<Func<T, object>> values = null, bool passInit = false) where T : BaseVM
        {
            SetValuesParser p = new SetValuesParser();
            var dir = p.Parse(values);
            return CreateVM(typeof(T), null, Ids.Cast<object>().ToArray(), dir, passInit) as T;
        }

        /// <summary>
        /// Create a ViewModel, and pass Session,cache,dc...etc to the viewmodel
        /// </summary>
        /// <typeparam name="T">The type of the viewmodelThe type of the viewmodel</typeparam>
        /// <param name="Ids">If the viewmodel is a BatchVM, the BatchVM's Ids property will be assigned</param>
        /// <param name="values">use Lambda to set viewmodel's properties,use && for multiply properties, for example CreateVM<Test>(values: x=>x.Field1=='a' && x.Field2 == 'b'); will set viewmodel's Field1 to 'a' and Field2 to 'b'</param>
        /// <param name="passInit">if true, the viewmodel will not call InitVM internally</param>
        /// <returns>ViewModel</returns>
        public T CreateVM<T> (int[] Ids, Expression<Func<T, object>> values = null, bool passInit = false) where T : BaseVM
        {
            SetValuesParser p = new SetValuesParser();
            var dir = p.Parse(values);
            return CreateVM(typeof(T), null, Ids.Cast<object>().ToArray(), dir, passInit) as T;
        }

        /// <summary>
        /// Create a ViewModel, and pass Session,cache,dc...etc to the viewmodel
        /// </summary>
        /// <typeparam name="T">The type of the viewmodelThe type of the viewmodel</typeparam>
        /// <param name="Ids">If the viewmodel is a BatchVM, the BatchVM's Ids property will be assigned</param>
        /// <param name="values">use Lambda to set viewmodel's properties,use && for multiply properties, for example CreateVM<Test>(values: x=>x.Field1=='a' && x.Field2 == 'b'); will set viewmodel's Field1 to 'a' and Field2 to 'b'</param>
        /// <param name="passInit">if true, the viewmodel will not call InitVM internally</param>
        /// <returns>ViewModel</returns>
        public T CreateVM<T> (long[] Ids, Expression<Func<T, object>> values = null, bool passInit = false) where T : BaseVM
        {
            SetValuesParser p = new SetValuesParser();
            var dir = p.Parse(values);
            return CreateVM(typeof(T), null, Ids.Cast<object>().ToArray(), dir, passInit) as T;
        }
        /// <summary>
        /// Create a ViewModel, and pass Session,cache,dc...etc to the viewmodel
        /// </summary>
        /// <typeparam name="T">The type of the viewmodelThe type of the viewmodel</typeparam>
        /// <param name="Ids">If the viewmodel is a BatchVM, the BatchVM's Ids property will be assigned</param>
        /// <param name="values">use Lambda to set viewmodel's properties,use && for multiply properties, for example CreateVM<Test>(values: x=>x.Field1=='a' && x.Field2 == 'b'); will set viewmodel's Field1 to 'a' and Field2 to 'b'</param>
        /// <param name="passInit">if true, the viewmodel will not call InitVM internally</param>
        /// <returns>ViewModel</returns>
        public T CreateVM<T> (string[] Ids, Expression<Func<T, object>> values = null, bool passInit = false) where T : BaseVM
        {
            SetValuesParser p = new SetValuesParser();
            var dir = p.Parse(values);
            return CreateVM(typeof(T), null, Ids.Cast<object>().ToArray(), dir, passInit) as T;
        }

        /// <summary>
        /// Create a ViewModel, and pass Session,cache,dc...etc to the viewmodel
        /// </summary>
        /// <param name="VmFullName">the fullname of the viewmodel's type</param>
        /// <param name="Id">If the viewmodel is a BaseCRUDVM, the data having this id will be fetched</param>
        /// <param name="Ids">If the viewmodel is a BatchVM, the BatchVM's Ids property will be assigned</param>
        /// <param name="passInit">if true, the viewmodel will not call InitVM internally</param>
        /// <returns>ViewModel</returns>
        public BaseVM CreateVM (string VmFullName, object Id = null, object[] Ids = null, bool passInit = false)
        {
            return CreateVM(Type.GetType(VmFullName), Id, Ids, null, passInit);
        }
        #endregion


        private void ProcessTreeDp (List<DataPrivilege> dps)
        {
            var dpsSetting = DataPrivilegeSettings;
            foreach (var dp in dpsSetting)
            {
                if (typeof(TreePoco).IsAssignableFrom(dp.ModelType))
                {
                    var ids = dps.Where(x => x.TableName == dp.ModelName).Select(x => x.RelateId).ToList();
                    if (ids.Count > 0 && ids.Contains(null) == false)
                    {
                        var skipids = dp.GetTreeParentIds(this, dps);
                        List<string> subids = new List<string>();
                        subids.AddRange(GetSubIds(dp, ids, dp.ModelType, skipids));
                        subids = subids.Distinct().ToList();
                        subids.ForEach(x => dps.Add(new DataPrivilege
                        {
                            TableName = dp.ModelName,
                            RelateId = x.ToString()
                        }));
                    }
                }

            }
        }
        private IEnumerable<string> GetSubIds (IDataPrivilege dp, List<string> p_id, Type modelType, List<string> skipids)
        {
            var ids = p_id.Where(x => skipids.Contains(x) == false).ToList();
            var subids = dp.GetTreeSubIds(this, ids);
            if (subids.Count > 0)
            {
                return subids.Concat(GetSubIds(dp, subids, modelType, skipids));
            }
            else
            {
                return new List<string>();
            }
        }

    }

}
