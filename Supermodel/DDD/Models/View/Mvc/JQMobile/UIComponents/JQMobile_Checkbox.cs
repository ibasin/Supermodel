using System;
using System.Web.Mvc;
using System.Web.Mvc.Html;

// ReSharper disable CheckNamespace
namespace Supermodel.DDD.Models.View.Mvc.JQMobile
// ReSharper restore CheckNamespace
{
    public abstract partial class JQMobile
    {
        public class Checkbox : ToggleSwitchMvcModel
        {
            public override MvcHtmlString EditorTemplate(HtmlHelper html, int screenOrderFrom = Int32.MinValue, int screenOrderTo = Int32.MaxValue, string markerAttribute = null)
            {
                return html.CheckBox("", Value);
            }
        }
    }
}
