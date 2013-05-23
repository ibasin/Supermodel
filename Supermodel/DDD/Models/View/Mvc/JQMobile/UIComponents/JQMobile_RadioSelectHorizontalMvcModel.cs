using System.Web.Mvc;

// ReSharper disable CheckNamespace
namespace Supermodel.DDD.Models.View.Mvc.JQMobile
// ReSharper restore CheckNamespace
{
    public abstract partial class JQMobile
    {
        public class RadioSelectHorizontalMvcModel : Mvc.UIComponents.RadioSelectMvcModel
        {
            public override MvcHtmlString EditorTemplate(HtmlHelper html, int screenOrderFrom = int.MinValue, int screenOrderTo = int.MaxValue, string markerAttribute = null)
            {
                return RadioSelectCommonEditorTemplate(html, screenOrderFrom, screenOrderTo, markerAttribute);
            }

            public static new MvcHtmlString RadioSelectCommonEditorTemplate(HtmlHelper html, int screenOrderFrom = int.MinValue, int screenOrderTo = int.MaxValue, string markerAttribute = null)
            {
                var result = RadioSelectVerticalMvcModel.RadioSelectCommonEditorTemplate(html, screenOrderFrom, screenOrderTo, markerAttribute).ToString().Replace("<fieldset data-role='controlgroup'>", "<fieldset data-role='controlgroup' data-type='horizontal'>");
                return MvcHtmlString.Create(result);
            }            
        }
    }
}
