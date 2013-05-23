using System.Web.Mvc;
using Supermodel.DDD.Models.Domain;

namespace Supermodel.DDD.Models.View.Mvc
{
    public interface IMvcModelEditorTemplate
    {
        /// <summary>
        /// false when SuperModel should allow MVC to use MVC ViewTemplates to display the model.  <see cref="IEntity"/>'s 
        /// implementation returns true.
        /// </summary>
        /// <returns></returns>
        bool GetIEditorTemplateImplemented();
        MvcHtmlString EditorTemplate(HtmlHelper html, int screenOrderFrom = int.MinValue, int screenOrderTo = int.MaxValue, string markerAttribute = null);
    }
}
