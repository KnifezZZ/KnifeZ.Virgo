using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;
using KnifeZ.Virgo.Core;
using KnifeZ.Virgo.Core.Extensions;
using KnifeZ.Virgo.Core.Support.Json;
using KnifeZ.Virgo.Mvc.Admin.ViewModels.FrameworkRoleVMs;
using KnifeZ.Virgo.Mvc.Admin.ViewModels.FrameworkUserVms;
using KnifeZ.Extensions;

namespace KnifeZ.Virgo.Mvc.Admin.ViewModels.FrameworkMenuVMs
{
    public class FrameworkMenuVM : BaseCRUDVM<FrameworkMenu>
    {
        [JsonIgnore]
        public List<ComboSelectListItem> AllParents { get; set; }
        [JsonIgnore]
        public List<ComboSelectListItem> AllModules { get; set; }
        [JsonIgnore]
        public List<ComboSelectListItem> AllActions { get; set; }

        [Display(Name = "Action")]
        public List<string> SelectedActionIDs { get; set; }

        [Display(Name = "Module")]
        public string SelectedModule { get; set; }

        [Display(Name = "AllowedRole")]
        public List<Guid> SelectedRolesIDs { get; set; }

        [JsonIgnore]
        public FrameworkUserBaseListVM UserListVM { get; set; }
        [JsonIgnore]
        public FrameworkRoleListVM RoleListVM { get; set; }

        public List<ComboSelectListItem> IConSelectItems { get; set; }

        public FrameworkMenuVM()
        {
            UserListVM = new FrameworkUserBaseListVM();
            RoleListVM = new FrameworkRoleListVM();
            AllActions = new List<ComboSelectListItem>();
            AllModules = new List<ComboSelectListItem>();

            SelectedRolesIDs = new List<Guid>();
        }

        protected override void InitVM()
        {

            SelectedRolesIDs.AddRange(DC.Set<FunctionPrivilege>().Where(x => x.MenuItemId == Entity.ID && x.RoleId != null && x.Allowed == true).Select(x => x.RoleId.Value).ToList());

            var data = DC.Set<FrameworkMenu>().AsNoTracking().ToList();
            var topMenu = data.Where(x => x.ParentId == null).ToList().FlatTree(x => x.DisplayOrder);
            var pids = Entity.GetAllChildrenIDs(DC);
            AllParents = data.Where(x => x.ID != Entity.ID && !pids.Contains(x.ID) && x.FolderOnly == true).ToList().ToListItems(y => y.PageName, x => x.ID);

            foreach (var p in AllParents)
            {
                if (p.Text.StartsWith("MenuKey."))
                {
                        p.Text = Localizer[p.Text];
                }
            }

            var modules = KnifeVirgo.GlobaInfo.AllModule;

            var toRemove = new List<SimpleModule>();
            foreach (var item in modules)
            {
                if (item.IgnorePrivillege)
                {
                    toRemove.Add(item);
                }
            }
            var m = modules.ToList();
            toRemove.ForEach(x => m.Remove(x));
            AllModules = m.ToListItems(y => y.ModuleName, y => y.FullName);
            if (string.IsNullOrEmpty(SelectedModule) == false || (string.IsNullOrEmpty(Entity.Url) == false && Entity.IsInside == true))
            {
                if (string.IsNullOrEmpty(SelectedModule))
                {
                    SelectedModule = modules.Where(x => (x.FullName == Entity.ClassName)).FirstOrDefault()?.FullName;
                }
                var mm = modules.Where(x => x.FullName == SelectedModule).SelectMany(x => x.Actions).Where(x => x.MethodName != "Index" && x.IgnorePrivillege == false).ToList();
                AllActions = mm.ToListItems(y => y.ActionName, y => y.Url);
                if (SelectedActionIDs == null)
                {
                    SelectedActionIDs = DC.Set<FrameworkMenu>().Where(x => AllActions.Select(y => y.Value).Contains(x.Url) && x.IsInside == true && x.FolderOnly == false).Select(x => x.Url).ToList();
                }
            }
        }

        public override void Validate()
        {
            if (Entity.IsInside == true && Entity.FolderOnly == false)
            {
                var modules = KnifeVirgo.GlobaInfo.AllModule;
                var test = DC.Set<FrameworkMenu>().Where(x => x.ClassName == this.SelectedModule && (x.MethodName == null || x.MethodName == "Index") && x.ID != Entity.ID).FirstOrDefault();
                if (test != null)
                {
                    MSD.AddModelError(" error", Localizer["ModuleHasSet"]);
                }

            }
            base.Validate();
        }

        public override void DoEdit(bool updateAllFields = false)
        {
            List<Guid> guids = new List<Guid>();
            if (Entity.IsInside == false)
            {
                if (Entity.Url != null && Entity.Url != ""  && Entity.Url.StartsWith("/") == false)
                {
                        if (Entity.Url.ToLower().StartsWith("http://") == false && Entity.Url.ToLower().StartsWith("https://") == false)
                        {
                            Entity.Url = "http://" + Entity.Url;
                        }
                }
            }
            else
            {
                if (string.IsNullOrEmpty(SelectedModule) == true && Entity.FolderOnly == false)
                {
                    MSD.AddModelError("SelectedModule", Localizer["SelectModule"]);
                    return;
                }
                if (string.IsNullOrEmpty(SelectedModule) == false && Entity.FolderOnly == false)
                {
                    var modules = KnifeVirgo.GlobaInfo.AllModule;
                    List<SimpleAction> otherActions = null;
                    var mainAction = modules.Where(x => x.FullName == this.SelectedModule).SelectMany(x => x.Actions).Where(x => x.MethodName == "Index").FirstOrDefault();
                    if (mainAction == null && Entity.ShowOnMenu == true)
                    {
                        MSD.AddModelError("Entity.ModuleId", Localizer["NoIndexInModule"]);
                        return;
                    }
                    if (mainAction == null && Entity.ShowOnMenu == false)
                    {
                        var model = modules.Where(x => x.FullName == this.SelectedModule)
                                            .FirstOrDefault();
                        mainAction = new SimpleAction();
                        mainAction.Module = model;
                        mainAction.MethodName = null;
                    }

                    Entity.Url = mainAction.Url;
                    Entity.ModuleName = mainAction.Module.ModuleName;
                    Entity.ActionName = mainAction.ActionDes?.Description ?? mainAction.ActionName;
                    Entity.ClassName = mainAction.Module.FullName;
                    Entity.MethodName = null;

                    otherActions = modules.Where(x => x.FullName == this.SelectedModule)
                                            .SelectMany(x => x.Actions)
                                            .Where(x => x.MethodName != "Index")
                                            .ToList();
                    var actionsInDB = DC.Set<FrameworkMenu>().AsNoTracking().Where(x => x.ParentId == Entity.ID).ToList();
                    int order = 1;
                    Entity.Children = new List<FrameworkMenu>();
                    foreach (var action in otherActions)
                    {
                        if (SelectedActionIDs != null && SelectedActionIDs.Contains(action.Url))
                        {
                            Guid aid = action.ID;
                            var adb = actionsInDB.Where(x => x.Url.ToLower() == action.Url.ToLower()).FirstOrDefault();
                            if (adb != null)
                            {
                                aid = adb.ID;
                            }
                            else
                            {
                                guids.Add(aid);
                            }
                            var menu = new FrameworkMenu();
                            menu.FolderOnly = false;
                            menu.IsPublic = false;
                            menu.Parent = Entity;
                            menu.ShowOnMenu = false;
                            menu.DisplayOrder = order++;
                            menu.Privileges = new List<FunctionPrivilege>();
                            menu.IsInside = true;
                            menu.DomainId = Entity.DomainId;
                            menu.PageName = action.ActionDes?.Description ?? action.ActionName;
                            menu.ModuleName = action.Module.ModuleName;
                            menu.ActionName = action.ActionDes?.Description ?? action.ActionName;
                            menu.ClassName = action.Module.FullName;
                            menu.MethodName = action.MethodName;
                            menu.Url = action.Url;
                            menu.ID = aid;
                            Entity.Children.Add(menu);
                        }
                    }
                }
                else
                {
                    Entity.Children = null;
                    Entity.Url = null;
                }
            }
            if (FC.ContainsKey("Entity.Children") == false)
            {
                FC.Add("Entity.Children", 0);
            }
            base.DoEdit();
            AddPrivilege(guids);
        }

        public override void DoAdd()
        {
            if (Entity.IsInside == false)
            {
                if (Entity.Url != null && Entity.Url != "" && Entity.Url.StartsWith("/") == false)
                {
                        if (Entity.Url.ToLower().StartsWith("http://") == false && Entity.Url.ToLower().StartsWith("https://") == false)
                        {
                            Entity.Url = "http://" + Entity.Url;
                        }
                }
            }
            else
            {

                if (string.IsNullOrEmpty(SelectedModule) == true && Entity.FolderOnly == false)
                {
                    MSD.AddModelError("SelectedModule", Localizer["SelectModule"]);
                    return;
                }
                if (string.IsNullOrEmpty(SelectedModule) == false && Entity.FolderOnly == false)
                {
                    var modules = KnifeVirgo.GlobaInfo.AllModule;
                    List<SimpleAction> otherActions = null;
                    var mainAction = modules.Where(x => x.FullName == this.SelectedModule).SelectMany(x => x.Actions).Where(x => x.MethodName == "Index").FirstOrDefault();
                    if (mainAction == null && Entity.ShowOnMenu == true)
                    {
                        MSD.AddModelError("Entity.ModuleId", Localizer["NoIndexInModule"]);
                        return;
                    }
                    if (mainAction == null && Entity.ShowOnMenu == false)
                    {
                        var model = modules.Where(x => x.FullName == this.SelectedModule).FirstOrDefault();
                        mainAction = new SimpleAction();
                        mainAction.Module = model;
                        mainAction.MethodName = null;
                    }
                    Entity.Url = mainAction.Url;
                    Entity.ModuleName = mainAction.Module.ModuleName;
                    Entity.ActionName = mainAction.ActionDes?.Description ?? mainAction.ActionName;
                    Entity.ClassName = mainAction.Module.FullName;
                    Entity.MethodName = null;
                    Entity.Children = new List<FrameworkMenu>();

                    otherActions = modules.Where(x => x.FullName == this.SelectedModule).SelectMany(x => x.Actions).Where(x => x.MethodName != "Index").ToList();
                    int order = 1;
                    foreach (var action in otherActions)
                    {
                        if (SelectedActionIDs?.Contains(action.Url) == true)
                        {
                            FrameworkMenu menu = new FrameworkMenu();
                            menu.FolderOnly = false;
                            menu.IsPublic = false;
                            menu.Parent = Entity;
                            menu.ShowOnMenu = false;
                            menu.DisplayOrder = order++;
                            menu.Privileges = new List<FunctionPrivilege>();
                            menu.IsInside = true;
                            menu.DomainId = Entity.DomainId;
                            menu.PageName = action.ActionDes?.Description ?? action.ActionName;
                            menu.ModuleName = action.Module.ModuleName;
                            menu.ActionName = action.ActionDes?.Description ?? action.ActionName;
                            menu.ClassName = action.Module.FullName;
                            menu.MethodName = action.MethodName;
                            menu.Url = action.Url;
                            Entity.Children.Add(menu);
                        }
                    }
                }

                else
                {
                    Entity.Children = null;
                    Entity.Url = null;
                }

            }
            base.DoAdd();
            List<Guid> guids = new List<Guid>();
            guids.Add(Entity.ID);
            if (Entity.Children != null)
            {
                guids.AddRange(Entity.Children?.Select(x => x.ID).ToList());
            }
            AddPrivilege(guids);
        }

        public void AddPrivilege(List<Guid> menuids)
        {
            //var oldIDs = DC.Set<FunctionPrivilege>().Where(x => menuids.Contains(x.MenuItemId)).Select(x => x.ID).ToList();
            var admin = DC.Set<FrameworkRole>().Where(x => x.RoleCode == "001").FirstOrDefault();
            //foreach (var oldid in oldIDs)
            //{
            //    try
            //    {
            //        FunctionPrivilege fp = new FunctionPrivilege { ID = oldid };
            //        DC.Set<FunctionPrivilege>().Attach(fp);
            //        DC.DeleteEntity(fp);
            //    }
            //    catch { }
            //}
            if (admin != null && SelectedRolesIDs.Contains(admin.ID) == false)
            {
                SelectedRolesIDs.Add(admin.ID);
            }
            foreach (var menuid in menuids)
            {

                if (SelectedRolesIDs != null)
                {
                    foreach (var id in SelectedRolesIDs)
                    {
                        FunctionPrivilege fp = new FunctionPrivilege();
                        fp.MenuItemId = menuid;
                        fp.RoleId = id;
                        fp.UserId = null;
                        fp.Allowed = true;
                        DC.Set<FunctionPrivilege>().Add(fp);
                    }
                }
            }

            DC.SaveChanges();
        }


        public override void DoDelete()
        {
            try
            {
                //级联删除所有子集
                DC.CascadeDelete(Entity);
                DC.SaveChanges();
            }
            catch
            { }
        }
    }
}
