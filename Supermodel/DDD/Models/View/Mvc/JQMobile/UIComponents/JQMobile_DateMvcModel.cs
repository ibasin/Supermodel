using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Mvc.Html;

// ReSharper disable CheckNamespace
namespace Supermodel.DDD.Models.View.Mvc.JQMobile
// ReSharper restore CheckNamespace
{
    public abstract partial class JQMobile
    {
        public class DateMvcModel : Mvc.UIComponents.DateMvcModel
        {
            #region Constructors
            public DateMvcModel()
            {
                Placeholder = null;
            }
            #endregion

            #region Overrides
            public override MvcHtmlString EditorTemplate(HtmlHelper html, int screenOrderFrom = int.MinValue, int screenOrderTo = int.MaxValue, string markerAttribute = null)
            {
                var dateTimeStr = (Value == null) ? "" : ((DateTime)Value).ToString("yyyy-MM-dd");
                var htmlAttributes = new Dictionary<string, object> {{"type", "date"}, {"data-clear-btn", true}};
                if (Placeholder != null) htmlAttributes.Add("placeholder", Placeholder);
                return html.TextBox("", dateTimeStr, htmlAttributes);
            }
            #endregion

            #region Properties
            public string Placeholder { get; set; }
            #endregion
        }
    }
}
