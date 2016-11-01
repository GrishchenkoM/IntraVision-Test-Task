using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;

namespace Web.Filters
{
    public class SimpleAuthorizeAttribute : AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            var password = WebConfigurationManager.AppSettings["Admin"];
            var queryString = httpContext.Request.QueryString;

            var key = queryString.Get(0);

            return key == password || base.AuthorizeCore(httpContext);
        }
    }
}