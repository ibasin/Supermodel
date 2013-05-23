using System;
using System.Collections.Generic;
using System.Text;
using System.Web.Mvc;
using ReflectionMapper;
using Supermodel.Mvc.Extensions;

namespace Supermodel.DDD.Models.View.Mvc.UIComponents
{
    public class CheckboxesListMvcModelUsing<MvcModelT> : MultiSelectMvcModelBase<MvcModelT> where MvcModelT : MvcModelForEntityCore
    {
        public string CssClass { get; set; }
        
        public override MvcHtmlString EditorTemplate(HtmlHelper html, int screenOrderFrom = int.MinValue, int screenOrderTo = int.MaxValue, string markerAttribute = null)
        {
            if (html.ViewData.Model == null) throw new NullReferenceException(ReflectionHelper.GetCurrentContext() + " is called for a model that is null");
            if (!(html.ViewData.Model is MultiSelectMvcModelCore)) throw new InvalidCastException(ReflectionHelper.GetCurrentContext() + " is called for a model of type diffrent from CheckboxesListFormModel.");

            var multiSelect = (MultiSelectMvcModelCore)html.ViewData.Model;
            return RenderCheckBoxesList(html, "", multiSelect.GetSelectListItemList(), CssClass);
        }

        public virtual MvcHtmlString RenderCheckBoxesList(HtmlHelper html, string name, IEnumerable<SelectListItem> selectList, string cssClass)
        {
            var fullName = html.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(name);
            if (String.IsNullOrEmpty(fullName)) throw new ArgumentException(@"Value cannot be null or empty.", "name");

            var result = new StringBuilder();
            if (cssClass == null) result.AppendLine("<ul>");
            else result.AppendLine("<ul class='" + cssClass + "'>");
            foreach (var item in selectList)
            {
                result.AppendLine("<li>");

                result.AppendLine(item.Selected ?
                    string.Format("<input name='{0}' type='checkbox' value='{1}' checked />", fullName, item.Value) :
                    string.Format("<input name='{0}' type='checkbox' value='{1}' />", fullName, item.Value));

                result.AppendLine("<label>" + item.Text + "</label>");
                result.AppendLine("</li>");
            }
            result.AppendLine("</ul>");
            result.AppendLine(string.Format("<input name='{0}' type='hidden' value='' />", fullName));
            return MvcHtmlString.Create(result.ToString());
        }
    }
}