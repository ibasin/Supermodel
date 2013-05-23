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
        public class CheckboxesListHorizontalMvcModel<MvcModelT> : MultiSelectMvcModelBase<MvcModelT> where MvcModelT : MvcModelForEntityCore
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
                var result = CheckboxesListVerticalMvcModel<MvcModelT>.RenderCheckBoxesList(html, name, selectList).ToString().Replace("<fieldset data-role='controlgroup'>", "<fieldset data-role='controlgroup' data-type='horizontal'>");
                return MvcHtmlString.Create(result);
            }
        }
    }
}