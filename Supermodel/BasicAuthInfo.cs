using System;
using System.Text;
using System.Web;

namespace Supermodel
{
    //Added a comment
    
    /// <summary>
    /// This class Base64 encodes username and password in a cookie; it therefore must occur over SSL.
    /// </summary>
    public class BasicAuthInfo
    {
        public string Username { get; private set; }

        public string Password { get; private set; }

        private BasicAuthInfo(string username, string password)
        {
            Username = username;
            Password = password;
        }

        public static BasicAuthInfo CreateFromHttpContext()
        {
            string authToken = HttpContext.Current.Request.Headers["Authorization"];
            if (authToken == null) return null;

            string authTokenDecoded = Encoding.UTF8.GetString(Convert.FromBase64String(authToken.Substring("Basic".Length).Trim()));
            var parts = authTokenDecoded.Split(':');
            string username = parts[0];
            string password = parts[1];

            return new BasicAuthInfo(username, password);
        }

        public static string CreateAuthToken(string username, string password)
        {
            //example "Basic aWJhaXNuOjkzNjcyNQ=="
            var payload = Encoding.Default.GetBytes(string.Join(":", new[] { username, password }));
            return "Basic " + Convert.ToBase64String(payload);
        }
    }  

        
        /*
        public object BindModel(System.Web.Mvc.ControllerContext controllerContext, System.Web.Mvc.ModelBindingContext bindingContext)
        {
            if (controllerContext == null) throw new ArgumentNullException("controllerContext");
            
            //string authToken = "Basic " + Convert.ToBase64String(Encoding.Default.GetBytes("ibaisn:936725")); //"Basic aWJhaXNuOjkzNjcyNQ=="
            string authToken = controllerContext.HttpContext.Request.Headers["Authorization"];
            if (authToken == null) return null;

            string authTokenDecoded = Encoding.UTF8.GetString(Convert.FromBase64String(authToken.Replace("Basic", "").Trim()));
            string username = authTokenDecoded.Substring(0, authTokenDecoded.IndexOf(":"));
            string password = authTokenDecoded.Substring(authTokenDecoded.IndexOf(":") + 1);

            return new BasicAuthInfo(username, password);
        }
        */
    //}

    //public class BasicAuthFormatter : MediaTypeFormatter
    //{
    //    protected override System.Threading.Tasks.Task<object> OnReadFromStreamAsync(Type type, Stream stream, HttpContentHeaders contentHeaders, FormatterContext formatterContext)
    //    {
    //        return base.OnReadFromStreamAsync(type, stream, contentHeaders, formatterContext);
    //    }
    //}
}
