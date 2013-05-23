using System.Web.Mvc;
using Supermodel.DDD.Models.Domain;

namespace Supermodel.DDD.Models.View.Mvc.JQMobile
{
    public abstract partial class JQMobile
    {
        public abstract class ChildMvcModelForEntity<EntityT, ParentEntityT> : Mvc.ChildMvcModelForEntity<EntityT, ParentEntityT> 
            where EntityT : class, IEntity, new()
            where ParentEntityT : class, IEntity, new()
        {
            public override MvcHtmlString EditorTemplate(HtmlHelper html, int screenOrderFrom = int.MinValue, int screenOrderTo = int.MaxValue, string markerAttribute = null)
            {
                return MvcModel.JQMobileCommonMvcModelEditorTemplate(html, screenOrderFrom, screenOrderTo, markerAttribute);
            }
        }
    }
}
