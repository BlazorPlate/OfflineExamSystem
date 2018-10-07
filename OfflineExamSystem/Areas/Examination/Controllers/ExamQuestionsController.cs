using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using OfflineExamSystem.Areas.Examination.Models;
using OfflineExamSystem.Helpers;

namespace OfflineExamSystem.Areas.Examination.Controllers
{
    [LocalizedAuthorize(Roles = "Admin")]
    public class ExamQuestionsController : BaseController
    {
        private OfflineExamDBEntities db = new OfflineExamDBEntities();

        // GET: Examination/ExamQuestions
        public ActionResult Index()
        {
            var examQuestions = db.ExamQuestions.Include(e => e.Exam).Include(e => e.Question);
            return View(examQuestions.ToList());
        }

        // GET: Examination/ExamQuestions/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ExamQuestion examQuestion = db.ExamQuestions.Find(id);
            if (examQuestion == null)
            {
                return HttpNotFound();
            }
            return View(examQuestion);
        }

        // GET: Examination/ExamQuestions/Create
        public ActionResult Create()
        {
            ViewBag.ExamId = new SelectList(db.Exams, "Id", Thread.CurrentThread.CurrentCulture.Name.Equals("en-US") ? "Name_En" : "Name_En");
            ViewBag.QuestionId = new SelectList(db.Questions, "Id", Thread.CurrentThread.CurrentCulture.Name.Equals("en-US") ? "Question_En" : "Question_Ar");
            return View();
        }

        // POST: Examination/ExamQuestions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,ExamId,QuestionId,QuestionNumber,IsActive")] ExamQuestion examQuestion)
        {
            if (ModelState.IsValid)
            {
                db.ExamQuestions.Add(examQuestion);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ExamId = new SelectList(db.Exams, "Id", Thread.CurrentThread.CurrentCulture.Name.Equals("en-US") ? "Name_En" : "Name_En", examQuestion.ExamId);
            ViewBag.QuestionId = new SelectList(db.Questions, "Id", Thread.CurrentThread.CurrentCulture.Name.Equals("en-US") ? "Question_En" : "Question_Ar", examQuestion.QuestionId);
            return View(examQuestion);
        }

        // GET: Examination/ExamQuestions/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ExamQuestion examQuestion = db.ExamQuestions.Find(id);
            if (examQuestion == null)
            {
                return HttpNotFound();
            }
            ViewBag.ExamId = new SelectList(db.Exams, "Id", Thread.CurrentThread.CurrentCulture.Name.Equals("en-US") ? "Name_En" : "Name_En", examQuestion.ExamId);
            ViewBag.QuestionId = new SelectList(db.Questions, "Id", Thread.CurrentThread.CurrentCulture.Name.Equals("en-US") ? "Question_En" : "Question_Ar", examQuestion.QuestionId);
            return View(examQuestion);
        }

        // POST: Examination/ExamQuestions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,ExamId,QuestionId,QuestionNumber,IsActive")] ExamQuestion examQuestion)
        {
            if (ModelState.IsValid)
            {
                db.Entry(examQuestion).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ExamId = new SelectList(db.Exams, "Id", Thread.CurrentThread.CurrentCulture.Name.Equals("en-US") ? "Name_En" : "Name_En", examQuestion.ExamId);
            ViewBag.QuestionId = new SelectList(db.Questions, "Id", Thread.CurrentThread.CurrentCulture.Name.Equals("en-US") ? "Question_En" : "Question_Ar", examQuestion.QuestionId);
            return View(examQuestion);
        }

        // GET: Examination/ExamQuestions/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ExamQuestion examQuestion = db.ExamQuestions.Find(id);
            if (examQuestion == null)
            {
                return HttpNotFound();
            }
            return View(examQuestion);
        }

        // POST: Examination/ExamQuestions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ExamQuestion examQuestion = db.ExamQuestions.Find(id);
            db.ExamQuestions.Remove(examQuestion);
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
