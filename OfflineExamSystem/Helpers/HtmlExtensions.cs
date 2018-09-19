using System;
using System.Web;
using System.Web.Mvc;
using System.Web.WebPages;

namespace OfflineExamSystem.Helpers
{
    public static class HtmlExtensions
    {
        #region Public Methods
        public static MvcHtmlString Script(this System.Web.Mvc.HtmlHelper htmlHelper, Func<object, HelperResult> template)
        {
            htmlHelper.ViewContext.HttpContext.Items["_script_" + Guid.NewGuid()] = template;
            return MvcHtmlString.Empty;
        }

        public static IHtmlString RenderScripts(this System.Web.Mvc.HtmlHelper htmlHelper)
        {
            foreach (object key in htmlHelper.ViewContext.HttpContext.Items.Keys)
            {
                if (key.ToString().StartsWith("_script_"))
                {
                    var template = htmlHelper.ViewContext.HttpContext.Items[key] as Func<object, HelperResult>;
                    if (template != null)
                    {
                        htmlHelper.ViewContext.Writer.Write(template(null));
                    }
                }
            }
            return MvcHtmlString.Empty;
        }
        #endregion Public Methods
    }
}