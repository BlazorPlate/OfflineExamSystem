using System.Web.Optimization;

namespace OfflineExamSystem
{
    public class BundleConfig
    {
        #region Public Methods
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*",
                        "~/Scripts/jquery.unobtrusive.js",
                        "~/Scripts/jquery.unobtrusive-ajax.js"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap-rtl").Include(
                    "~/Scripts/bootstrap-rtl.js",
                    "~/Scripts/respond.js"));

            bundles.Add(new StyleBundle("~/Content/style").Include(
                       "~/Content/bootstrap.css",
                       "~/Content/site.css"));

            bundles.Add(new StyleBundle("~/Content/style-rtl").Include(
                   "~/Content/bootstrap-rtl.css",
                   "~/Content/site.css"));
        }
        #endregion Public Methods
    }
}