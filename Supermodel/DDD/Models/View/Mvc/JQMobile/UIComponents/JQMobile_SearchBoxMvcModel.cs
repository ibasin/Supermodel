using System.Collections.Generic;
using System.Web.Mvc.Html;

// ReSharper disable CheckNamespace
namespace Supermodel.DDD.Models.View.Mvc.JQMobile
// ReSharper restore CheckNamespace
{
    public abstract partial class JQMobile
    {
        public class SearchBoxMvcModel : TextBoxForStringMvcModel
        {
            public override System.Web.Mvc.MvcHtmlString EditorTemplate(System.Web.Mvc.HtmlHelper html, int screenOrderFrom = int.MinValue, int screenOrderTo = int.MaxValue, string markerAttribute = null)
            {
                var htmlAttributes = new Dictionary<string, object> { { "type", "search" } };
                if (Placeholder != null) htmlAttributes.Add("placeholder", Placeholder);
                return html.TextBox("", Value ?? "", htmlAttributes);
            }
        }
    }
}
