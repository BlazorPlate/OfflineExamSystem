using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Optimization;
using System.Web.Routing;

namespace OfflineExamSystem
{
    // Note: For instructions on enabling IIS7 classic mode,
    // visit http://go.microsoft.com/?LinkId=301868
    public class MvcApplication : System.Web.HttpApplication
    {
        #region Protected Methods
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            DataAnnotationsModelValidatorProvider.AddImplicitRequiredAttributeForValueTypes = false;
            DefaultModelBinder.ResourceClassKey = "ValidationResources";
            ClientDataTypeModelValidatorProvider.ResourceClassKey = "ValidationResources";
            ValidationExtensions.ResourceClassKey = "ValidationResources";
        }
        #endregion Protected Methods
    }
}