using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KnifeZ.Virgo.Core;
using KnifeZ.Virgo.Core.Attributes;
using KnifeZ.Virgo.Core.Extensions;
using KnifeZ.Virgo.Mvc.Model;

namespace KnifeZ.Virgo.Mvc
{
    public class VGenCodeVM:BaseVM
    {
        public VGenCodeModel CodeModel { get; set; }
        public VGenCodeVM ()
        {
        }
        public VGenCodeVM (VGenCodeModel _model)
        {
            CodeModel = _model;
        }

        public string GenTemplates ()
        {
            return "success";
        }
        public IOrderedQueryable<CodeGenListView> GetFieldInfos (string modelFullName)
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

            return lv.AsQueryable().OrderBy(x => x.FieldName);
        }
        private string GetRelatedNamespace (List<FieldInfo> pros, string s)
        {
            string otherns = @"";
            Type modelType = Type.GetType(CodeModel.ModelName);
            foreach (var pro in pros)
            {
                Type proType = null;

                if (string.IsNullOrEmpty(pro.RelatedField))
                {
                    proType = modelType.GetProperties().Where(x => x.Name == pro.FieldName).Select(x => x.PropertyType).FirstOrDefault();
                }
                else
                {
                    proType = Type.GetType(pro.RelatedField);
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

    public class VGenCodeModel { 
    
        public string ModelName { get; set; }
        public List<FieldInfo> FieldInfos { get; set; }

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

        [Display(Name = "IsImportField")]
        public bool IsImportField { get; set; }

        [Display(Name = "IsBatchField")]
        public bool IsBatchField { get; set; }

        public int Index { get; set; }

        [Display(Name = "LinkedType")]
        public string LinkedType { get; set; }

    }

}
