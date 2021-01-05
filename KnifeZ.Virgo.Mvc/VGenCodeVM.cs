using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using KnifeZ.Virgo.Core;
using KnifeZ.Virgo.Core.Attributes;
using KnifeZ.Virgo.Core.Extensions;
using KnifeZ.Virgo.Mvc.Model;
using Microsoft.Extensions.FileProviders;

namespace KnifeZ.Virgo.Mvc
{
    public class VGenCodeVM : BaseVM
    {
        public VGenCodeModel CodeModel { get; set; }

        public string ModelName => CodeModel.ModelType?.Split(',').FirstOrDefault()?.Split('.').LastOrDefault() ?? "";
        /// <summary>
        /// Model namespace
        /// </summary>
        public string ModelNS => CodeModel.ModelType?.Split(',').FirstOrDefault()?.Split('.').SkipLast(1).ToSpratedString(seperator: ".");
        /// <summary>
        /// 基础路径
        /// </summary>
        public string BaseDir { get; set; } = AppDomain.CurrentDomain.BaseDirectory;
        public string _mainDir;
        /// <summary>
        /// 项目根目录
        /// </summary>
        public string MainDir
        {
            get
            {
                if (_mainDir == null)
                {
                    int? index = BaseDir?.IndexOf($"{Path.DirectorySeparatorChar}bin{Path.DirectorySeparatorChar}Debug{Path.DirectorySeparatorChar}");
                    if (index == null || index < 0)
                    {
                        index = BaseDir?.IndexOf($"{Path.DirectorySeparatorChar}bin{Path.DirectorySeparatorChar}Release{Path.DirectorySeparatorChar}") ?? 0;
                    }

                    _mainDir = BaseDir?.Substring(0, index.Value);
                }
                return _mainDir;
            }
            set
            {
                _mainDir = value;
            }
        }

        private string _mainNs;
        public string MainNS
        {
            get
            {
                int index = MainDir.LastIndexOf(Path.DirectorySeparatorChar);
                if (index > 0)
                {
                    _mainNs = MainDir.Substring(index + 1);
                }
                else
                {
                    _mainNs = MainDir;
                }
                return _mainNs;
            }
            set
            {
                _mainNs = value;
            }

        }

        private string _controllerNs;
        public string ControllerNs
        {
            get
            {
                if (_controllerNs == null)
                {
                    _controllerNs = MainNS + ".Controllers";
                }
                return _controllerNs;
            }
            set
            {
                _controllerNs = value;
            }
        }

        public string _controllerdir;
        public string ControllerDir
        {
            get
            {
                if (_controllerdir == null)
                {
                    if (string.IsNullOrEmpty(CodeModel.Area))
                    {
                        _controllerdir = Directory.CreateDirectory(MainDir + $"{Path.DirectorySeparatorChar}Controllers").FullName;
                    }
                    else
                    {
                        _controllerdir = Directory.CreateDirectory(MainDir + $"{Path.DirectorySeparatorChar}Areas{Path.DirectorySeparatorChar}{CodeModel.Area}{Path.DirectorySeparatorChar}Controllers").FullName;
                    }
                }
                return _controllerdir;
            }
        }

        public string _vmdir;
        /// <summary>
        /// ViewModel项目根目录
        /// </summary>
        public string VmDir
        {
            get
            {
                if (_vmdir == null)
                {
                    var up = Directory.GetParent(MainDir);
                    var vmdir = up.GetDirectories().Where(x => x.Name.ToLower().EndsWith(".viewmodel")).FirstOrDefault();
                    if (vmdir == null)
                    {
                        if (string.IsNullOrEmpty(CodeModel.Area))
                        {
                            vmdir = Directory.CreateDirectory(MainDir + $"{Path.DirectorySeparatorChar}ViewModels{Path.DirectorySeparatorChar}{ModelName}VMs");
                        }
                        else
                        {
                            vmdir = Directory.CreateDirectory(MainDir + $"{Path.DirectorySeparatorChar}CodeModel.Areas{Path.DirectorySeparatorChar}{CodeModel.Area}{Path.DirectorySeparatorChar}ViewModels{Path.DirectorySeparatorChar}{ModelName}VMs");
                        }
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(CodeModel.Area))
                        {
                            vmdir = Directory.CreateDirectory(vmdir.FullName + $"{Path.DirectorySeparatorChar}{ModelName}VMs");
                        }
                        else
                        {
                            vmdir = Directory.CreateDirectory(vmdir.FullName + $"{Path.DirectorySeparatorChar}{CodeModel.Area}{Path.DirectorySeparatorChar}{ModelName}VMs");
                        }

                    }
                    _vmdir = vmdir.FullName;
                }
                return _vmdir;
            }
        }

        private string _vmNs;
        /// <summary>
        /// ViewModel namespace
        /// </summary>
        public string VMNs
        {
            get
            {
                if (_vmNs == null)
                {
                    var up = Directory.GetParent(MainDir);
                    var vmdir = up.GetDirectories().Where(x => x.Name.ToLower().EndsWith(".viewmodel")).FirstOrDefault();
                    if (vmdir == null)
                    {
                        if (string.IsNullOrEmpty(CodeModel.Area))
                        {
                            _vmNs = MainNS + $".ViewModels.{ModelName}VMs";
                        }
                        else
                        {
                            _vmNs = MainNS + $".{CodeModel.Area}.ViewModels.{ModelName}VMs";
                        }
                    }
                    else
                    {
                        int index = vmdir.FullName.LastIndexOf(Path.DirectorySeparatorChar);
                        if (index > 0)
                        {
                            _vmNs = vmdir.FullName.Substring(index + 1);
                        }
                        else
                        {
                            _vmNs = vmdir.FullName;
                        }
                        if (string.IsNullOrEmpty(CodeModel.Area))
                        {
                            _vmNs += $".{ModelName}VMs";
                        }
                        else
                        {
                            _vmNs += $".{CodeModel.Area}.{ModelName}VMs";
                        }
                    }
                }
                return _vmNs;
            }
            set
            {
                _vmNs = value;
            }
        }

        /// <summary>
        /// 生成模板
        /// </summary>
        /// <returns></returns>
        public string GenTemplates ()
        {
            var genList = new Dictionary<string, string>
            {
                //生成ViewModel
                { $"{VmDir}{Path.DirectorySeparatorChar}{ModelName}VM.cs", GenerateVM("CurdVM") },
                { $"{VmDir}{Path.DirectorySeparatorChar}{ModelName}ListVM.cs", GenerateVM("ListVM") },
                { $"{VmDir}{Path.DirectorySeparatorChar}{ModelName}BatchVM.cs", GenerateVM("BatchVM") },
                { $"{VmDir}{Path.DirectorySeparatorChar}{ModelName}ImportVM.cs", GenerateVM("ImportVM") },
                { $"{VmDir}{Path.DirectorySeparatorChar}{ModelName}Searcher.cs", GenerateVM("Searcher") },
                //生成Controller
                { $"{ControllerDir}{Path.DirectorySeparatorChar}{ModelName}Controller.cs", GenApiController() }
            };
            //生成Vue
            var vueDir = ConfigInfo.AppSettings["VueDir"];
            genList.Add($"{vueDir}{Path.DirectorySeparatorChar}src{Path.DirectorySeparatorChar}views{Path.DirectorySeparatorChar}{ModelName.ToLower()}{Path.DirectorySeparatorChar}index.vue"
                , GenerateVue("index"));
            genList.Add($"{vueDir}{Path.DirectorySeparatorChar}src{Path.DirectorySeparatorChar}views{Path.DirectorySeparatorChar}{ModelName.ToLower()}{Path.DirectorySeparatorChar}api{Path.DirectorySeparatorChar}index.js"
                , GenerateVue("api.index"));
            genList.Add($"{vueDir}{Path.DirectorySeparatorChar}src{Path.DirectorySeparatorChar}views{Path.DirectorySeparatorChar}{ModelName.ToLower()}{Path.DirectorySeparatorChar}views{Path.DirectorySeparatorChar}dialog-form.vue"
                , GenerateVue("views.dialog-form"));

            //附加枚举
            string enumPath = $"{vueDir}{Path.DirectorySeparatorChar}src{Path.DirectorySeparatorChar}configs{Path.DirectorySeparatorChar}enums.js";
            genList.Add(enumPath, GenerateEnums(enumPath));

            foreach (var item in genList)
            {
                if (!Directory.Exists(Path.GetDirectoryName(item.Key)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(item.Key));
                }
                if (!File.Exists(item.Key))
                {
                    File.Create(item.Key).Close();
                }
                using (StreamWriter sw = new StreamWriter(item.Key, false, Encoding.UTF8))
                {
                    sw.WriteLine(item.Value);
                }
            }
            if (CodeModel.IsInsertMenu)
            {
                var allowedRole = DC.Set<FrameworkRole>().Where(x => x.RoleCode == "001").FirstOrDefault();
                var actions = new List<string> { "Get", "Edit", "Search", "Add", "BatchDelete", "ExportExcel", "ExportExcelByIds", "GetExcelTemplate", "Import" };
                FrameworkMenu menu = new FrameworkMenu
                {
                    PageName = CodeModel.ModelName,
                    ActionName = null,
                    ModuleName = CodeModel.ModelName,
                    ClassName = ControllerNs + "," + ModelName,
                    MethodName = null,
                    Url = "/" + ModelName.ToLower(),
                    ICon = "subtract",
                    Privileges = new List<FunctionPrivilege>() { new FunctionPrivilege { RoleId = allowedRole.ID, Allowed = true } },
                    ShowOnMenu = true,
                    FolderOnly = false,
                    Children = new List<FrameworkMenu>(),
                    IsPublic = false,
                    IsInside = true,
                    DisplayOrder = 99,
                    CreateTime = DateTime.Now
                };
                int order = 1;
                //添加常规方法
                foreach (var item in actions)
                {
                    var child = new FrameworkMenu()
                    {
                        PageName = "MenuKey." + item,
                        ActionName = item,
                        ModuleName = CodeModel.ModelName,
                        ClassName = ControllerNs + "," + ModelName,
                        MethodName = item,
                        Url = "/api/" + ModelName + "/" + item,
                        Privileges = new List<FunctionPrivilege>(){
                            new FunctionPrivilege{ RoleId = allowedRole.ID, Allowed = true}
                        },
                        ShowOnMenu = false,
                        FolderOnly = false,
                        Children = new List<FrameworkMenu>(),
                        IsPublic = false,
                        IsInside = true,
                        DisplayOrder = order++,
                        CreateTime = DateTime.Now,
                        ParentId = menu.ID
                    };
                    if (item == "Get")
                    {
                        child.Url = "/api/" + ModelName + "/{id}";
                    }
                    menu.Children.Add(child);
                }
                List<string> existSubPro = new List<string>();
                //添加拓展方法
                foreach (var item in CodeModel.FieldInfos.Where(x => x.IsSearcherField == true || x.IsFormField == true).ToList())
                {
                    if ((item.InfoType == FieldInfoType.One2Many || item.InfoType == FieldInfoType.Many2Many) && item.SubField != "`file")
                    {
                        var subtype = Type.GetType(item.LinkedType);
                        var subpro = subtype.GetProperties().Where(x => x.Name == item.SubField).FirstOrDefault();
                        var key = subtype.FullName + ":" + subpro.Name;
                        existSubPro.Add(key);
                        int count = existSubPro.Where(x => x == key).Count();
                        if (count == 1)
                        {
                            string acitonName = "Get" + subtype.Name + "List";
                            menu.Children.Add(new FrameworkMenu()
                            {
                                PageName = "获取" + subtype.Name + "列表",
                                ActionName = "获取" + subtype.Name + "列表",
                                ModuleName = CodeModel.ModelName,
                                ClassName = ControllerNs + "," + ModelName,
                                MethodName = acitonName,
                                Url = "/api/" + ModelName + "/" + acitonName,
                                Privileges = new List<FunctionPrivilege>(){
                                    new FunctionPrivilege{ RoleId = allowedRole.ID, Allowed = true}
                                },
                                ShowOnMenu = false,
                                FolderOnly = false,
                                Children = new List<FrameworkMenu>(),
                                IsPublic = false,
                                IsInside = true,
                                DisplayOrder = order++,
                                CreateTime = DateTime.Now,
                                ParentId = menu.ID

                            });
                        }
                    }
                }
                //插入菜单表
                DC.Set<FrameworkMenu>().Add(menu);
                DC.SaveChanges();
            }
            return Program._localizer["GenerateSuccess"];
        }

        #region 生成模板代码
        public string GenApiController ()
        {
            string rv = GetResource("Controller.txt")
                .Replace("$vmnamespace$", VMNs)
                .Replace("$namespace$", ControllerNs)
                .Replace("$des$", CodeModel.ModelName)
                .Replace("$modelname$", ModelName)
                .Replace("$modelnamespace$", ModelNS)
                .Replace("$controllername$", $"{ModelName}");
            if (string.IsNullOrEmpty(CodeModel.Area))
            {
                rv = rv.Replace("$area$", "");
            }
            else
            {
                rv = rv.Replace("$area$", $"[Area(\"{CodeModel.Area}\")]");
            }
            #region 附加方法

            StringBuilder other = new StringBuilder();
            List<VFieldInfo> pros = CodeModel.FieldInfos.Where(x => x.IsSearcherField == true || x.IsFormField == true).ToList();
            List<string> existSubPro = new List<string>();
            for (int i = 0; i < pros.Count; i++)
            {
                var item = pros[i];
                if ((item.InfoType == FieldInfoType.One2Many || item.InfoType == FieldInfoType.Many2Many) && item.SubField != "`file")
                {
                    var subtype = Type.GetType(item.LinkedType);
                    var subpro = subtype.GetProperties().Where(x => x.Name == item.SubField).FirstOrDefault();
                    var key = subtype.FullName + ":" + subpro.Name;
                    existSubPro.Add(key);
                    int count = existSubPro.Where(x => x == key).Count();
                    if (count == 1)
                    {
                        other.AppendLine($@"
        [ActionDescription(""获取{subtype.Name}列表"")]
        [HttpGet(""[action]"")]
        public ActionResult Get{subtype.Name}List()
        {{
            return Ok(DC.Set<{subtype.Name}>().GetSelectListItems(LoginUserInfo?.DataPrivileges, null, x => x.{item.SubField}));
        }}");
                    }
                }
            }
            rv = rv.Replace("$other$", other.ToString());
            #endregion
            return rv;
        }

        public string GenerateVM (string name)
        {
            var rv = GetResource($"{name}.txt")
                .Replace("$modelnamespace$", ModelNS)
                .Replace("$vmnamespace$", VMNs)
                .Replace("$modelname$", ModelName)
                .Replace("$area$", $"{CodeModel.Area ?? ""}")
                .Replace("$classname$", $"{ModelName}");
            if (name == "Searcher" || name == "BatchVM")
            {
                string prostring = "";
                string initstr = "";
                Type modelType = Type.GetType(CodeModel.ModelType);
                List<VFieldInfo> pros = null;
                if (name == "Searcher")
                {
                    pros = CodeModel.FieldInfos.Where(x => x.IsSearcherField == true).ToList();
                }
                if (name == "BatchVM")
                {
                    pros = CodeModel.FieldInfos.Where(x => x.IsBatchField == true).ToList();
                }
                foreach (var pro in pros)
                {
                    //对于一对一或者一对多的搜索和批量修改字段，需要在vm中生成对应的变量来获取关联表的数据
                    if (pro.InfoType != FieldInfoType.Normal)
                    {
                        var subtype = Type.GetType(pro.LinkedType);
                        if (typeof(TopBasePoco).IsAssignableFrom(subtype) == false || subtype == typeof(FileAttachment))
                        {
                            continue;
                        }
                    }

                    //生成普通字段定义
                    var proType = modelType.GetProperties().Where(x => x.Name == pro.FieldName).FirstOrDefault();
                    var display = proType.GetCustomAttribute<DisplayAttribute>();
                    if (display != null)
                    {
                        prostring += $@"
        [Display(Name = ""{display.Name}"")]";
                    }
                    string typename = proType.PropertyType.Name;
                    string proname = pro.GetField(DC, modelType);

                    switch (pro.InfoType)
                    {
                        case FieldInfoType.Normal:
                            if (proType.PropertyType.IsNullable())
                            {
                                typename = proType.PropertyType.GetGenericArguments()[0].Name + "?";
                            }
                            else if (proType.PropertyType != typeof(string))
                            {
                                typename = proType.PropertyType.Name + "?";
                            }
                            break;
                        case FieldInfoType.One2Many:
                            typename = pro.GetFKType(DC, modelType);
                            if (typename != "string")
                            {
                                typename += "?";
                            }
                            break;
                        case FieldInfoType.Many2Many:
                            proname = $@"Selected{pro.FieldName}IDs";
                            typename = $"List<{pro.GetFKType(DC, modelType)}>";
                            break;
                        default:
                            break;
                    }
                    if (typename == "DateTime" || typename == "DateTime?")
                    {
                        typename = "DateRange";
                    }
                    prostring += $@"
        public {typename} {proname} {{ get; set; }}";
                }
                rv = rv.Replace("$pros$", prostring).Replace("$init$", initstr);
                rv = GetRelatedNamespace(pros, rv);
            }
            if (name == "ListVM")
            {
                string headerstring = "";
                string selectstring = "";
                string wherestring = "";
                string subprostring = "";
                string formatstring = "";
                string actionstring = "";

                var pros = CodeModel.FieldInfos.Where(x => x.IsListField == true).ToList();
                Type modelType = Type.GetType(CodeModel.ModelType);
                List<PropertyInfo> existSubPro = new List<PropertyInfo>();
                foreach (var pro in pros)
                {
                    if (pro.InfoType == FieldInfoType.Normal)
                    {
                        headerstring += $@"
                this.MakeGridHeader(x => x.{pro.FieldName}),";
                        if (pro.FieldName.ToLower() != "id")
                        {
                            selectstring += $@"
                    {pro.FieldName} = x.{pro.FieldName},";
                        }
                    }
                    else
                    {
                        var subtype = Type.GetType(pro.LinkedType);
                        if (subtype == typeof(FileAttachment))
                        {
                            var filefk = DC.GetFKName2(modelType, pro.FieldName);
                            headerstring += $@"
                this.MakeGridHeader(x => x.{filefk}).SetFormat({filefk}Format),";
                            selectstring += $@"
                    {filefk} = x.{filefk},";
                            formatstring += GetResource("HeaderFormat.txt").Replace("$modelname$", ModelName).Replace("$field$", filefk).Replace("$classname$", $"{ModelName}");
                        }
                        else
                        {
                            var subpro = subtype.GetProperties().Where(x => x.Name == pro.SubField).FirstOrDefault();
                            existSubPro.Add(subpro);
                            string prefix = "";
                            int count = existSubPro.Where(x => x.Name == subpro.Name).Count();
                            if (count > 1)
                            {
                                prefix = count + "";
                            }
                            string subtypename = subpro.PropertyType.Name;
                            if (subpro.PropertyType.IsNullable())
                            {
                                subtypename = subpro.PropertyType.GetGenericArguments()[0].Name + "?";
                            }

                            var subdisplay = subpro.GetCustomAttribute<DisplayAttribute>();
                            headerstring += $@"
                this.MakeGridHeader(x => x.{subpro.DeclaringType.Name + "_" + pro.SubField + prefix}),";
                            if (pro.InfoType == FieldInfoType.One2Many)
                            {
                                selectstring += $@"
                    {subpro.DeclaringType.Name + "_" + pro.SubField + prefix} = x.{pro.FieldName}.{pro.SubField},";
                            }
                            else
                            {
                                var middleType = modelType.GetSingleProperty(pro.FieldName).PropertyType.GenericTypeArguments[0];
                                var middlename = DC.GetPropertyNameByFk(middleType, pro.SubIdField);
                                selectstring += $@"
                    {subpro.DeclaringType.Name + "_" + pro.SubField + prefix} = x.{pro.FieldName}.Select(y=>y.{middlename}.{pro.SubField}).ToSpratedString(null,"",""), ";
                            }
                            if (subdisplay?.Name != null)
                            {
                                subprostring += $@"
        [Display(Name = ""{subdisplay.Name}"")]";
                            }
                            subprostring += $@"
        public {subtypename} {subpro.DeclaringType.Name + "_" + pro.SubField + prefix} {{ get; set; }}";
                        }
                    }

                }
                var wherepros = CodeModel.FieldInfos.Where(x => x.IsSearcherField == true).ToList();
                foreach (var pro in wherepros)
                {
                    if (pro.SubField == "`file")
                    {
                        continue;
                    }
                    var proType = modelType.GetProperties().Where(x => x.Name == pro.FieldName).Select(x => x.PropertyType).FirstOrDefault();

                    switch (pro.InfoType)
                    {
                        case FieldInfoType.Normal:
                            if (proType == typeof(string))
                            {
                                wherestring += $@"
                .CheckContain(Searcher.{pro.FieldName}, x=>x.{pro.FieldName})";
                            }
                            else if (proType == typeof(DateTime) || proType == typeof(DateTime?))
                            {
                                wherestring += $@"
                .CheckBetween(Searcher.{pro.FieldName}?.GetStartTime(), Searcher.{pro.FieldName}?.GetEndTime(), x => x.{pro.FieldName}, includeMax: false)";
                            }
                            else
                            {
                                wherestring += $@"
                .CheckEqual(Searcher.{pro.FieldName}, x=>x.{pro.FieldName})";
                            }
                            break;
                        case FieldInfoType.One2Many:
                            var fk = DC.GetFKName2(modelType, pro.FieldName);
                            wherestring += $@"
                .CheckEqual(Searcher.{fk}, x=>x.{fk})";
                            break;
                        case FieldInfoType.Many2Many:
                            var subtype = Type.GetType(pro.LinkedType);
                            var fk2 = DC.GetFKName(modelType, pro.FieldName);
                            wherestring += $@"
                .CheckWhere(Searcher.Selected{pro.FieldName}IDs,x=>DC.Set<{proType.GetGenericArguments()[0].Name}>().Where(y=>Searcher.Selected{pro.FieldName}IDs.Contains(y.{pro.SubIdField})).Select(z=>z.{fk2}).Contains(x.ID))";
                            break;
                        default:
                            break;
                    }
                }
                rv = rv.Replace("$headers$", headerstring).Replace("$where$", wherestring).Replace("$select$", selectstring).Replace("$subpros$", subprostring).Replace("$format$", formatstring).Replace("$actions$", actionstring);
                rv = GetRelatedNamespace(pros, rv);
            }
            if (name == "CurdVM")
            {
                string prostr = "";
                string initstr = "";
                string includestr = "";
                string addstr = "";
                string editstr = "";
                var pros = CodeModel.FieldInfos.Where(x => x.IsFormField == true && string.IsNullOrEmpty(x.LinkedType) == false).ToList();
                foreach (var pro in pros)
                {
                    var subtype = Type.GetType(pro.LinkedType);
                    if (typeof(TopBasePoco).IsAssignableFrom(subtype) == false || subtype == typeof(FileAttachment))
                    {
                        continue;
                    }
                    var fname = "All" + pro.FieldName + "s";
                    prostr += $@"
        public List<ComboSelectListItem> {fname} {{ get; set; }}";
                    initstr += $@"
            {fname} = DC.Set<{subtype.Name}>().GetSelectListItems(LoginUserInfo?.DataPrivileges, null, y => y.{pro.SubField});";
                    includestr += $@"
            SetInclude(x => x.{pro.FieldName});";

                    if (pro.InfoType == FieldInfoType.Many2Many)
                    {
                        Type modelType = Type.GetType(CodeModel.ModelType);
                        var protype = modelType.GetProperties().Where(x => x.Name == pro.FieldName).FirstOrDefault();
                        prostr += $@"
        [Display(Name = ""{protype.GetPropertyDisplayName()}"")]
        public List<{pro.GetFKType(DC, modelType)}> Selected{pro.FieldName}IDs {{ get; set; }}";
                        initstr += $@"
            Selected{pro.FieldName}IDs = Entity.{pro.FieldName}?.Select(x => x.{pro.SubIdField}).ToList();";
                        addstr += $@"
            Entity.{pro.FieldName} = new List<{protype.PropertyType.GetGenericArguments()[0].Name}>();
            if (Selected{pro.FieldName}IDs != null)
            {{
                foreach (var id in Selected{pro.FieldName}IDs)
                {{
                    Entity.{pro.FieldName}.Add(new {protype.PropertyType.GetGenericArguments()[0].Name} {{ {pro.SubIdField} = id }});
                }}
            }}
";
                        editstr += $@"
            Entity.{pro.FieldName} = new List<{protype.PropertyType.GetGenericArguments()[0].Name}>();
            if(Selected{pro.FieldName}IDs != null )
            {{
                Selected{pro.FieldName}IDs.ForEach(x => Entity.{pro.FieldName}.Add(new {protype.PropertyType.GetGenericArguments()[0].Name} {{ ID = Guid.NewGuid(), {pro.SubIdField} = x }}));
            }}
";
                    }
                }

                rv = rv.Replace("$pros$", "").Replace("$init$", "").Replace("$include$", includestr).Replace("$add$", "").Replace("$edit$", "");
                rv = GetRelatedNamespace(pros, rv);
            }
            if (name == "ImportVM")
            {
                string prostring = "";
                string initstr = "";
                Type modelType = Type.GetType(CodeModel.ModelType);
                List<VFieldInfo> pros = CodeModel.FieldInfos.Where(x => x.IsImportField == true).ToList();
                foreach (var pro in pros)
                {
                    if (pro.InfoType == FieldInfoType.Many2Many)
                    {
                        continue;
                    }

                    if (string.IsNullOrEmpty(pro.LinkedType) == false)
                    {
                        var subtype = Type.GetType(pro.LinkedType);
                        if (typeof(TopBasePoco).IsAssignableFrom(subtype) == false || subtype == typeof(FileAttachment))
                        {
                            continue;
                        }
                        initstr += $@"
            {pro.FieldName + "_Excel"}.DataType = ColumnDataType.ComboBox;
            {pro.FieldName + "_Excel"}.ListItems = DC.Set<{subtype.Name}>().GetSelectListItems(LoginUserInfo?.DataPrivileges, null, y => y.{pro.SubField});";
                    }
                    var proType = modelType.GetProperties().Where(x => x.Name == pro.FieldName).FirstOrDefault();
                    var display = proType.GetCustomAttribute<DisplayAttribute>();
                    var filefk = DC.GetFKName2(modelType, pro.FieldName);
                    if (display != null)
                    {
                        prostring += $@"
        [Display(Name = ""{display.Name}"")]";
                    }
                    if (string.IsNullOrEmpty(pro.LinkedType) == false)
                    {
                        prostring += $@"
        public ExcelPropety {pro.FieldName + "_Excel"} = ExcelPropety.CreateProperty<{ModelName}>(x => x.{filefk});";
                    }
                    else
                    {
                        prostring += $@"
        public ExcelPropety {pro.FieldName + "_Excel"} = ExcelPropety.CreateProperty<{ModelName}>(x => x.{pro.FieldName});";
                    }
                }
                rv = rv.Replace("$pros$", prostring).Replace("$init$", initstr);
                rv = GetRelatedNamespace(pros, rv);

            }
            return rv;
        }

        public string GenerateVue (string name)
        {
            var rv = GetResource($"{name}.txt", "vue").Replace("$modelname$", Utils.ToFirstLower(ModelName));
            List<VFieldInfo> fields = CodeModel.FieldInfos.ToList();
            StringBuilder sbQueryInfos = new StringBuilder();
            StringBuilder sbQueryItems = new StringBuilder();
            StringBuilder sbColumns = new StringBuilder();
            StringBuilder sbImports = new StringBuilder();
            StringBuilder sbExtAPIs = new StringBuilder();
            StringBuilder sbExtData = new StringBuilder();
            StringBuilder sbExtCreated = new StringBuilder();
            StringBuilder sbFields = new StringBuilder();
            List<string> existSubPro = new List<string>();
            var modelProps = Type.GetType(CodeModel.ModelType).GetProperties();
            //循环所有字段
            for (int i = 0; i < fields.Count; i++)
            {
                var item = fields[i];
                if ((item.InfoType == FieldInfoType.One2Many || item.InfoType == FieldInfoType.Many2Many) && item.SubField != "`file")
                {
                    var subtype = Type.GetType(item.LinkedType);
                    var subpro = subtype.GetProperties().Where(x => x.Name == item.SubField).FirstOrDefault();
                    var key = subtype.FullName + ":" + subpro.Name;
                    existSubPro.Add(key);
                    int count = existSubPro.Where(x => x == key).Count();
                    if (count == 1)
                    {
                        if (name == "api.index")
                        {
                            sbExtAPIs.AppendLine($@"
	get{subtype.Name}List(data) {{
		return request({{
			url: reqPath + 'Get{subtype.Name}List',
			method: 'get',
			data: data,
		}})
	}},");
                        }
                        if (name == "index" || name == "views.dialog-form")
                        {
                            if (item.IsFormField || item.IsListField)
                            {
                                sbExtData.AppendLine($@"
			{Utils.ToFirstLower(subtype.Name)}ListData: [],");
                                sbExtCreated.AppendLine($@"
		apiEvents.get{subtype.Name}List().then((res) => {{
            this.{Utils.ToFirstLower(subtype.Name)}ListData = res.Data
        }})");

                            }
                        }
                    }
                }

                if (item.IsListField)
                {
                    sbColumns.Append($@"
				{{ key: '{item.FieldName}', title: '{item.FieldDes}' }},");
                }

                if (item.IsSearcherField)
                {
                    sbQueryInfos.Append($@"
				{item.FieldName}:'',");
                    // TODO query slot
                    sbQueryItems.Append($@"
				<a-form-item label=""{item.FieldDes}"" name=""{item.FieldName}"">
					<a-input type=""text"" v-model:value=""queryInfos.{item.FieldName}""></a-input>
				</a-form-item>");

                }

                if (item.IsFormField)
                {
                    var proType = modelProps.Where(x => x.Name == item.FieldName).Select(x => x.PropertyType).FirstOrDefault();
                    Type checktype = proType;
                    if (proType.IsNullable())
                    {
                        checktype = proType.GetGenericArguments()[0];
                    }
                    if (checktype.IsBoolOrNullableBool())
                    {
                        sbFields.Append($@"
				{{
					title: '{item.FieldDes}',
					key: '{item.FieldName}',
					type: 'switch'
				}},");

                    }
                    else if (checktype.IsEnum())
                    {
                        sbImports.Append($@"{item.FieldName}Types,");
                        sbFields.Append($@"
				{{
					title: '{item.FieldDes}',
					key: '{item.FieldName}',
					type: 'radio',
					props: {{
						items: {item.FieldName}Types,
					}}
				}},");

                    }
                    else
                    {
                        sbFields.Append($@"
				{{
					title: '{item.FieldDes}',
					key: '{item.FieldName}',
					type: 'input'
				}},");

                    }
                }
            }

            if (name == "index")
            {
                rv = rv.Replace("$queryInfo$", sbQueryInfos.ToString())
                    .Replace("$queryItems$", sbQueryItems.ToString())
                    .Replace("$columns$", sbColumns.ToString())
                    .Replace("$extData$", sbExtData.ToString())
                    .Replace("$extCreated$", sbExtCreated.ToString())
                    .Replace("$imports$", "import {" + sbImports.ToString() + "} from '@/configs/enums.js'");
            }
            if (name == "views.dialog-form")
            {
                rv = rv.Replace("$fields$", sbFields.ToString())
                    .Replace("$extData$", sbExtData.ToString())
                    .Replace("$extCreated$", sbExtCreated.ToString())
                    .Replace("$imports$", "import {" + sbImports.ToString() + "} from '@/configs/enums.js'");
            }
            if (name == "api.index")
            {
                rv = rv.Replace("$extAPIs$", sbExtAPIs.ToString());
            }
            return rv;
        }

        public string GenerateEnums (string path)
        {
            string content = File.ReadAllText(path);
            StringBuilder enumstr = new StringBuilder();
            List<string> existEnum = new List<string>();

            var pros = CodeModel.FieldInfos.Where(x => x.IsListField == true || x.IsSearcherField == true).ToList();
            foreach (var item in pros)
            {
                var proType = Type.GetType(CodeModel.ModelType).GetProperties().Where(x => x.Name == item.FieldName).Select(x => x.PropertyType).FirstOrDefault();
                Type checktype = proType;
                if (proType.IsNullable())
                {
                    checktype = proType.GetGenericArguments()[0];
                }
                if (checktype.IsEnum())
                {
                    if (existEnum.Contains(checktype.Name) == false && !content.Contains(item.FieldName + "Types"))
                    {
                        var es = checktype.ToListItems();
                        enumstr.AppendLine($@"export const {item.FieldName}Types = [");
                        for (int a = 0; a < es.Count; a++)
                        {
                            var e = es[a];
                            enumstr.Append($@"	{{ Text: '{e.Text}', Value: {e.Value} }}");
                            enumstr.AppendLine();
                        }
                        enumstr.AppendLine($@"];");
                        existEnum.Add(checktype.Name);
                    }
                }
            }
            return content + enumstr.ToString();
        }

        #endregion

        public List<CodeGenListView> GetFieldInfos (string modelFullName)
        {
            Type modeltype = Type.GetType(modelFullName);
            var pros = modeltype.GetProperties();
            List<CodeGenListView> lv = new List<CodeGenListView>();
            int count = 0;
            Type[] basetype = new Type[] { typeof(BasePoco), typeof(TopBasePoco), typeof(PersistPoco) };
            List<string> ignoreField = new List<string>();
            foreach (var pro in pros)
            {
                if (basetype.Contains(pro.DeclaringType) == false)
                {
                    if (pro.CanWrite == false)
                    {
                        continue;
                    }
                    if (pro.Name.ToLower() == "id" && pro.PropertyType != typeof(string))
                    {
                        continue;
                    }
                    CodeGenListView view = new CodeGenListView()
                    {
                        FieldName = pro.Name,
                        FieldDes = pro.GetPropertyDisplayName(),
                        SubIdField = "",
                        Index = count
                    };
                    var notmapped = pro.GetCustomAttributes(typeof(NotMappedAttribute), false).FirstOrDefault();
                    Type checktype = pro.PropertyType;
                    if (pro.PropertyType.IsNullable())
                    {
                        checktype = pro.PropertyType.GetGenericArguments()[0];
                    }
                    if (ignoreField.Contains(checktype.Name))
                    {
                        continue;
                    }
                    bool show = false;
                    view.IsFormField = true;
                    view.IsListField = true;
                    view.IsImportField = true;
                    if (checktype.IsPrimitive || checktype == typeof(string) || checktype == typeof(DateTime) || checktype.IsEnum() || checktype == typeof(decimal))
                    {
                        show = true;
                    }
                    if (typeof(TopBasePoco).IsAssignableFrom(checktype))
                    {
                        var fk = DC.GetFKName2(modeltype, pro.Name);
                        if (fk != null)
                        {
                            ignoreField.Add(fk);
                            show = true;
                        }
                        if (checktype == typeof(FileAttachment))
                        {
                            view.IsImportField = false;
                            view.FieldDes += $"({Program._localizer["Attachment"]})";
                        }
                        else
                        {
                            view.FieldDes += $"({Program._localizer["OneToMany"]})";
                        }
                        view.LinkedType = checktype.AssemblyQualifiedName;
                        view.SubSelectItems = GetLinkFields(checktype.AssemblyQualifiedName);
                    }
                    if (checktype.IsList())
                    {
                        checktype = pro.PropertyType.GetGenericArguments()[0];
                        if (checktype.IsNullable())
                        {
                            checktype = checktype.GetGenericArguments()[0];
                        }
                        var middletable = checktype.GetCustomAttributes(typeof(MiddleTableAttribute), false).FirstOrDefault();
                        if (middletable != null)
                        {
                            view.FieldDes += $"({Program._localizer["ManyToMany"]})";
                            view.IsImportField = false;
                            var subpros = checktype.GetProperties();
                            foreach (var spro in subpros)
                            {
                                if (basetype.Contains(spro.DeclaringType) == false)
                                {
                                    Type subchecktype = spro.PropertyType;
                                    if (spro.PropertyType.IsNullable())
                                    {
                                        subchecktype = spro.PropertyType.GetGenericArguments()[0];
                                    }
                                    if (typeof(TopBasePoco).IsAssignableFrom(subchecktype) && subchecktype != modeltype)
                                    {
                                        view.LinkedType = subchecktype.AssemblyQualifiedName;
                                        view.SubSelectItems = GetLinkFields(subchecktype.AssemblyQualifiedName);
                                        var fk = DC.GetFKName2(checktype, spro.Name);
                                        view.SubIdField = fk;
                                        show = true;
                                    }
                                }
                            }
                        }
                    }
                    if (notmapped != null)
                    {
                        view.FieldDes += "(NotMapped)";
                        view.IsFormField = false;
                        view.IsSearcherField = false;
                        view.IsBatchField = false;
                        view.IsImportField = false;
                        view.IsListField = false;
                    }
                    if (show == true)
                    {
                        lv.Add(view);
                        count++;
                    }
                }
            }

            for (int i = 0; i < lv.Count(); i++)
            {
                if (ignoreField.Contains(lv[i].FieldName))
                {
                    for (int j = i; j < lv.Count(); j++)
                    {
                        lv[j].Index--;
                    }
                    lv.RemoveAt(i);
                    i--;
                }
            }
            var res = lv.AsQueryable().OrderBy(x => x.FieldName).ToList();
            return res;
        }
        //获取关联表字段
        private List<ComboSelectListItem> GetLinkFields (string linkedType)
        {
            if (string.IsNullOrEmpty(linkedType) == false)
            {
                var linktype = Type.GetType(linkedType);
                if (linktype != typeof(FileAttachment))
                {
                    var subpros = linktype.GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance).Where(x => x.GetMemberType() == typeof(string) && x.Name != "BatchError").OrderBy(x => x.Name).ToList().ToListItems(x => x.Name, x => x.Name);
                    var subproswithname = subpros.Where(x => x.Text.ToLower().Contains("name")).ToList();
                    var subproswithoutname = subpros.Where(x => x.Text.ToLower().Contains("name") == false).ToList();
                    subpros = new List<ComboSelectListItem>();
                    subpros.AddRange(subproswithname);
                    subpros.AddRange(subproswithoutname);
                    if (subpros.Count == 0)
                    {
                        subpros.Add(new ComboSelectListItem { Text = "Id", Value = "Id" });
                    }
                    return subpros;
                }
            }
            return new List<ComboSelectListItem>();

        }


        public static string GetResource (string fileName, string subdir = "")
        {
            //获取编译在程序中的Controller原始代码文本
            Assembly assembly = Assembly.GetExecutingAssembly();
            string loc;
            if (string.IsNullOrEmpty(subdir))
            {
                loc = $"KnifeZ.Virgo.Mvc.Areas.templates.{fileName}";
            }
            else
            {
                loc = $"KnifeZ.Virgo.Mvc.Areas.templates.{subdir}.{fileName}";
            }
            var textStreamReader = new StreamReader(assembly.GetManifestResourceStream(loc));
            string content = textStreamReader.ReadToEnd();
            textStreamReader.Close();
            return content;
        }
        private string GetRelatedNamespace (List<VFieldInfo> pros, string s)
        {
            string otherns = @"";
            Type modelType = Type.GetType(CodeModel.ModelType);
            foreach (var pro in pros)
            {
                Type proType = null;

                if (string.IsNullOrEmpty(pro.LinkedType))
                {
                    proType = modelType.GetProperties().Where(x => x.Name == pro.FieldName).Select(x => x.PropertyType).FirstOrDefault();
                }
                else
                {
                    proType = Type.GetType(pro.LinkedType);
                }
                string prons = proType.Namespace;
                if (proType.IsNullable())
                {
                    prons = proType.GetGenericArguments()[0].Namespace;
                }
                if (s.Contains($"using {prons}") == false && otherns.Contains($"using {prons}") == false)
                {
                    otherns += $@"using {prons};
";
                }

            }

            return s.Replace("$othernamespace$", otherns);
        }

    }

    public class VGenCodeModel
    {
        public string ModelName { get; set; }
        public List<VFieldInfo> FieldInfos { get; set; }
        public List<CodeGenListView> ConfigFields { get; set; }
        public bool IsInsertMenu { get; set; }

        public string Area { get; set; }

        public string ModelType { get; set; }

    }


    public class CodeGenListView : BasePoco
    {
        [Display(Name = "FieldName")]
        public string FieldName { get; set; }

        [Display(Name = "FieldDes")]
        public string FieldDes { get; set; }


        [Display(Name = "IsSearcherField")]
        public bool IsSearcherField { get; set; }

        [Display(Name = "IsListField")]
        public bool IsListField { get; set; }

        [Display(Name = "IsFormField")]
        public bool IsFormField { get; set; }


        [Display(Name = "SubField")]
        public string SubField { get; set; }

        public string SubIdField { get; set; }

        public List<ComboSelectListItem> SubSelectItems { get; set; }

        [Display(Name = "IsImportField")]
        public bool IsImportField { get; set; }

        [Display(Name = "IsBatchField")]
        public bool IsBatchField { get; set; }

        public int Index { get; set; }

        [Display(Name = "LinkedType")]
        public string LinkedType { get; set; }

    }

}
