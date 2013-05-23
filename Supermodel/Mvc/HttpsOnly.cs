namespace Supermodel.Mvc
{
    using System;
    using System.Net.Http;
    using System.Web.Http.Filters;
    using System.Web.Http.Controllers;
    using System.Net;

    public class HttpsOnly : ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            if (!String.Equals(actionContext.Request.RequestUri.Scheme, "https", StringComparison.OrdinalIgnoreCase))
            {
                actionContext.Response = new HttpResponseMessage(HttpStatusCode.BadRequest) { Content = new StringContent("HTTPS Required") };
            }
        }
    }
}