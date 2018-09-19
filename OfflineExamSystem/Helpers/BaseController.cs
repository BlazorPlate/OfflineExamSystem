using System;
using System.Threading;
using System.Web.Mvc;

namespace OfflineExamSystem.Helpers
{
    public class BaseController : Controller
    {
        #region Protected Methods
        //protected override IAsyncResult BeginExecuteCore(AsyncCallback callback, object state)
        //{
        //    CultureInfo ci = new CultureInfo("ar-JO");
        //    Thread.CurrentThread.CurrentCulture = ci;
        //    Thread.CurrentThread.CurrentUICulture = ci;
        //    return base.BeginExecuteCore(callback, state);
        //}
        protected override IAsyncResult BeginExecuteCore(AsyncCallback callback, object state)
        {
            string cultureName = RouteData.Values["culture"] as string;
            // Attempt to read the culture cookie from Request
            if (cultureName == null)
                cultureName = Request.UserLanguages != null && Request.UserLanguages.Length > 0 ? Request.UserLanguages[0] : null; // obtain it from HTTP header AcceptLanguages

            // Validate culture name
            cultureName = CultureHelper.GetImplementedCulture(cultureName);
            if (RouteData.Values["culture"] as string != cultureName)
            {
                // Force a valid culture in the URL
                RouteData.Values["culture"] = cultureName.ToLowerInvariant(); // lower case too
                Response.RedirectToRoute("Default", RouteData.Values); // Redirect user
            }
            // Modify current thread's cultures
            Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(cultureName);
            Thread.CurrentThread.CurrentUICulture = Thread.CurrentThread.CurrentCulture;
            return base.BeginExecuteCore(callback, state);
        }
        #endregion Protected Methods
    }
}