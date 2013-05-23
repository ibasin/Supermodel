using System;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using ReflectionMapper;

namespace Supermodel.DDD.Models.View.Mvc.UIComponents
{
    public class ListBoxMvcModelUsing<MvcModelT> : MultiSelectMvcModelBase<MvcModelT> where MvcModelT : MvcModelForEntityCore
    {
        public override MvcHtmlString EditorTemplate(HtmlHelper html, int screenOrderFrom = int.MinValue, int screenOrderTo = int.MaxValue, string markerAttribute = null)
        {
            if (html.ViewData.Model == null) throw new NullReferenceException(ReflectionHelper.GetCurrentContext() + " is called for a model that is null");
            if (!(html.ViewData.Model is MultiSelectMvcModelCore)) throw new InvalidCastException(ReflectionHelper.GetCurrentContext() + " is called for a model of type diffrent from ListBoxFormModel.");

            var multiSelect = (MultiSelectMvcModelCore)html.ViewData.Model;
            var fullName = html.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName("");
            var result = html.ListBox("", multiSelect.GetSelectListItemList()) + string.Format("<input name='{0}' type='hidden' value='' />", fullName);
            return MvcHtmlString.Create(result);
        }
    }
}