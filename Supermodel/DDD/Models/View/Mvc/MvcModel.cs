using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using ReflectionMapper;
using Supermodel.DDD.Models.View.Mvc.Metadata;
using Supermodel.Mvc.Extensions;

namespace Supermodel.DDD.Models.View.Mvc
{
    public abstract class MvcModel : IMvcModelEditorTemplate
    {
        public virtual bool GetIEditorTemplateImplemented() { return true; }
        public virtual MvcHtmlString EditorTemplate(HtmlHelper html, int screenOrderFrom = int.MinValue, int screenOrderTo = int.MaxValue, string markerAttribute = null)
        {
            if (html.ViewData.Model == null) throw new NullReferenceException(ReflectionHelper.GetCurrentContext() + " is called for a model that is null");
            if (!(html.ViewData.Model is MvcModel)) throw new InvalidCastException(ReflectionHelper.GetCurrentContext() + " is called for a model of type different from MvcModel.");

            var result = new StringBuilder();
            if (screenOrderFrom == int.MinValue) result.AppendLine("<table" + SupermodelSettings.Scaffolding.MakeIdAndClassAttribues(SupermodelSettings.Scaffolding.FormTableId, SupermodelSettings.Scaffolding.FormTableCssClass) + ">");
            if (html.ViewData.TemplateInfo.TemplateDepth <= 1)
            {
                var properties = html.ViewData.ModelMetadata.Properties.Where(
                        pm => pm.ShowForEdit && 
                        !html.ViewData.TemplateInfo.Visited(pm) && 
                        (pm.AdditionalValues.ContainsKey("ScreenOrder") ? ((ScreenOrderAttribute)pm.AdditionalValues["ScreenOrder"]).Order : 100) >= screenOrderFrom &&
                        (pm.AdditionalValues.ContainsKey("ScreenOrder") ? ((ScreenOrderAttribute)pm.AdditionalValues["ScreenOrder"]).Order : 100) <= screenOrderTo)
                        .OrderBy(pm => pm.AdditionalValues.ContainsKey("ScreenOrder") ? ((ScreenOrderAttribute)pm.AdditionalValues["ScreenOrder"]).Order : 100);

                foreach (var prop in properties)
                {
                    //By default we do not scaffold ICollections
                    if (prop.ModelType.Name == typeof(ICollection<>).Name) continue;

                    if (prop.HideSurroundingHtml)
                    {
                        result.AppendLine(html.Editor(prop.PropertyName).ToString());
                    }
                    else
                    {
                        var propMarketAttribute = markerAttribute;
                        if (prop.AdditionalValues.ContainsKey("HtmlAttr")) propMarketAttribute += " " + ((HtmlAttrAttribute)prop.AdditionalValues["HtmlAttr"]).Attr;
                        result.AppendLine("<tr " + propMarketAttribute + ">");

                        //Label
                        if (!prop.AdditionalValues.ContainsKey("HideLabel"))
                        {
                            result.AppendLine("<td>");
                            result.AppendLine("<span class = '" + SupermodelSettings.Scaffolding.LabelCssClass + "'>");

                            result.AppendLine(html.Label(prop.PropertyName).ToString());

                            if (!prop.AdditionalValues.ContainsKey("NoRequiredLabel"))
                            {
                                if ((prop.IsRequired && prop.ModelType != typeof(bool)) || prop.AdditionalValues.ContainsKey("ForceRequiredLabel"))
                                {
                                    result.AppendLine("<span" + SupermodelSettings.Scaffolding.MakeClassAttribue(SupermodelSettings.Scaffolding.RequiredAsteriskCssClass) + "><sup>*</sup></span>");
                                }
                            }
                            result.AppendLine("</span>");
                            result.AppendLine("</td>");
                        }

                        //Value
                        result.AppendLine(!prop.AdditionalValues.ContainsKey("HideLabel") ? "<td>" : "<td colspan='2'>");
                        if (!prop.AdditionalValues.ContainsKey("DisplayOnly") || prop.IsReadOnly)
                        {
                            result.AppendLine(html.Supermodel().Editor(prop.PropertyName).ToString());
                            result.AppendLine("<br />");
                            var validationMessage = html.ValidationMessage(prop.PropertyName);
                            if (validationMessage != null) result.AppendLine(validationMessage.ToString());
                        }
                        else
                        {
                            result.AppendLine("<span class ='" + SupermodelSettings.Scaffolding.DisplayCssClass + "'>");
                            result.AppendLine(html.Supermodel().Display(prop.PropertyName).ToString());
                            result.AppendLine("</span>");
                        }
                        result.AppendLine("</td>");

                        result.AppendLine("</tr>");
                    }
                }
            }
            if (screenOrderTo == int.MaxValue) result.Append("</table>");
            return MvcHtmlString.Create(result.ToString());
        }
    }
}