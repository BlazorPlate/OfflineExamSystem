using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OfflineExamSystem.Helpers;

namespace OfflineExamSystem.Controllers
{
    [LocalizedAuthorize]
    public class HomeController : BaseController
    {
        public ActionResult Index()
        {
            if (!User.IsInRole("Admin"))
            {
                return RedirectToAction("Index", "Home", new { area = "Examination" });
            }
            return View();
        }
    }
}