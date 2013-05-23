using System.Web.Mvc;

namespace Supermodel.DDD.Models.View.Mvc.UIComponents
{
    public class RadioSelectMvcModelUsing<MvcModelT> : DropdownMvcModelUsing<MvcModelT> where MvcModelT : MvcModelForEntityCore
    {
        public override MvcHtmlString EditorTemplate(HtmlHelper html, int screenOrderFrom = int.MinValue, int screenOrderTo = int.MaxValue, string markerAttribute = null)
        {
            return RadioSelectMvcModel.RadioSelectCommonEditorTemplate(html, screenOrderFrom, screenOrderTo, markerAttribute);
        }
    }
}
