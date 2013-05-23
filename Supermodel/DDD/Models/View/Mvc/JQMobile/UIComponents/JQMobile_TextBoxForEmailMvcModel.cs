using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Mvc.Html;

// ReSharper disable CheckNamespace
namespace Supermodel.DDD.Models.View.Mvc.JQMobile
// ReSharper restore CheckNamespace
{
    public abstract partial class JQMobile
    {
        public class TextBoxForEmailMvcModel : TextBoxForStringMvcModel
        {
            #region Overrides
            public override MvcHtmlString EditorTemplate(HtmlHelper html, int screenOrderFrom = int.MinValue, int screenOrderTo = int.MaxValue, string markerAttribute = null)
            {
                var htmlAttributes = new Dictionary<string, object> { { "type", "email" }, { "data-clear-btn", true } };
                if (Placeholder != null) htmlAttributes.Add("placeholder", Placeholder);
                return html.TextBox("", Value ?? "", htmlAttributes);
            }
            #endregion
        }
    }
}
