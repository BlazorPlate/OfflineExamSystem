using OfflineExamSystem.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OfflineExamSystem.Controllers
{
    [LocalizedAuthorize(Roles = "CRM")]
    public class TestAuthorizationController : BaseController
    {
        // GET: TestAuthorization
        public ActionResult Index()
        {
            return View();
        }

        // GET: TestAuthorization/Sales
        [LocalizedAuthorize(Roles = "CEO,Audit,Sales")]
        public ActionResult Sales()
        {
            return View();
        }

        // GET: TestAuthorization/Marketing
        [LocalizedAuthorize(Roles = "CEO,Marketing")]
        public ActionResult Marketing()
        {
            return View();
        }

        // GET: TestAuthorization/Accounting
        [LocalizedAuthorize(Roles = "CEO,Audit,Accounting")]
        public ActionResult Accounting()
        {
            return View();
        }

        // GET: TestAuthorization/Operations
        [LocalizedAuthorize(Roles = "CEO,Operations")]
        public ActionResult Operations()
        {
            return View();
        }

        // GET: TestAuthorization/Support
        [LocalizedAuthorize(Roles = "CEO,Support")]
        public ActionResult Support()
        {
            return View();
        }
    }
}
