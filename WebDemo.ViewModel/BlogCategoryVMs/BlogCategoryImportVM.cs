using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

using KnifeZ.Virgo.Core;
using KnifeZ.Virgo.Core.Extensions;

using WebDemo.Model;


namespace WebDemo.ViewModel.BlogCategoryVMs
{
    public partial class BlogCategoryTemplateVM : BaseTemplateVM
    {
        [Display(Name = "类别名称")]
        public ExcelPropety Name_Excel = ExcelPropety.CreateProperty<BlogCategory>(x => x.Name);
        [Display(Name = "类别描述")]
        public ExcelPropety Description_Excel = ExcelPropety.CreateProperty<BlogCategory>(x => x.Description);
        [Display(Name = "图标")]
        public ExcelPropety Icon_Excel = ExcelPropety.CreateProperty<BlogCategory>(x => x.Icon);
        [Display(Name = "静态地址")]
        public ExcelPropety Url_Excel = ExcelPropety.CreateProperty<BlogCategory>(x => x.Url);
        [Display(Name = "排序")]
        public ExcelPropety Sort_Excel = ExcelPropety.CreateProperty<BlogCategory>(x => x.Sort);
        [Display(Name = "ParentFolder")]
        public ExcelPropety Parent_Excel = ExcelPropety.CreateProperty<BlogCategory>(x => x.ParentId);

	    protected override void InitVM()
        {
            Parent_Excel.DataType = ColumnDataType.ComboBox;
            Parent_Excel.ListItems = DC.Set<BlogCategory>().GetSelectListItems(KnifeVirgo, y => y.Name);
        }

    }

    public class BlogCategoryImportVM : BaseImportVM<BlogCategoryTemplateVM, BlogCategory>
    {

    }

}
