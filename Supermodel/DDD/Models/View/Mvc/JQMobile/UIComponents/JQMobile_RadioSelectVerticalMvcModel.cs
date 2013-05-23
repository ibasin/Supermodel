using System;
using System.Text;
using System.Web.Mvc;
using System.Web.Mvc.Html;

// ReSharper disable CheckNamespace
namespace Supermodel.DDD.Models.View.Mvc.JQMobile
// ReSharper restore CheckNamespace
{
    public abstract partial class JQMobile
    {
        public class RadioSelectVerticalMvcModel : Mvc.UIComponents.RadioSelectMvcModel
        {
            public override MvcHtmlString EditorTemplate(HtmlHelper html, int screenOrderFrom = int.MinValue, int screenOrderTo = int.MaxValue, string markerAttribute = null)
            {
                return RadioSelectCommonEditorTemplate(html, screenOrderFrom, screenOrderTo, markerAttribute);
            }

            public static new MvcHtmlString RadioSelectCommonEditorTemplate(HtmlHelper html, int screenOrderFrom = int.MinValue, int screenOrderTo = int.MaxValue, string markerAttribute = null)
            {
                if (html.ViewData.Model == null) throw new NullReferenceException("Html.RadioSelectFormModelEditor() is called for a model that is null");
                if (!(html.ViewData.Model is Mvc.UIComponents.DropdownMvcModel)) throw new InvalidCastException("Html.RadioSelectFormModelEditor() is called for a model of type diffrent from RadioSelectFormModel.");

                var radio = (Mvc.UIComponents.DropdownMvcModel)html.ViewData.Model;
                var result = new StringBuilder();
                result.AppendLine("<fieldset data-role='controlgroup'>");
                result.AppendLine("<legend></legend>");
                var index = 1;
                foreach (var option in radio.Options)
                {
                    var isSelectedOption = (radio.SelectedValue != null && string.CompareOrdinal(radio.SelectedValue, option.Value) == 0);
                    if (isSelectedOption || !option.IsDisabled)
                    {
                        var id = html.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName("") + "__" + index++;
                        result.AppendLine(html.RadioButton("", option.Value, (radio.SelectedValue == option.Value), new { id = id }).ToString());
                        var labelText = !option.IsDisabled ? option.Label : option.Label + " [DISABLED]";
                        result.AppendLine("<label for='" + id + "'>" + labelText + "</label>");
                    }
                }
                result.AppendLine("</fieldset>");
                return MvcHtmlString.Create(result.ToString());
            }            
        }
    }
}
