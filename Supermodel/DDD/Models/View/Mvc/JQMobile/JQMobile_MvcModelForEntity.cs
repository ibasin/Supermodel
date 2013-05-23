using System.Linq;
using System.Web.Mvc;
using Supermodel.DDD.Models.Domain;
using Supermodel.DDD.Models.View.Mvc.UIComponents;

namespace Supermodel.DDD.Models.View.Mvc.JQMobile
{
    public abstract partial class JQMobile
    {
        public abstract class MvcModelForEntity<EntityT> : Mvc.MvcModelForEntity<EntityT> where EntityT : class, IEntity, new()
        {
            public override MvcHtmlString EditorTemplate(HtmlHelper html, int screenOrderFrom = int.MinValue, int screenOrderTo = int.MaxValue, string markerAttribute = null)
            {
                return MvcModel.JQMobileCommonMvcModelEditorTemplate(html, screenOrderFrom, screenOrderTo, markerAttribute);
            }
        }
    }
}

