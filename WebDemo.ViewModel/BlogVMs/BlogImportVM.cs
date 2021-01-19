using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

using KnifeZ.Virgo.Core;
using KnifeZ.Virgo.Core.Extensions;

using WebDemo.Model;


namespace WebDemo.ViewModel.BlogVMs
{
    public partial class BlogTemplateVM : BaseTemplateVM
    {
        [Display(Name = "标题")]
        public ExcelPropety Title_Excel = ExcelPropety.CreateProperty<Blog>(x => x.Title);
        [Display(Name = "摘要")]
        public ExcelPropety Description_Excel = ExcelPropety.CreateProperty<Blog>(x => x.Description);
        [Display(Name = "关键字")]
        public ExcelPropety Keywords_Excel = ExcelPropety.CreateProperty<Blog>(x => x.Keywords);
        [Display(Name = "正文")]
        public ExcelPropety BodyText_Excel = ExcelPropety.CreateProperty<Blog>(x => x.BodyText);
        [Display(Name = "博客类别")]
        public ExcelPropety BlogCategory_Excel = ExcelPropety.CreateProperty<Blog>(x => x.BlogCategoryId);
        [Display(Name = "是否单页")]
        public ExcelPropety IsSinglePage_Excel = ExcelPropety.CreateProperty<Blog>(x => x.IsSinglePage);
        [Display(Name = "访问量")]
        public ExcelPropety VisitCount_Excel = ExcelPropety.CreateProperty<Blog>(x => x.VisitCount);
        [Display(Name = "状态")]
        public ExcelPropety Status_Excel = ExcelPropety.CreateProperty<Blog>(x => x.Status);
        [Display(Name = "静态地址")]
        public ExcelPropety Url_Excel = ExcelPropety.CreateProperty<Blog>(x => x.Url);

	    protected override void InitVM()
        {
            BlogCategory_Excel.DataType = ColumnDataType.ComboBox;
            BlogCategory_Excel.ListItems = DC.Set<BlogCategory>().GetSelectListItems(KnifeVirgo, y => y.Name);
        }

    }

    public class BlogImportVM : BaseImportVM<BlogTemplateVM, Blog>
    {

    }

}
