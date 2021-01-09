using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using KnifeZ.Virgo.Core;
using KnifeZ.Virgo.Core.Extensions;

using WebDemo.Model;


namespace WebDemo.ViewModel.BlogCategoryVMs
{
    public partial class BlogCategoryListVM : BasePagedListVM<BlogCategory_View, BlogCategorySearcher>
    {

        protected override IEnumerable<IGridColumn<BlogCategory_View>> InitGridHeader()
        {
            return new List<GridColumn<BlogCategory_View>>{
                this.MakeGridHeader(x => x.Description),
                this.MakeGridHeader(x => x.Icon),
                this.MakeGridHeader(x => x.Name),
                this.MakeGridHeader(x => x.BlogCategory_Name),
                this.MakeGridHeader(x => x.Sort),
                this.MakeGridHeader(x => x.Url),
            };
        }

        public override IOrderedQueryable<BlogCategory_View> GetSearchQuery()
        {
            var query = DC.Set<BlogCategory>()
                .CheckContain(Searcher.Name, x=>x.Name)
                .CheckContain(Searcher.Url, x=>x.Url)
                .Select(x => new BlogCategory_View
                {
				    ID = x.ID,
                    Description = x.Description,
                    Icon = x.Icon,
                    Name = x.Name,
                    BlogCategory_Name = x.Parent.Name,
                    Sort = x.Sort,
                    Url = x.Url,
                })
                .OrderBy(x => x.ID);
            return query;
        }

    }

    public class BlogCategory_View : BlogCategory{
        [Display(Name = "类别名称")]
        public String BlogCategory_Name { get; set; }

    }
}


