using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using KnifeZ.Extensions;
using KnifeZ.Virgo.Core;
using KnifeZ.Virgo.Core.Extensions;

namespace KnifeZ.Virgo.Mvc.Admin.ViewModels.FrameworkMenuVMs
{
    public class FrameworkMenuListVM : BasePagedListVM<FrameworkMenu_ListView, FrameworkMenuSearcher>
    {
        /// <summary>
        /// 页面显示按钮方案，如果需要增加Action类型则将按钮添加到此类中
        /// </summary>
        public FrameworkMenuListVM()
        {
            this.NeedPage = false;
        }

        protected override IEnumerable<IGridColumn<FrameworkMenu_ListView>> InitGridHeader()
        {
            List<GridColumn<FrameworkMenu_ListView>> rv = new List<GridColumn<FrameworkMenu_ListView>>();
            switch (SearcherMode)
            {
                case ListVMSearchModeEnum.Batch:
                    rv.AddRange(new GridColumn<FrameworkMenu_ListView>[] {
                        this.MakeGridHeader(x => x.PageName),
                        this.MakeGridHeader(x => x.ModuleName),
                        this.MakeGridHeader(x => x.ActionName),
                    });
                    break;
                case ListVMSearchModeEnum.Custom1:
                    rv.AddRange(new GridColumn<FrameworkMenu_ListView>[] {
                        this.MakeGridHeader(x => x.PageName),
                    });
                    break;
                case ListVMSearchModeEnum.Custom2:
                    rv.AddRange(new GridColumn<FrameworkMenu_ListView>[] {
                        this.MakeGridHeader(x => x.PageName),
                         this.MakeGridHeader(x => x.ParentId),
                   });
                    break;
                default:
                    rv.AddRange(new GridColumn<FrameworkMenu_ListView>[] {
                        this.MakeGridHeader(x => x.PageName),
                        this.MakeGridHeader(x => x.ModuleName),
                        this.MakeGridHeader(x => x.ShowOnMenu),
                        this.MakeGridHeader(x => x.FolderOnly),
                        this.MakeGridHeader(x => x.IsPublic),
                        this.MakeGridHeader(x => x.DisplayOrder),
                        this.MakeGridHeader(x => x.ICon),
                    });
                    break;
            }
            return rv;
        }


        /// <summary>
        /// 查询结果
        /// </summary>
        public override IOrderedQueryable<FrameworkMenu_ListView> GetSearchQuery()
        {

            var data = DC.Set<FrameworkMenu>().ToList();
            var topdata = data.Where(x => x.ParentId == null).ToList().FlatTree(x => x.DisplayOrder).Where(x => x.IsInside == false || x.FolderOnly == true || x.Url.EndsWith("/Index") || x.MethodName == null).ToList();
            foreach (var item in topdata)
            {
                if (item.PageName?.StartsWith("MenuKey.") == true)
                {
                        item.PageName =Localizer[item.PageName];
                }
                if (item.ModuleName?.StartsWith("MenuKey.") == true)
                {
                        item.ModuleName = Localizer[item.ModuleName];
                }

            }
            topdata.ForEach((x) => { int l = x.GetLevel(); for (int i = 0; i < l; i++) { x.PageName = "&nbsp;&nbsp;&nbsp;&nbsp;" + x.PageName; } });
            if (SearcherMode == ListVMSearchModeEnum.Custom2)
            {
                var pris = DC.Set<FunctionPrivilege>()
                                .Where(x => x.RoleId == Searcher.RoleID)
                                .ToList();
                var allowed = pris.Where(x => x.Allowed == true).Select(x => x.MenuItemId).ToList();
                var denied = pris.Where(x => x.Allowed == false).Select(x => x.MenuItemId).ToList();
                int order = 0;
                var data2 = topdata.Select(x => new FrameworkMenu_ListView
                {
                    ID = x.ID,
                    PageName = x.PageName,
                    ModuleName = x.ModuleName,
                    ActionName = x.ActionName,
                    ShowOnMenu = x.ShowOnMenu,
                    FolderOnly = x.FolderOnly,
                    IsPublic = x.IsPublic,
                    DisplayOrder = x.DisplayOrder,
                    Children = x.Children?.Select(y => new FrameworkMenu_ListView
                    {
                        ID = y.ID,
                        Allowed = allowed.Contains(y.ID),
                        ActionName = y.ActionName
                    }),
                    ExtraOrder = order++,
                    ParentId = x.ParentId,
                    Parent = x.Parent,
                    IsInside = x.IsInside,
                    HasChild = x.Children != null && x.Children.Count > 0,
                    Allowed = allowed.Contains(x.ID),
                    Denied = denied.Contains(x.ID)
                }).OrderBy(x => x.ExtraOrder);
                return data2.AsQueryable() as IOrderedQueryable<FrameworkMenu_ListView>;
            }
            else
            {
                int order = 0;
                var data2 = topdata.Select(x => new FrameworkMenu_ListView
                {
                    ID = x.ID,
                    PageName = x.PageName,
                    ModuleName = x.ModuleName,
                    ActionName = x.ActionName,
                    ShowOnMenu = x.ShowOnMenu,
                    FolderOnly = x.FolderOnly,
                    IsPublic = x.IsPublic,
                    DisplayOrder = x.DisplayOrder,
                    ExtraOrder = order++,

                    ParentId = x.ParentId,
                    ICon = x.ICon,
                    HasChild = x.Children != null && x.Children.Count > 0
                }).OrderBy(x => x.ExtraOrder);

                return data2.AsQueryable() as IOrderedQueryable<FrameworkMenu_ListView>;

            }
        }

    }

    /// <summary>
    /// 如果需要显示树类型的列表需要继承ITreeData`T`接口，并实现Children,Parent,ParentID属性
    /// </summary>
    public class FrameworkMenu_ListView : BasePoco
    {
        [Display(Name = "PageName")]
        public string PageName { get; set; }

        [Display(Name = "ModuleName")]
        public string ModuleName { get; set; }

        [Display(Name = "ActionName")]
        public string ActionName { get; set; }

        [Display(Name = "ShowOnMenu")]
        public bool? ShowOnMenu { get; set; }

        [Display(Name = "FolderOnly")]
        public bool? FolderOnly { get; set; }

        [Display(Name = "IsPublic")]
        public bool? IsPublic { get; set; }

        [Display(Name = "DisplayOrder")]
        public int? DisplayOrder { get; set; }

        [Display(Name = "ICon")]
        public string ICon { get; set; }

        public bool Allowed { get; set; }

        public bool Denied { get; set; }

        public bool HasChild { get; set; }

        public string IconClass { get; set; }

        public IEnumerable<FrameworkMenu_ListView> Children { get; set; }

        public FrameworkMenu Parent { get; set; }

        public Guid? ParentId { get; set; }

        public int ExtraOrder { get; set; }

        public bool? IsInside { get; set; }
    }
}
