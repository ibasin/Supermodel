using System;
using System.Collections.Generic;
using System.Text;
using System.Web.Mvc;
using ReflectionMapper;
using Supermodel.DDD.Models.View.Mvc.UIComponents;

// ReSharper disable CheckNamespace
namespace Supermodel.DDD.Models.View.Mvc.JQMobile
// ReSharper restore CheckNamespace
{
    public abstract partial class JQMobile
    {
        public class CheckboxesListVerticalMvcModel<MvcModelT> : MultiSelectMvcModelBase<MvcModelT> where MvcModelT : MvcModelForEntityCore
        {
            public override MvcHtmlString EditorTemplate(HtmlHelper html, int screenOrderFrom = int.MinValue, int screenOrderTo = int.MaxValue, string markerAttribute = null)
            {
                if (html.ViewData.Model == null) throw new NullReferenceException(ReflectionHelper.GetCurrentContext() + " is called for a model that is null");
                if (!(html.ViewData.Model is MultiSelectMvcModelCore)) throw new InvalidCastException(ReflectionHelper.GetCurrentContext() + " is called for a model of type diffrent from CheckboxesListFormModel.");

                var multiSelect = (MultiSelectMvcModelCore)html.ViewData.Model;
                return RenderCheckBoxesList(html, "", multiSelect.GetSelectListItemList());
            }

            public static MvcHtmlString RenderCheckBoxesList(HtmlHelper html, string name, IEnumerable<SelectListItem> selectList)
            {
                var fullName = html.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(name);
                if (String.IsNullOrEmpty(fullName)) throw new ArgumentException(@"Value cannot be null or empty.", "name");

                var result = new StringBuilder();
                result.AppendLine("<fieldset data-role='controlgroup'>");
                result.AppendLine("<legend></legend>");
                var index = 1;
                foreach (var item in selectList)
                {
                    var id = html.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName("") + "__" + index++;
                    result.AppendLine(item.Selected ?
                        string.Format("<input name='{0}' type='checkbox' value='{1}' id='{2}' checked />", fullName, item.Value, id) :
                        string.Format("<input name='{0}' type='checkbox' value='{1}' id='{2}' />", fullName, item.Value, id));
                    result.AppendFormat("<label for='{0}'>{1}</label>", id, item.Text);
                }
                result.AppendLine("</fieldset>");
                result.AppendLine(string.Format("<input name='{0}' type='hidden' value='' />", fullName));
                return MvcHtmlString.Create(result.ToString());
            }
        }
    }
}
