using System;
using System.Text;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace Supermodel.DDD.Models.View.Mvc.UIComponents
{
    public class RadioSelectMvcModel : DropdownMvcModel
    {
        public override MvcHtmlString EditorTemplate(HtmlHelper html, int screenOrderFrom = int.MinValue, int screenOrderTo = int.MaxValue, string markerAttribute = null)
        {
            return RadioSelectCommonEditorTemplate(html, screenOrderFrom, screenOrderTo, markerAttribute);
        }

        public static MvcHtmlString RadioSelectCommonEditorTemplate(HtmlHelper html, int screenOrderFrom = int.MinValue, int screenOrderTo = int.MaxValue, string markerAttribute = null)
        {
            if (html.ViewData.Model == null) throw new NullReferenceException("Html.RadioSelectFormModelEditor() is called for a model that is null");
            if (!(html.ViewData.Model is DropdownMvcModel)) throw new InvalidCastException("Html.RadioSelectFormModelEditor() is called for a model of type diffrent from RadioSelectFormModel.");

            var radio = (DropdownMvcModel)html.ViewData.Model;
            var result = new StringBuilder();
            result.AppendLine("<ul>");
            foreach (var option in radio.Options)
            {
                var isSelectedOption = (radio.SelectedValue != null && string.CompareOrdinal(radio.SelectedValue, option.Value) == 0);
                if (isSelectedOption || !option.IsDisabled)
                {
                    result.AppendLine("<li>");
                    result.AppendLine(html.RadioButton("", option.Value, (radio.SelectedValue == option.Value)) + (!option.IsDisabled ? option.Label : option.Label + " [DISABLED]"));
                    result.AppendLine("</li>");
                }
            }
            result.AppendLine("</ul>");
            return MvcHtmlString.Create(result.ToString());
        }
    }
}
