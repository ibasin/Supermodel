using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Mvc.Html;

// ReSharper disable CheckNamespace
namespace Supermodel.DDD.Models.View.Mvc.JQMobile
// ReSharper restore CheckNamespace
{
    public abstract partial class JQMobile
    {
        public class TextAreaMvcModel : TextBoxForStringMvcModel
        {
            #region Constructors
            public TextAreaMvcModel()
            {
                Cols = 40;
                Rows = 8;
            }
            #endregion

            #region Overrides
            public override MvcHtmlString EditorTemplate(HtmlHelper html, int screenOrderFrom = int.MinValue, int screenOrderTo = int.MaxValue, string markerAttribute = null)
            {
                var htmlAttributes = new Dictionary<string, object> { { "cols", Cols }, { "Rows", Rows }, { "data-clear-btn", true } };
                if (Placeholder != null) htmlAttributes.Add("placeholder", Placeholder);
                return html.TextArea("", Value ?? "", htmlAttributes);
            }
            #endregion

            #region Properties
            public uint Cols { get; set; }
            public uint Rows { get; set; }
            #endregion
        }
    }
}
