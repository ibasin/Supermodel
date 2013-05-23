using System.Web.Mvc;

// ReSharper disable CheckNamespace
namespace Supermodel.DDD.Models.View.Mvc.JQMobile
// ReSharper restore CheckNamespace
{
    public abstract partial class JQMobile
    {
        public class RadioSelectVerticalMvcModelUsing<MvcModelT> : Mvc.UIComponents.RadioSelectMvcModelUsing<MvcModelT> where MvcModelT : MvcModelForEntityCore
        {
            public override MvcHtmlString EditorTemplate(HtmlHelper html, int screenOrderFrom = int.MinValue, int screenOrderTo = int.MaxValue, string markerAttribute = null)
            {
                return RadioSelectVerticalMvcModel.RadioSelectCommonEditorTemplate(html, screenOrderFrom, screenOrderTo, markerAttribute);
            }
        }
    }
}

