using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Supermodel.DDD.Models.View.Mvc.UIComponents
{
    public class ImageMvcModel : BinaryFileMvcModel, IMvcModelDisplayTemplate
    {
        public ImageMvcModel()
        {
            HtmlAttributesAsObj = null;
        }
        
        public override object MapToObjectCustom(object obj, Type objType)
        {
            //do nothing
            return obj;
        }

        public override MvcHtmlString EditorTemplate(HtmlHelper html, int screenOrderFrom = int.MinValue, int screenOrderTo = int.MaxValue, string markerAttribute = null)
        {
            return DisplayTemplate(html, screenOrderFrom, screenOrderTo, markerAttribute);
        }

        public bool GetIDisplayTemplateImplemented() { return true; }
        public MvcHtmlString DisplayTemplate(HtmlHelper html, int screenOrderFrom = int.MinValue, int screenOrderTo = int.MaxValue, string markerAttribute = null)
        {
            return new MvcHtmlString(string.Format("<div><img src='/{0}/GetBinaryFile/{1}?pn={2}' {3}/></div>", html.ViewContext.RouteData.Values["Controller"], html.ViewContext.RouteData.Values["id"], html.ViewData.ModelMetadata.PropertyName, UtilsLib.GenerateAttributesString(HtmlAttributesAsDict)));
        }

        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            throw new NotImplementedException(); //This should never be called
        }

        #region Properties
        public object HtmlAttributesAsObj { set { HtmlAttributesAsDict = value == null ? null : UtilsLib.ObjectToCaseSensitiveDictionary(value); } }
        public IDictionary<string, object> HtmlAttributesAsDict { get; set; }
        #endregion

    }
}
