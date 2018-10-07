using OfflineExamSystem.Areas.Examination.Models;
using OfflineExamSystem.Helpers;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace OfflineExamSystem.Areas.Examination.Controllers
{
    [LocalizedAuthorize(Roles = "Admin")]
    public class QuestionsController : BaseController
    {
        private OfflineExamDBEntities db = new OfflineExamDBEntities();

        // GET: Examination/Questions
        public ActionResult Index()
        {
            return View();
        }
        public PartialViewResult QuestionGrid(string globalSearch, int? page, int? rows, string sort, string order)
        {
            IQueryable<Question> query = (from q in db.Questions select q);
            if (!string.IsNullOrEmpty(globalSearch))
            {
                query = query.Where(q => q.Question_En.ToLower().Contains(globalSearch.Trim().ToLower()) || q.Question_Ar.ToLower().Contains(globalSearch.Trim().ToLower()));
            }
            query = query.OrderBy(q => q.Id);
            ViewBag.TotalRows = query.Count();
            return PartialView("_QuestionGrid", query.Skip((page - 1 ?? 0) * (rows ?? 10)).Take(rows ?? 10).ToList());
        }

        // GET: Examination/Questions/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Question question = db.Questions.Find(id);
            if (question == null)
            {
                return HttpNotFound();
            }
            return View(question);
        }

        // GET: Examination/Questions/Create
        public ActionResult Create()
        {
            ViewBag.CategoryId = new SelectList(db.Categories, "Id", "Category1");
            return View();
        }

        // POST: Examination/Questions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,CategoryId,Question_En,Question_Ar,CorrectHint_En,CorrectHint_Ar,WrongHint_En,WrongHint_Ar")] Question question)
        {
            question.QuestionType = "MULTIPLE";
            if (ModelState.IsValid)
            {
                db.Questions.Add(question);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CategoryId = new SelectList(db.Categories, "Id", "Category1", question.CategoryId);
            return View(question);
        }

        // GET: Examination/Questions/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Question question = db.Questions.Find(id);
            if (question == null)
            {
                return HttpNotFound();
            }
            ViewBag.CategoryId = new SelectList(db.Categories, "Id", "Category1", question.CategoryId);
            question.QuestionType = "MULTIPLE";
            return View(question);
        }

        // POST: Examination/Questions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,CategoryId,QuestionType,Question_En,Question_Ar,CorrectHint_En,CorrectHint_Ar,WrongHint_En,WrongHint_Ar")] Question question)
        {
            if (ModelState.IsValid)
            {
                db.Entry(question).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CategoryId = new SelectList(db.Categories, "Id", "Category1", question.CategoryId);
            return View(question);
        }

        // GET: Examination/Questions/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Question question = db.Questions.Find(id);
            if (question == null)
            {
                return HttpNotFound();
            }
            return View(question);
        }

        // POST: Examination/Questions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Question question = db.Questions.Find(id);
            db.Questions.Remove(question);
            db.SaveChanges();
            return RedirectToAction("Index");
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
