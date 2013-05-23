using System;
using System.Web.Mvc;

namespace Supermodel.DDD.Models.View.Mvc.UIComponents
{
    public class RadioSelectMvcModelUsingEnum<EnumT> : DropdownMvcModelUsingEnum<EnumT> where EnumT : struct, IConvertible
    {
        public override MvcHtmlString EditorTemplate(HtmlHelper html, int screenOrderFrom = int.MinValue, int screenOrderTo = int.MaxValue, string markerAttribute = null)
        {
            return RadioSelectMvcModel.RadioSelectCommonEditorTemplate(html, screenOrderFrom, screenOrderTo, markerAttribute);
        }
    }

}
