using System.Web.Http;
using System.Net;
using System.Net.Http;

namespace Supermodel.Mvc
{
    public static class HttpResponseHelper
    {
        public static HttpResponseException CreateException(HttpStatusCode status, string reasonPhrase)
        {
            var response = new HttpResponseMessage(HttpStatusCode.Forbidden) { ReasonPhrase = reasonPhrase };
            return new HttpResponseException(response);
        }
    }
}
