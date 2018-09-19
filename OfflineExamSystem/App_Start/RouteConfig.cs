using OfflineExamSystem.Helpers;
using System.Web.Mvc;
using System.Web.Routing;

namespace OfflineExamSystem
{
    public class RouteConfig
    {
        #region Public Methods
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{culture}/{controller}/{action}/{id}",
                defaults: new { culture = CultureHelper.GetDefaultCulture(), controller = "Home", action = "Index", id = UrlParameter.Optional },
                namespaces: new[] { "OfflineExamSystem.Controllers" });
        }
        #endregion Public Methods
    }
}