using System.Web.Mvc;

namespace Supermodel.DDD.Models.View.Mvc
{
    public interface IMvcModelDisplayTemplate
    {
        bool GetIDisplayTemplateImplemented();
        MvcHtmlString DisplayTemplate(HtmlHelper html, int screenOrderFrom = int.MinValue, int screenOrderTo = int.MaxValue, string markerAttribute = null);    
    }
}
