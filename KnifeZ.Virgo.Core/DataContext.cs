using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.Logging.Debug;
using Microsoft.Extensions.Options;
using Npgsql;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using KnifeZ.Virgo.Core.Extensions;
using MySqlConnector;
using KnifeZ.Virgo.Core.Support.Json;
using System.Threading;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using KnifeZ.Extensions.DatabaseAccessor;

namespace KnifeZ.Virgo.Core
{
    /// <summary>
    /// FrameworkContext
    /// </summary>
    public partial class FrameworkContext : EmptyContext, IDataContext
    {
        public DbSet<FrameworkMenu> BaseFrameworkMenus { get; set; }
        public DbSet<FunctionPrivilege> BaseFunctionPrivileges { get; set; }
        public DbSet<DataPrivilege> BaseDataPrivileges { get; set; }
        public DbSet<FileAttachment> BaseFileAttachments { get; set; }
        public DbSet<FrameworkRole> BaseFrameworkRoles { get; set; }
        public DbSet<FrameworkGroup> BaseFrameworkGroups { get; set; }
        public DbSet<ActionLog> BaseActionLogs { get; set; }

        public DbSet<PersistedGrant> PersistedGrants { get; set; }


        /// <summary>
        /// FrameworkContext
        /// </summary>
        public FrameworkContext () : base()
        {
        }

        /// <summary>
        /// FrameworkContext
        /// </summary>
        /// <param name="cs"></param>
        public FrameworkContext (string cs) : base(cs)
        {
        }

        public FrameworkContext (string cs, DBTypeEnum dbtype, string version = null) : base(cs, dbtype, version)
        {
        }

        public FrameworkContext (ConnectionStrings cs) : base(cs)
        {
        }
        public FrameworkContext (DbContextOptions options) : base(options) { }

        /// <summary>
        /// OnModelCreating
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected override void OnModelCreating (ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            //菜单和菜单权限的级联删除
            modelBuilder.Entity<FunctionPrivilege>().HasOne(x => x.MenuItem).WithMany(x => x.Privileges).HasForeignKey(x => x.MenuItemId).OnDelete(DeleteBehavior.Cascade);
            //用户和用户搜索条件级联删除
            //modelBuilder.Entity<SearchCondition>().HasOne(x => x.User).WithMany(x => x.SearchConditions).HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<DataPrivilege>().HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<DataPrivilege>().HasOne(x => x.Group).WithMany().HasForeignKey(x => x.GroupId).OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<FrameworkUserBase>().HasIndex(x => x.ITCode).IsUnique();
        }



        /// <summary>
        /// 数据初始化
        /// </summary>
        /// <param name="allModules"></param>
        /// <param name="IsSpa"></param>
        /// <returns>返回true表示需要进行初始化数据操作，返回false即数据库已经存在或不需要初始化数据</returns>
        public async override Task<bool> DataInit (object allModules, bool IsSpa)
        {
            bool rv = await Database.EnsureCreatedAsync();
            //判断是否存在初始数据
            bool emptydb = false;

            try
            {
                emptydb = !Set<FrameworkUserBase>().Any() && !Set<FrameworkUserRole>().Any() && !Set<FrameworkMenu>().Any();
            }
            catch { }

            if (emptydb == true)
            {
                var AllModules = allModules as List<SimpleModule>;
                var roles = new FrameworkRole[]
                {
                    new FrameworkRole{ ID = Guid.NewGuid(), RoleCode = "001", RoleName = CoreProgram.Callerlocalizer["Admin"]}
                };

                var adminRole = roles[0];
                if (Set<FrameworkMenu>().Any() == false)
                {
                    var systemManagement = GetFolderMenu("SystemManagement", new List<FrameworkRole> { adminRole });
                    var logList = IsSpa ? GetMenu2(AllModules, "ActionLog", new List<FrameworkRole> { adminRole }, 1) : GetMenu(AllModules, "_Admin", "ActionLog", "Index", new List<FrameworkRole> { adminRole }, 1);
                    var userList = IsSpa ? GetMenu2(AllModules, "FrameworkUser", new List<FrameworkRole> { adminRole }, 2) : GetMenu(AllModules, "_Admin", "FrameworkUser", "Index", new List<FrameworkRole> { adminRole }, 2);
                    var roleList = IsSpa ? GetMenu2(AllModules, "FrameworkRole", new List<FrameworkRole> { adminRole }, 3) : GetMenu(AllModules, "_Admin", "FrameworkRole", "Index", new List<FrameworkRole> { adminRole }, 3);
                    var groupList = IsSpa ? GetMenu2(AllModules, "FrameworkGroup", new List<FrameworkRole> { adminRole }, 4) : GetMenu(AllModules, "_Admin", "FrameworkGroup", "Index", new List<FrameworkRole> { adminRole }, 4);
                    var menuList = IsSpa ? GetMenu2(AllModules, "FrameworkMenu", new List<FrameworkRole> { adminRole }, 5) : GetMenu(AllModules, "_Admin", "FrameworkMenu", "Index", new List<FrameworkRole> { adminRole }, 5);
                    var dpList = IsSpa ? GetMenu2(AllModules, "DataPrivilege", new List<FrameworkRole> { adminRole }, 6) : GetMenu(AllModules, "_Admin", "DataPrivilege", "Index", new List<FrameworkRole> { adminRole }, 6);
                    if (logList != null)
                    {
                        var menus = new FrameworkMenu[] { logList, userList, roleList, groupList, menuList, dpList };
                        foreach (var item in menus)
                        {
                            if (item != null)
                            {
                                systemManagement.Children.Add(item);
                            }
                        }
                        Set<FrameworkMenu>().Add(systemManagement);

                        systemManagement.ICon = "settings-4";
                        logList.ICon = "ghost";
                        userList.ICon = "user";
                        roleList.ICon = "lock-2";
                        groupList.ICon = "group";
                        menuList.ICon = "menu";
                        dpList.ICon = "shield";
                    }

                }
                Set<FrameworkRole>().AddRange(roles);
                await SaveChangesAsync();
            }
            return rv;
        }

        private FrameworkMenu GetFolderMenu (string FolderText, List<FrameworkRole> allowedRoles, bool isShowOnMenu = true, bool isInherite = false)
        {
            FrameworkMenu menu = new FrameworkMenu
            {
                PageName = "MenuKey." + FolderText,
                Children = new List<FrameworkMenu>(),
                Privileges = new List<FunctionPrivilege>(),
                ShowOnMenu = isShowOnMenu,
                IsInside = true,
                FolderOnly = true,
                IsPublic = false,
                DisplayOrder = 1
            };

            if (allowedRoles != null)
            {
                foreach (var role in allowedRoles)
                {
                    menu.Privileges.Add(new FunctionPrivilege { RoleId = role.ID, Allowed = true });

                }
            }
            return menu;
        }

        private FrameworkMenu GetMenu (List<SimpleModule> allModules, string areaName, string controllerName, string actionName, List<FrameworkRole> allowedRoles, int displayOrder)
        {
            var acts = allModules.Where(x => x.ClassName == controllerName && (areaName == null || x.Area?.Prefix?.ToLower() == areaName.ToLower())).SelectMany(x => x.Actions).ToList();
            var act = acts.Where(x => x.MethodName == actionName).SingleOrDefault();
            var rest = acts.Where(x => x.MethodName != actionName && x.IgnorePrivillege == false).ToList();
            FrameworkMenu menu = GetMenuFromAction(act, true, allowedRoles, displayOrder);
            if (menu != null)
            {
                for (int i = 0; i < rest.Count; i++)
                {
                    if (rest[i] != null)
                    {
                        menu.Children.Add(GetMenuFromAction(rest[i], false, allowedRoles, (i + 1)));
                    }
                }
            }
            return menu;
        }

        private FrameworkMenu GetMenu2 (List<SimpleModule> allModules, string controllerName, List<FrameworkRole> allowedRoles, int displayOrder)
        {
            var acts = allModules.Where(x => x.FullName == $"KnifeZ.Virgo.Admin.Api,{controllerName}" && x.IsApi == true).SelectMany(x => x.Actions).ToList();
            var rest = acts.Where(x => x.IgnorePrivillege == false).ToList();
            SimpleAction act = null;
            if (acts.Count > 0)
            {
                act = acts[0];
            }
            FrameworkMenu menu = GetMenuFromAction(act, true, allowedRoles, displayOrder);
            if (menu != null)
            {
                menu.Url = "/" + acts[0].Module.ClassName.ToLower();
                menu.ModuleName = menu.ModuleName;
                menu.PageName = menu.ModuleName;
                menu.ActionName = "MainPage";
                menu.ClassName = acts[0].Module.FullName;
                menu.MethodName = null;
                for (int i = 0; i < rest.Count; i++)
                {
                    if (rest[i] != null)
                    {
                        menu.Children.Add(GetMenuFromAction(rest[i], false, allowedRoles, (i + 1)));
                    }
                }
            }
            return menu;
        }

        private FrameworkMenu GetMenuFromAction (SimpleAction act, bool isMainLink, List<FrameworkRole> allowedRoles, int displayOrder = 1)
        {
            if (act == null)
            {
                return null;
            }
            FrameworkMenu menu = new FrameworkMenu
            {
                //ActionId = act.ID,
                //ModuleId = act.ModuleId,
                ClassName = act.Module.FullName,
                MethodName = act.MethodName,
                Url = act.Url,
                Privileges = new List<FunctionPrivilege>(),
                ShowOnMenu = isMainLink,
                FolderOnly = false,
                Children = new List<FrameworkMenu>(),
                IsPublic = false,
                IsInside = true,
                DisplayOrder = displayOrder,
            };
            if (isMainLink)
            {
                menu.PageName = "MenuKey." + act.Module.ActionDes?.Description;
                menu.ModuleName = "MenuKey." + act.Module.ActionDes?.Description;
                menu.ActionName = act.ActionDes?.Description ?? act.ActionName;
                menu.MethodName = null;
            }
            else
            {
                menu.PageName = "MenuKey." + act.ActionDes?.Description;
                menu.ModuleName = "MenuKey." + act.ActionDes?.Description;
                menu.ActionName = act.ActionDes?.Description ?? act.ActionName;
            }
            if (allowedRoles != null)
            {
                foreach (var role in allowedRoles)
                {
                    menu.Privileges.Add(new FunctionPrivilege { RoleId = role.ID, Allowed = true });

                }
            }
            return menu;
        }

        private FrameworkMenu GetFolderMenu(string FolderText, List<FrameworkRole> allowedRoles, List<FrameworkUserBase> allowedUsers, bool isShowOnMenu = true, bool isInherite = false)
        {
            FrameworkMenu menu = new FrameworkMenu
            {
                PageName = "MenuKey." + FolderText,
                Children = new List<FrameworkMenu>(),
                Privileges = new List<FunctionPrivilege>(),
                ShowOnMenu = isShowOnMenu,
                IsInside = true,
                FolderOnly = true,
                IsPublic = false,
                DisplayOrder = 1
            };

            if (allowedRoles != null)
            {
                foreach (var role in allowedRoles)
                {
                    menu.Privileges.Add(new FunctionPrivilege { RoleId = role.ID, Allowed = true });

                }
            }
            if (allowedUsers != null)
            {
                foreach (var user in allowedUsers)
                {
                    menu.Privileges.Add(new FunctionPrivilege { UserId = user.ID, Allowed = true });
                }
            }

            return menu;
        }

    }

}
