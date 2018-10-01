using OfflineExamSystem.Areas.Examination.Models;
using OfflineExamSystem.Helpers;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading;
using System.Web.Mvc;

namespace OfflineExamSystem.Areas.Examination.Controllers
{
    public class ChoicesController : BaseController
    {
        private OfflineExamDBEntities db = new OfflineExamDBEntities();

        // GET: Examination/Choices
        public ActionResult Index(int? questionId)
        {
            if (questionId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            IQueryable<Choice> choices = db.Choices.Include(c => c.Question).Where(c => c.QuestionId == questionId);
            ViewBag.QuestionId = questionId;
            return View(choices.ToList());
        }

        // GET: Examination/Choices/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Choice choice = db.Choices.Find(id);
            if (choice == null)
            {
                return HttpNotFound();
            }
            return View(choice);
        }

        // GET: Examination/Choices/Create
        public ActionResult Create(int? questionId)
        {
            if (questionId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ViewBag.Question = Thread.CurrentThread.CurrentCulture.Name.Equals("en-US") ? db.Questions.Find(questionId).Question_En : db.Questions.Find(questionId).Question_Ar;
            Choice choice = new Choice();
            choice.QuestionId = questionId.Value;
            return View(choice);
        }

        // POST: Examination/Choices/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,QuestionId,Label_En,Label_Ar,Points,IsActive")] Choice choice)
        {
            if (ModelState.IsValid)
            {
                db.Choices.Add(choice);
                db.SaveChanges();
                return RedirectToAction("Index", new { questionId = choice.QuestionId });
            }
            ViewBag.Question = Thread.CurrentThread.CurrentCulture.Name.Equals("en-US") ? db.Questions.Find(choice.QuestionId).Question_En : db.Questions.Find(choice.QuestionId).Question_Ar;
            return View(choice);
        }

        // GET: Examination/Choices/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Choice choice = db.Choices.Find(id);
            if (choice == null)
            {
                return HttpNotFound();
            }
            ViewBag.Question = Thread.CurrentThread.CurrentCulture.Name.Equals("en-US") ? db.Questions.Find(choice.QuestionId).Question_En : db.Questions.Find(choice.QuestionId).Question_Ar;
            return View(choice);
        }

        // POST: Examination/Choices/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,QuestionId,Label_En,Label_Ar,Points,IsActive")] Choice choice)
        {
            if (ModelState.IsValid)
            {
                db.Entry(choice).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", new { questionId = choice.QuestionId });
            }
            ViewBag.Question = Thread.CurrentThread.CurrentCulture.Name.Equals("en-US") ? db.Questions.Find(choice.QuestionId).Question_En : db.Questions.Find(choice.QuestionId).Question_Ar;
            return View(choice);
        }

        // GET: Examination/Choices/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Choice choice = db.Choices.Find(id);
            if (choice == null)
            {
                return HttpNotFound();
            }
            return View(choice);
        }

        // POST: Examination/Choices/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Choice choice = db.Choices.Find(id);
            db.Choices.Remove(choice);
            db.SaveChanges();
            return RedirectToAction("Index", new { questionId = choice.QuestionId });
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
