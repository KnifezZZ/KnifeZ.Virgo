using System.Collections.Generic;
using System.Linq;
using KnifeZ.Virgo.Core;
using KnifeZ.Virgo.Core.Extensions;

namespace KnifeZ.Virgo.Mvc.Admin.ViewModels.FrameworkMenuVMs
{
    public class FrameworkMenuListVM2 : BasePagedListVM<FrameworkMenu_ListView, FrameworkMenuSearcher>
    {
        /// <summary>
        /// 页面显示按钮方案，如果需要增加Action类型则将按钮添加到此类中
        /// </summary>
        public FrameworkMenuListVM2()
        {
            this.NeedPage = false;
        }

        protected override IEnumerable<IGridColumn<FrameworkMenu_ListView>> InitGridHeader()
        {
            List<GridColumn<FrameworkMenu_ListView>> rv = new List<GridColumn<FrameworkMenu_ListView>>();
            rv.AddRange(new GridColumn<FrameworkMenu_ListView>[] {
                        this.MakeGridHeader(x => x.PageName),
                        this.MakeGridHeader(x => x.ModuleName),
                        this.MakeGridHeader(x => x.ShowOnMenu),
                        this.MakeGridHeader(x => x.FolderOnly),
                        this.MakeGridHeader(x => x.IsPublic),
                        this.MakeGridHeader(x => x.DisplayOrder),
                        this.MakeGridHeader(x => x.ICon),
                        this.MakeGridHeader(x => x.Children),
                        this.MakeGridHeader(x=>x.ParentID).SetHide()
                    });
            return rv;
        }

        /// <summary>
        /// 查询结果
        /// </summary>
        public override IOrderedQueryable<FrameworkMenu_ListView> GetSearchQuery()
        {
            var data = DC.Set<FrameworkMenu>().ToList();
            var topdata = data.Where(x => x.ParentId == null).ToList().FlatTree(x => x.DisplayOrder).Where(x => x.IsInside == false || x.FolderOnly == true || string.IsNullOrEmpty(x.MethodName)).ToList();
            foreach (var item in topdata)
            {
                if (item.PageName?.StartsWith("MenuKey.") == true)
                {
                    item.PageName = Localizer[item.PageName];
                }
                if (item.ModuleName?.StartsWith("MenuKey.") == true)
                {
                    item.ModuleName = Localizer[item.ModuleName];
                }
            }
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
                ParentID = x.ParentId,
                ICon = x.ICon,
                HasChild = (x.Children != null && x.Children.Count > 0)
            }).OrderBy(x => x.ExtraOrder).ToList();

            return data2.AsQueryable() as IOrderedQueryable<FrameworkMenu_ListView>;
        }



    }

}
