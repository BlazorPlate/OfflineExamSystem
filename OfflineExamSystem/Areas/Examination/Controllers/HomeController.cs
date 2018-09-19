using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OfflineExamSystem.Helpers;
using OfflineExamSystem.Areas.Examination.Models;
using OfflineExamSystem.Areas.Examination.ViewModels;
using OfflineExamSystem.Areas.Examination.DTOs;
using System.Data.Entity;
using PagedList;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using OfflineExamSystem.Models;
using System.Threading;
using System.Globalization;
using System.Data.Entity.Validation;

namespace OfflineExamSystem.Areas.Examination.Controllers
{
    public class HomeController : BaseController
    {
        private List<object> examsList = new List<object>();
        private OfflineExamDBEntities db = new OfflineExamDBEntities();
        public ActionResult Index()
        {
            return View();
        }
        public JsonResult GetExamsByExamMode(bool examMode)
        {
            var exams = db.Exams.Where(c => c.ExamType == examMode).OrderBy(c => c.Name); ;
            foreach (var item in exams)
            {
                examsList.Add(new
                {
                    Id = item.Id,
                    Name = item.Name
                });
            }
            return Json(new SelectList(examsList, "Id", "Name", "--Choose exam type--"));
        }
        public JsonResult GetHintByQuestionId(int questionId)
        {
            var question = db.Questions.Find(questionId).Hint;
            return Json(question);
        }
        public ActionResult Init()
        {
            ViewBag.ExamId = new SelectList(db.Exams.Where(x => x.IsActive == true).Select(x => new { x.Id, x.Name }), "Id", "Name", "--Choose exam type--");
            SessionViewModel sessionViewModel = null;
            if (Session["SessionViewModel"] == null)
                sessionViewModel = new SessionViewModel();
            else
                sessionViewModel = (SessionViewModel)Session["SessionViewModel"];

            return View(sessionViewModel);
        }
        public ActionResult Router(string ExamMode, SessionViewModel sessionViewModel)
        {
            if (ExamMode.Equals("false"))
            {
                return RedirectToAction("Simulation", "Home", new { ExamId = sessionViewModel.ExamId });
            }
            else
            {
                return RedirectToAction("Instruction", "Home", new { ExamId = sessionViewModel.ExamId });
            }
        }
        public ActionResult Simulation(int ExamId)
        {
            SessionViewModel sessionViewModel = new SessionViewModel();
            sessionViewModel.ExamId = ExamId;
            if (sessionViewModel != null)
            {
                var test = db.Exams.Where(x => x.IsActive == true && x.Id == sessionViewModel.ExamId).FirstOrDefault();
                if (test != null)
                {
                    ViewBag.ExamName = test.Name;
                    ViewBag.ExamDescription = test.Description;
                    ViewBag.QuestionCount = test.ExamQuestions.Count;
                    ViewBag.ExamDuration = test.DurationInMinute;
                }
            }
            return View(sessionViewModel);
        }
        [LocalizedAuthorize(Roles = "Managerial_Examinee,Technician_Examinee")]
        public ActionResult Instruction(int ExamId)
        {
            SessionViewModel sessionViewModel = new SessionViewModel();
            var currentUser = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(System.Web.HttpContext.Current.User.Identity.GetUserId());
            if (currentUser != null)
            {
                sessionViewModel.ExamId = ExamId;
                sessionViewModel.UserName = currentUser.EmpId.ToString();
                sessionViewModel.FullName_Ar = currentUser.FullName_Ar;
                sessionViewModel.FullName_En = currentUser.FullName_En;
                sessionViewModel.Email = currentUser.Email;
            }
            if (sessionViewModel != null)
            {
                var test = db.Exams.Where(x => x.IsActive == true && x.Id == sessionViewModel.ExamId).FirstOrDefault();
                if (test != null)
                {
                    ViewBag.ExamName = test.Name;
                    ViewBag.ExamDescription = test.Description;
                    ViewBag.QuestionCount = test.ExamQuestions.Count;
                    ViewBag.ExamDuration = test.DurationInMinute;
                }
            }
            return View(sessionViewModel);
        }
        public ActionResult RegisterSimulation(SessionViewModel model, bool resume = false)
        {
            if (model != null)
                Session["SessionViewModel"] = model;
            if (model == null || model.ExamId < 1)
            {
                TempData["message"] = "Invalid Registraion details. Please try again";
                return RedirectToAction("Init");
            }
            Examinee examinee = new Examinee()
            {
                UserName = "Trainer_" + DateTime.Now,
                FullName_Ar = "Trainer_" + DateTime.Now,
                FullName_En = "Trainer_" + DateTime.Now,
                EntryDate = DateTime.Now,
            };
            db.Examinees.Add(examinee);
            try
            {
                db.SaveChanges();
            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                    }
                }
                throw;
            }


            Session session = db.Sessions.Where(x => x.ExamineeId == examinee.Id
            && x.ExamId == model.ExamId
            && x.TokenExpireTime > DateTime.Now).OrderByDescending(r => r.Id).FirstOrDefault();
            if (session != null && resume)
            {
                Session["TOKEN"] = session.Token;
                Session["TOKENEXPIRE"] = session.TokenExpireTime;
            }
            else
            {
                Exam exam = db.Exams.Where(x => x.IsActive && x.Id == model.ExamId).FirstOrDefault();
                if (exam != null)
                {
                    Session newSession = new Session()
                    {
                        RegistrationDate = DateTime.Now,
                        ExamId = model.ExamId,
                        Token = Guid.NewGuid(),
                        TokenExpireTime = DateTime.Now.AddMinutes(exam.DurationInMinute)
                    };
                    examinee.Sessions.Add(newSession);
                    db.Sessions.Add(newSession);
                    db.SaveChanges();
                    Session["TOKEN"] = newSession.Token;
                    Session["TOKENEXPIRE"] = newSession.TokenExpireTime;
                }
            }
            return RedirectToAction("ExamPaperSimulation", new { @token = Session["TOKEN"] });
        }
        public ActionResult Register(SessionViewModel model, bool resume = false)
        {
            if (model != null)
                Session["SessionViewModel"] = model;
            if (model == null || string.IsNullOrEmpty(model.UserName) || model.ExamId < 1)
            {
                TempData["message"] = "Invalid Registraion details. Please try again";
                return RedirectToAction("Init");
            }
            Examinee examinee = db.Examinees.Where(x => x.UserName.Equals(model.UserName, StringComparison.InvariantCultureIgnoreCase)
            && ((string.IsNullOrEmpty(model.Email) && string.IsNullOrEmpty(x.Email)) || (x.Email == model.Email))
            && ((string.IsNullOrEmpty(model.Phone) && string.IsNullOrEmpty(x.Phone)) || (x.Phone == model.Phone))).FirstOrDefault();
            if (examinee == null)
            {
                examinee = new Examinee()
                {
                    UserName = model.UserName,
                    FullName_En = model.FullName_En,
                    FullName_Ar = model.FullName_Ar,
                    Email = model.Email,
                    Phone = model.Phone,
                    EntryDate = DateTime.Now,
                };
                db.Examinees.Add(examinee);
                db.SaveChanges();
            }
            Session session = db.Sessions.Where(x => x.ExamineeId == examinee.Id
            && x.ExamId == model.ExamId
            && x.TokenExpireTime > DateTime.Now).OrderByDescending(r => r.Id).FirstOrDefault();
            if (session != null && resume)
            {
                Session["TOKEN"] = session.Token;
                Session["TOKENEXPIRE"] = session.TokenExpireTime;
            }
            else
            {
                Exam exam = db.Exams.Where(x => x.IsActive && x.Id == model.ExamId).FirstOrDefault();
                if (exam != null)
                {
                    Session newSession = new Session()
                    {
                        RegistrationDate = DateTime.Now,
                        ExamId = model.ExamId,
                        Token = Guid.NewGuid(),
                        TokenExpireTime = DateTime.Now.AddMinutes(exam.DurationInMinute)
                    };
                    examinee.Sessions.Add(newSession);
                    db.Sessions.Add(newSession);
                    db.SaveChanges();
                    Session["TOKEN"] = newSession.Token;
                    Session["TOKENEXPIRE"] = newSession.TokenExpireTime;
                }
            }
            return RedirectToAction("ExamPaper", new { @token = Session["TOKEN"] });
        }

        public ActionResult ExamPaper(Guid token, int? page)
        {
            var qno = page;
            if (token == null)
            {
                TempData["message"] = "You have an invalid token. Please re-register and try again";
                return RedirectToAction("Init");
            }
            var session = db.Sessions.Where(x => x.Token.Equals(token)).FirstOrDefault();
            if (session == null)
            {
                TempData["message"] = "This token is invalid";
                return RedirectToAction("Init");
            }
            if (session.TokenExpireTime < DateTime.Now)
            {
                TempData["message"] = "The exam duration has expired at " + session.TokenExpireTime.ToString();
                return RedirectToAction("Init");
            }
            if (qno.GetValueOrDefault() < 1)
                qno = 1;
            var examQuestionId = db.ExamQuestions
                .Where(x => x.ExamId == session.ExamId && x.QuestionNumber == qno)
                .Select(x => x.Id).FirstOrDefault();
            if (examQuestionId > 0)
            {
                var examQuestions = db.ExamQuestions.Where(x => x.Id == examQuestionId)
                    .Select(x => new QuestionViewModel()
                    {
                        QuestionType = x.Question.QuestionType,
                        QuestionNumber = x.QuestionNumber,
                        Question = x.Question.Question1,
                        Point = x.Question.Points,
                        ExamId = x.ExamId,
                        ExamName = x.Exam.Name,
                        Options = x.Question.Choices.Where(y => y.IsActive == true).Select(y => new QuestionOptionsViewModel()
                        {
                            ChoiceId = y.Id,
                            Label = y.Label,
                        }).ToList()
                    }).FirstOrDefault();
                var savedAnswers = db.Answers.Where(x => x.ExamQuestionId == examQuestionId && x.SessionId == session.Id && x.Choice.IsActive == true)
                    .Select(x => new { x.ChoiceId, x.Answer1 }).ToList();
                foreach (var savedAnswer in savedAnswers)
                {
                    examQuestions.Options.Where(x => x.ChoiceId == savedAnswer.ChoiceId).FirstOrDefault().Answer = savedAnswer.Answer1;
                }
                examQuestions.TotalQuestionInSet = db.ExamQuestions.Where(x => x.Question.IsActive == true && x.ExamId == session.ExamId).Count();
                ViewBag.TimeExpire = session.TokenExpireTime;
                var products = db.ExamQuestions
                .Where(x => x.ExamId == session.ExamId).OrderBy(x => x.QuestionNumber)
                .Select(x => x.QuestionNumber);

                var pageNumber = qno ?? 1;
                var onePageOfQuestion = products.ToPagedList(pageNumber, 1);

                ViewBag.OnePageOfQuestion = onePageOfQuestion;

                return View(examQuestions);
            }
            else
            {
                return View("Error");
            }
        }
        public ActionResult ExamPaperSimulation(Guid token, int? page)
        {
            var qno = page;
            if (token == null)
            {
                TempData["message"] = "You have an invalid token. Please re-register and try again";
                return RedirectToAction("Init");
            }
            var session = db.Sessions.Where(x => x.Token.Equals(token)).FirstOrDefault();
            if (session == null)
            {
                TempData["message"] = "This token is invalid";
                return RedirectToAction("Init");
            }
            if (session.TokenExpireTime < DateTime.Now)
            {
                TempData["message"] = "The exam duration has expired at " + session.TokenExpireTime.ToString();
                return RedirectToAction("Init");
            }
            if (qno.GetValueOrDefault() < 1)
                qno = 1;
            var examQuestionId = db.ExamQuestions
                .Where(x => x.ExamId == session.ExamId && x.QuestionNumber == qno)
                .Select(x => x.Id).FirstOrDefault();
            if (examQuestionId > 0)
            {
                var examQuestions = db.ExamQuestions.Where(x => x.Id == examQuestionId)
                    .Select(x => new QuestionViewModel()
                    {
                        QuestionType = x.Question.QuestionType,
                        QuestionNumber = x.QuestionNumber,
                        Question = x.Question.Question1,
                        Point = x.Question.Points,
                        ExamId = x.ExamId,
                        ExamName = x.Exam.Name,
                        Options = x.Question.Choices.Where(y => y.IsActive == true).Select(y => new QuestionOptionsViewModel()
                        {
                            ChoiceId = y.Id,
                            Label = y.Label,
                        }).ToList()
                    }).FirstOrDefault();
                var savedAnswers = db.Answers.Where(x => x.ExamQuestionId == examQuestionId && x.SessionId == session.Id && x.Choice.IsActive == true)
                    .Select(x => new { x.ChoiceId, x.Answer1 }).ToList();
                foreach (var savedAnswer in savedAnswers)
                {
                    examQuestions.Options.Where(x => x.ChoiceId == savedAnswer.ChoiceId).FirstOrDefault().Answer = savedAnswer.Answer1;
                }
                examQuestions.TotalQuestionInSet = db.ExamQuestions.Where(x => x.Question.IsActive == true && x.ExamId == session.ExamId).Count();
                ViewBag.TimeExpire = session.TokenExpireTime;
                var products = db.ExamQuestions
                .Where(x => x.ExamId == session.ExamId).OrderBy(x => x.QuestionNumber)
                .Select(x => x.QuestionNumber);

                var pageNumber = qno ?? 1;
                var onePageOfQuestion = products.ToPagedList(pageNumber, 1);

                ViewBag.OnePageOfQuestion = onePageOfQuestion;

                return View(examQuestions);
            }
            else
            {
                return View("Error");
            }
        }
        [ValidateInput(false)]
        [HttpPost]
        public ActionResult PostAnswer(AnswerViewModel choices)
        {
            var session = db.Sessions.Where(x => x.Token.Equals(choices.Token)).FirstOrDefault();
            if (session == null)
            {
                TempData["message"] = "This token is invalid";
                return RedirectToAction("Init");
            }
            if (session.TokenExpireTime < DateTime.Now)
            {
                TempData["message"] = "The exam duration has expired at " + session.TokenExpireTime.ToString();
                return RedirectToAction("Init");
            }
            var testQuestionInfo = db.ExamQuestions.Where(x => x.ExamId == session.ExamId
            && x.QuestionNumber == choices.QuestionId)
            .Select(x => new
            {
                TQId = x.Id,
                QT = x.Question.QuestionType,
                QID = x.Id,
                POINT = (decimal)x.Question.Points
            }).FirstOrDefault();
            if (testQuestionInfo != null)
            {
                if (choices.UserChoices.Count > 1)
                {
                    var allPointValueOfChoices =
                        (
                            from a in db.Choices.Where(x => x.IsActive)
                            join b in choices.UserSelectedId on a.Id equals b
                            select new { a.Id, Points = (decimal)a.Points }).AsEnumerable()
                            .Select(x => new Answer()
                            {
                                SessionId = session.Id,
                                ExamQuestionId = testQuestionInfo.QID,
                                ChoiceId = x.Id,
                                Answer1 = "CHECKED",
                                MarkScored = Math.Floor((testQuestionInfo.POINT / 100.00M) * x.Points)
                            }
                        ).ToList();
                    var oldChoices = db.Answers.Where(p => p.ExamQuestionId == choices.QuestionId && p.SessionId == session.Id).ToList();
                    db.Answers.RemoveRange(oldChoices);
                    db.Answers.AddRange(allPointValueOfChoices);
                }
                else
                {
                    //the answer is of type TEXT
                    db.Answers.Add(new Answer()
                    {
                        SessionId = session.Id,
                        ExamQuestionId = testQuestionInfo.QID,
                        ChoiceId = choices.UserChoices.FirstOrDefault().ChoiceId,
                        MarkScored = 1,
                        Answer1 = choices.Answer
                    });
                }
                db.SaveChanges();
            }
            var nextQuestionNumber = 1;
            if (choices.Direction.Equals("forward", StringComparison.CurrentCultureIgnoreCase))
            {
                nextQuestionNumber = db.ExamQuestions.Where(x => x.ExamId == choices.ExamId
                && x.QuestionNumber > choices.QuestionId)
                .OrderBy(x => x.QuestionNumber).Take(1).Select(x => x.QuestionNumber).FirstOrDefault();
            }
            else
            {
                nextQuestionNumber = db.ExamQuestions.Where(x => x.ExamId == choices.ExamId
                && x.QuestionNumber < choices.QuestionId)
                .OrderByDescending(x => x.QuestionNumber).Take(1).Select(x => x.QuestionNumber).FirstOrDefault();
            }
            if (nextQuestionNumber < 1)
                nextQuestionNumber = 1;
            return RedirectToAction("ExamPaper", new
            {
                @token = Session["TOKEN"],
                @page = nextQuestionNumber
            });
        }
    }
}