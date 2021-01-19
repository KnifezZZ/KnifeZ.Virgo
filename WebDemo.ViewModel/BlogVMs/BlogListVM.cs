using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using KnifeZ.Virgo.Core;
using KnifeZ.Virgo.Core.Extensions;

using WebDemo.Model;


namespace WebDemo.ViewModel.BlogVMs
{
    public partial class BlogListVM : BasePagedListVM<Blog_View, BlogSearcher>
    {
    
        public BlogListVM ()
        {
            this.NeedPage = Searcher.Limit > 0;
        }

        protected override IEnumerable<IGridColumn<Blog_View>> InitGridHeader()
        {
            return new List<GridColumn<Blog_View>>{
                this.MakeGridHeader(x => x.Title),
                this.MakeGridHeader(x => x.Description),
                this.MakeGridHeader(x => x.Keywords),
                this.MakeGridHeader(x => x.BlogCategory_Name),
                this.MakeGridHeader(x => x.BlogCategoryId),
                this.MakeGridHeader(x => x.BlogClassification_Name),
                this.MakeGridHeader(x => x.VisitCount),
                this.MakeGridHeader(x => x.Status),
                this.MakeGridHeader(x => x.Url),
            };
        }

        public override IOrderedQueryable<Blog_View> GetSearchQuery()
        {
            var query = DC.Set<Blog>()
                .CheckContain(Searcher.Title, x=>x.Title)
                .CheckEqual(Searcher.Status, x=>x.Status)
                .Select(x => new Blog_View
                {
				    ID = x.ID,
                    Title = x.Title,
                    Description = x.Description,
                    Keywords = x.Keywords,
                    BlogCategory_Name = x.BlogCategory.Name,
                    BlogClassification_Name = x.BlogClassificationMiddle.Select(y=>y.BlogClassification.Name).ToSepratedString(null,","), 
                    VisitCount = x.VisitCount,
                    Status = x.Status,
                    Url = x.Url,
                })
                .OrderBy(x => x.ID);
            return query;
        }

    }

    public class Blog_View : Blog{
        [Display(Name = "类别名称")]
        public String BlogCategory_Name { get; set; }
        [Display(Name = "类别名称")]
        public String BlogClassification_Name { get; set; }

    }
}


