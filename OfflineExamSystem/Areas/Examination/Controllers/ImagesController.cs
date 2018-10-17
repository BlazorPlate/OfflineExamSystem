using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using OfflineExamSystem.Areas.Examination.Models;
using OfflineExamSystem.Helpers;
using PagedList;

namespace OfflineExamSystem.Areas.Examination.Controllers
{
    public class ImagesController : BaseController
    {
        private OfflineExamDBEntities db = new OfflineExamDBEntities();
        // GET: Examination/Images/Details/5
        public ActionResult Index(int? page)
        {
            var pageNumber = page ?? 1; // if no page was specified in the querystring, default to the first page (1)
            IEnumerable<Image> images = db.Images.OrderBy(u => u.PageNum).ToPagedList(pageNumber, 1); // will only contain 25 products max because of the pageSize
            return View(images);
        }
        public ActionResult Direction(string direction, int? page)
        {
            int nextQuestionNumber = 1;
            if (direction.Equals("forward", StringComparison.CurrentCultureIgnoreCase))
            {
                nextQuestionNumber = db.Images.Where(x => x.PageNum > page)
                .OrderBy(x => x.PageNum).Take(1).Select(x => x.PageNum).FirstOrDefault();
            }
            else
            {
                nextQuestionNumber = db.Images.Where(x => x.PageNum < page)
                .OrderByDescending(x => x.PageNum).Take(1).Select(x => x.PageNum).FirstOrDefault();
            }
            if (nextQuestionNumber < 1)
            {
                nextQuestionNumber = 1;
            }

            return RedirectToAction("Index", new
            {
                @page = nextQuestionNumber
            });
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
