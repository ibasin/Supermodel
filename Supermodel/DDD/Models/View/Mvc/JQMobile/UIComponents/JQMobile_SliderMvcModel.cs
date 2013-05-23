using System.Collections.Generic;
using System.Web.Mvc.Html;
using Supermodel.Mvc.Extensions;

// ReSharper disable CheckNamespace
namespace Supermodel.DDD.Models.View.Mvc.JQMobile
// ReSharper restore CheckNamespace
{
    public abstract partial class JQMobile
    {
        public class SliderMvcModel : TextBoxForIntMvcModel
        {
            public SliderMvcModel()
            {
                Min = 0;
                Max = 100;
            }
            
            public override System.Web.Mvc.MvcHtmlString EditorTemplate(System.Web.Mvc.HtmlHelper html, int screenOrderFrom = int.MinValue, int screenOrderTo = int.MaxValue, string markerAttribute = null)
            {
                var htmlAttributes = new Dictionary<string, object> { { "type", "range" }, {"Min", Min }, { "Max", Max }, { "data-highlight", true } };
                return html.TextBox("", GetStringValue(), htmlAttributes);
            }

            public override System.Web.Mvc.MvcHtmlString DisplayTemplate(System.Web.Mvc.HtmlHelper html, int screenOrderFrom = int.MinValue, int screenOrderTo = int.MaxValue, string markerAttribute = null)
            {
                return EditorTemplate(html, screenOrderFrom, screenOrderTo, markerAttribute).Supermodel().DisableAllControls();
            }

            public int Min { get; set; }
            public int Max { get; set; }
        }
    }
}
