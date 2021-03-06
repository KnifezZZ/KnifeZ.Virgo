using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using KnifeZ.Extensions;
using KnifeZ.Virgo.Core;
using KnifeZ.Virgo.Core.Extensions;

namespace KnifeZ.Virgo.Mvc.Admin.ViewModels.FrameworkUserVms
{
    public class FrameworkUserVM : BaseCRUDVM<FrameworkUser>
    {
        [JsonIgnore]
        public List<ComboSelectListItem> AllRoles { get; set; }
        [Display(Name = "Role")]
        public List<Guid> SelectedRolesIDs { get; set; }

        [JsonIgnore]
        public List<ComboSelectListItem> AllGroups { get; set; }
        [Display(Name = "Group")]
        public List<Guid> SelectedGroupIDs { get; set; }


        public FrameworkUserVM ()
        {
            SetInclude(x => x.UserRoles, x => x.UserGroups);
        }

        /// <summary>
        /// 验证重复字段
        /// </summary>
        /// <returns></returns>
        public override DuplicatedInfo<FrameworkUser> SetDuplicatedCheck ()
        {
            var rv = CreateFieldsInfo(SimpleField(x => x.ITCode));
            return rv;
        }

        protected override void InitVM ()
        {
            SelectedRolesIDs = Entity.UserRoles.Select(x => x.RoleId).ToList();
            AllRoles = DC.Set<FrameworkRole>().GetSelectListItems(KnifeVirgo, y => y.RoleName);
            SelectedGroupIDs = Entity.UserGroups.Select(x => x.GroupId).ToList();
            AllGroups = DC.Set<FrameworkGroup>().GetSelectListItems(KnifeVirgo, y => y.GroupName);
        }

        protected override void ReInitVM ()
        {
            AllRoles = DC.Set<FrameworkRole>().GetSelectListItems(KnifeVirgo, y => y.RoleName);
            AllGroups = DC.Set<FrameworkGroup>().GetSelectListItems(KnifeVirgo, y => y.GroupName);
        }

        public override async Task DoAddAsync ()
        {
            if (ControllerName.Contains("/api") == false)
            {
                Entity.UserRoles = new List<FrameworkUserRole>();
                Entity.UserGroups = new List<FrameworkUserGroup>();
                if (SelectedRolesIDs != null)
                {
                    foreach (var roleid in SelectedRolesIDs)
                    {
                        Entity.UserRoles.Add(new FrameworkUserRole { RoleId = roleid });
                    }
                }
                if (SelectedGroupIDs != null)
                {
                    foreach (var groupid in SelectedGroupIDs)
                    {
                        Entity.UserGroups.Add(new FrameworkUserGroup { GroupId = groupid });
                    }
                }
            }
            Entity.IsValid = true;
            Entity.Password = Utils.GetMD5String(Entity.Password);
            await base.DoAddAsync();
        }

        public override async Task DoEditAsync (bool updateAllFields = false)
        {
            FC.TryAdd("Entity.UserRoles", 0);
            FC.TryAdd("Entity.UserGroups", 0);
            Entity.UserRoles = new List<FrameworkUserRole>();
            Entity.UserGroups = new List<FrameworkUserGroup>();
            if (SelectedRolesIDs != null)
            {
                SelectedRolesIDs.ForEach(x => Entity.UserRoles.Add(new FrameworkUserRole { ID = Guid.NewGuid(), UserId = Entity.ID, RoleId = x }));
            }
            if (SelectedGroupIDs != null)
            {
                SelectedGroupIDs.ForEach(x => Entity.UserGroups.Add(new FrameworkUserGroup { ID = Guid.NewGuid(), UserId = Entity.ID, GroupId = x }));
            }
            await base.DoEditAsync(updateAllFields);
            await KnifeVirgo.RemoveUserCache(Entity.ID.ToString());
        }

        public override async Task DoDeleteAsync ()
        {
            await base.DoDeleteAsync();
            await KnifeVirgo.RemoveUserCache(Entity.ID.ToString());
        }

        public void ChangePassword ()
        {
            Entity.Password = Utils.GetMD5String(Entity.Password);
            DC.UpdateProperty(Entity, x => x.Password);
            DC.SaveChanges();
        }
    }
}
