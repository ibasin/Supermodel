using System.Web.Mvc;

namespace Supermodel.DDD.Models.View.Mvc
{
    public static class SupermodelSettings
    {
        public static class Scaffolding
        {
            public static MvcHtmlString MakeIdAndClassAttribues(string id, string cssClass)
            {
                return MvcHtmlString.Create(MakeIdAttribue(id).ToString() + MakeClassAttribue(cssClass));
            }

            public static MvcHtmlString MakeIdAttribue(string id)
            {
                return MvcHtmlString.Create(string.IsNullOrEmpty(id) ? "" : string.Format(" Id='{0}' ", id));
            }

            public static MvcHtmlString MakeClassAttribue(string cssClass)
            {
                return MvcHtmlString.Create(string.IsNullOrEmpty(cssClass) ? "" : string.Format(" class='{0}' ", cssClass));
            }

            //EditorTemplate
            public static string FormTableId { get; set; }
            public static string FormTableCssClass { get; set; }

            //CRUDEdit
            public static string DisplayCssClass { get; set; }
            
            public static string LabelCssClass { get; set; }
            
            public static string RequiredAsteriskCssClass { get; set; }

            public static string SaveButtonId { get; set; }
            public static string SaveButtonCssClass { get; set; }

            public static string BackButtonId { get; set; }
            public static string BackButtonCssClass { get; set; }

            //CRUDList
            public static string CRUDListTopDivId { get; set; }
            public static string CRUDListTopDivCssClass { get; set; }

            public static string CRUDListTableId { get; set; }
            public static string CRUDListTableCssClass { get; set; }

            public static string CRUDListEditCssClass { get; set; }
            public static string CRUDListDeleteCssClass { get; set; }
            public static string CRUDListAddNewCssClass { get; set; }
        }
    }
}
