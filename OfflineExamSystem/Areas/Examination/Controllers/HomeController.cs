using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Newtonsoft.Json;
using OfflineExamSystem.Areas.Examination.Models;
using OfflineExamSystem.Areas.Examination.ViewModels;
using OfflineExamSystem.Helpers;
using OfflineExamSystem.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace OfflineExamSystem.Areas.Examination.Controllers
{
    public class Result
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }
    public class HomeController : BaseController
    {
        private List<object> examsList = new List<object>();
        private OfflineExamDBEntities db = new OfflineExamDBEntities();
        private ApplicationUserManager _userManager;
        private IAuthenticationManager AuthenticationManager => HttpContext.GetOwinContext().Authentication;
        public ApplicationUserManager UserManager
        {
            get => _userManager ?? HttpContext.GetOwinContext()
                    .GetUserManager<ApplicationUserManager>();
            private set => _userManager = value;
        }
        public ActionResult Index()
        {
            return View();
        }
        public JsonResult GetExamsByExamMode(bool examMode)
        {
            IOrderedQueryable<Exam> exams = db.Exams.Where(c => c.ExamType == examMode).OrderBy(c => c.Name_En); ;
            foreach (Exam item in exams)
            {
                examsList.Add(new
                {
                    Id = item.Id,
                    Name = Thread.CurrentThread.CurrentCulture.Name.Equals("en-US") ? item.Name_En : item.Name_En
                });
            }
            return Json(new SelectList(examsList, "Id", "Name", Resources.Resources.ChooseExamType));
        }
        public JsonResult GetHintByQuestionId(int questionId, int[] answers)
        {
            string hint = string.Empty;
            Result result = new Result();
            if (answers != null)
            {
                GetHint(questionId, answers, out Question question, out decimal mark);
                if (mark == 1)
                {
                    result.Key = "true";
                    result.Value = Thread.CurrentThread.CurrentCulture.Name.Equals("en-US") ? question.CorrectHint_En : question.CorrectHint_Ar;

                }
                else
                {
                    result.Key = "false";
                    result.Value = Thread.CurrentThread.CurrentCulture.Name.Equals("en-US") ? question.WrongHint_En : question.WrongHint_Ar;
                }
            }
            else
            {
                result.Key = "na";
                result.Value = "Please choose answer to validate.";
            }

            return Json(JsonConvert.SerializeObject(result));
            //return Json(correctHint);
        }

        private void GetHint(int questionId, int[] answers, out Question question, out decimal mark)
        {
            int[] sortedAnswers = answers.OrderBy(i => i).ToArray();
            question = db.Questions.Find(questionId);
            List<Choice> questionChoices = question.Choices.Where(c => c.Points > 0).OrderBy(c => c.Id).ToList();
            mark = 0;
            for (int i = 0; i < sortedAnswers.Count(); i++)
            {
                if (i < questionChoices.Count)
                {
                    if (questionChoices[i].Points > 0 && questionChoices[i].Id == answers[i])
                    {
                        mark = 1;
                    }
                    else
                    {
                        mark = 0;
                    }
                }
                else
                {
                    mark = 0;
                }
            }
        }

        public ActionResult Init()
        {
            if (Thread.CurrentThread.CurrentCulture.Name.Equals("en-US"))
            {
                ViewBag.ExamId = new SelectList(db.Exams.Where(x => x.IsActive == true).Select(x => new { x.Id, x.Name_En }), "Id", "Name_En", "--Choose exam type--");
            }
            else
            {
                ViewBag.ExamId = new SelectList(db.Exams.Where(x => x.IsActive == true).Select(x => new { x.Id, x.Name_Ar }), "Id", "Name_Ar", "--Choose exam type--");
            }
            SessionViewModel sessionViewModel = null;
            if (Session["SessionViewModel"] == null)
            {
                sessionViewModel = new SessionViewModel();
            }
            else
            {
                sessionViewModel = (SessionViewModel)Session["SessionViewModel"];
            }

            return View(sessionViewModel);
        }
        public ActionResult Router(string ExamMode, SessionViewModel sessionViewModel)
        {
            if (ExamMode.Equals("false"))
            {
                return RedirectToAction("InstructionSimulation", "Home", new { ExamId = sessionViewModel.ExamId });
            }
            else
            {
                return RedirectToAction("Instruction", "Home", new { ExamId = sessionViewModel.ExamId });
            }
        }
        public ActionResult InstructionSimulation(int ExamId)
        {
            SessionViewModel sessionViewModel = new SessionViewModel
            {
                ExamId = ExamId
            };
            if (sessionViewModel != null)
            {
                Exam test = db.Exams.Where(x => x.IsActive == true && x.Id == sessionViewModel.ExamId).FirstOrDefault();
                if (test != null)
                {
                    ViewBag.ExamName = Thread.CurrentThread.CurrentCulture.Name.Equals("en-US") ? test.Name_En : test.Name_Ar;
                    ViewBag.ExamDescription = Thread.CurrentThread.CurrentCulture.Name.Equals("en-US") ? test.Description_En : test.Description_Ar;
                    ViewBag.QuestionCount = test.ExamQuestions.Count - 1;
                    ViewBag.ExamDuration = test.DurationInMinute;
                }
            }
            return View(sessionViewModel);
        }
        [LocalizedAuthorize(Roles = "Managerial_Examinee,Technician_Examinee")]
        public ActionResult Instruction(int ExamId)
        {
            SessionViewModel sessionViewModel = new SessionViewModel();
            ApplicationUser currentUser = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(System.Web.HttpContext.Current.User.Identity.GetUserId());
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
                Exam test = db.Exams.Where(x => x.IsActive == true && x.Id == sessionViewModel.ExamId).FirstOrDefault();
                if (test != null)
                {
                    ViewBag.ExamName = Thread.CurrentThread.CurrentCulture.Name.Equals("en-US") ? test.Name_En : test.Name_Ar;
                    ViewBag.ExamDescription = Thread.CurrentThread.CurrentCulture.Name.Equals("en-US") ? test.Description_En : test.Description_Ar;
                    ViewBag.QuestionCount = test.ExamQuestions.Count - 1;
                    ViewBag.ExamDuration = test.DurationInMinute;
                }
            }
            return View(sessionViewModel);
        }
        public ActionResult Register(SessionViewModel model, bool resume = false)
        {
            if (model != null)
            {
                Session["SessionViewModel"] = model;
            }

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
        public ActionResult RegisterSimulation(SessionViewModel model, bool resume = false)
        {
            if (model != null)
            {
                Session["SessionViewModel"] = model;
            }

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
                foreach (DbEntityValidationResult eve in e.EntityValidationErrors)
                {
                    Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (DbValidationError ve in eve.ValidationErrors)
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
        public ActionResult ExamPaper(Guid token, int? page)
        {
            int? qno = page;
            if (token == null)
            {
                TempData["message"] = "You have an invalid token. Please re-register and try again";
                return RedirectToAction("Init");
            }
            Session session = db.Sessions.Where(x => x.Token.Equals(token)).FirstOrDefault();
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
            {
                qno = 1;
            }

            int examQuestionId = db.ExamQuestions
                .Where(e => e.ExamId == session.ExamId && e.QuestionNumber == qno)
                .Select(e => e.Id).FirstOrDefault();
            if (examQuestionId > 0)
            {
                QuestionViewModel examQuestions =

                    db.ExamQuestions.Where(e => e.Id == examQuestionId)
                    .Select(e => new QuestionViewModel()
                    {
                        QuestionType = e.Question.QuestionType,
                        QuestionNumber = e.QuestionNumber,
                        Question = Thread.CurrentThread.CurrentCulture.Name.Equals("en-US") ? e.Question.Question_En : e.Question.Question_Ar,
                        Point = e.Question.Points,
                        ExamId = e.ExamId,
                        ExamName = Thread.CurrentThread.CurrentCulture.Name.Equals("en-US") ? e.Exam.Name_En : e.Exam.Name_Ar,
                        Options = e.Question.Choices.Where(y => y.IsActive == true).Select(c => new QuestionOptionsViewModel()
                        {
                            ChoiceId = c.Id,
                            Label = Thread.CurrentThread.CurrentCulture.Name.Equals("en-US") ? c.Label_En : c.Label_Ar,
                        }).ToList()
                    }).FirstOrDefault();
                var savedAnswers = db.Answers.Where(a => a.ExamQuestionId == examQuestionId && a.SessionId == session.Id && a.Choice.IsActive == true)
                    .Select(a => new { a.ChoiceId, a.Answer1 }).ToList();
                foreach (var savedAnswer in savedAnswers)
                {
                    examQuestions.Options.Where(x => x.ChoiceId == savedAnswer.ChoiceId).FirstOrDefault().Answer = savedAnswer.Answer1;
                }
                examQuestions.TotalQuestionInSet = db.ExamQuestions.Where(q => q.Question.IsActive == true && q.ExamId == session.ExamId).Count();
                ViewBag.TimeExpire = session.TokenExpireTime;
                IQueryable<int> questions = db.ExamQuestions
                .Where(q => q.ExamId == session.ExamId).OrderBy(q => q.QuestionNumber)
                .Select(q => q.QuestionNumber);

                int pageNumber = qno ?? 1;
                IPagedList<int> onePageOfQuestion = questions.ToPagedList(pageNumber, 1);

                ViewBag.OnePagePerQuestion = onePageOfQuestion;

                return View(examQuestions);
            }
            else
            {
                return View("Error");
            }
        }
        public ActionResult ExamPaperSimulation(Guid token, int? page)
        {
            int? qno = page;
            if (token == null)
            {
                TempData["message"] = "You have an invalid token. Please re-register and try again";
                return RedirectToAction("Init");
            }
            Session session = db.Sessions.Where(s => s.Token.Equals(token)).FirstOrDefault();
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
            {
                qno = 1;
            }

            int examQuestionId = db.ExamQuestions
                .Where(e => e.ExamId == session.ExamId && e.QuestionNumber == qno)
                .Select(e => e.Id).FirstOrDefault();
            if (examQuestionId > 0)
            {
                QuestionViewModel examQuestions = db.ExamQuestions.Where(e => e.Id == examQuestionId)
                    .Select(e => new QuestionViewModel()
                    {
                        QuestionType = e.Question.QuestionType,
                        QuestionNumber = e.QuestionNumber,
                        Question = Thread.CurrentThread.CurrentCulture.Name.Equals("en-US") ? e.Question.Question_En : e.Question.Question_Ar,
                        Point = e.Question.Points,
                        ExamId = e.ExamId,
                        ExamName = Thread.CurrentThread.CurrentCulture.Name.Equals("en-US") ? e.Exam.Name_En : e.Exam.Name_Ar,
                        Options = e.Question.Choices.Where(y => y.IsActive == true).Select(c => new QuestionOptionsViewModel()
                        {
                            ChoiceId = c.Id,
                            Label = Thread.CurrentThread.CurrentCulture.Name.Equals("en-US") ? c.Label_En : c.Label_Ar,
                        }).ToList()
                    }).FirstOrDefault();
                var savedAnswers = db.Answers.Where(a => a.ExamQuestionId == examQuestionId && a.SessionId == session.Id && a.Choice.IsActive == true)
                    .Select(a => new { a.ChoiceId, a.Answer1 }).ToList();
                foreach (var savedAnswer in savedAnswers)
                {
                    examQuestions.Options.Where(e => e.ChoiceId == savedAnswer.ChoiceId).FirstOrDefault().Answer = savedAnswer.Answer1;
                }
                examQuestions.TotalQuestionInSet = db.ExamQuestions.Where(e => e.Question.IsActive == true && e.ExamId == session.ExamId).Count();
                ViewBag.TimeExpire = session.TokenExpireTime;
                IQueryable<int> questions = db.ExamQuestions.Where(e => e.ExamId == session.ExamId).OrderBy(e => e.QuestionNumber).Select(e => e.QuestionNumber);
                int pageNumber = qno ?? 1;
                IPagedList<int> onePagePerQuestion = questions.ToPagedList(pageNumber, 1);
                ViewBag.OnePagePerQuestion = onePagePerQuestion;
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
            Session session = db.Sessions.Where(x => x.Token.Equals(choices.Token)).FirstOrDefault();
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
                    List<Answer> allPointValueOfChoices =
                        (
                            from a in db.Choices.Where(x => x.IsActive)
                            join b in choices.UserSelectedId on a.Id equals b
                            select new { a.Id, Points = a.Points }).AsEnumerable()
                            .Select(x => new Answer()
                            {
                                SessionId = session.Id,
                                ExamQuestionId = testQuestionInfo.QID,
                                ChoiceId = x.Id,
                                Answer1 = "CHECKED",
                                MarkScored = Math.Floor((testQuestionInfo.POINT / 100.00M) * x.Points)
                            }
                        ).ToList();
                    List<Answer> oldChoices = db.Answers.Where(p => p.ExamQuestionId == choices.QuestionId && p.SessionId == session.Id).ToList();
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
            int nextQuestionNumber = 1;
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
            {
                nextQuestionNumber = 1;
            }

            return RedirectToAction("ExamPaper", new
            {
                @token = Session["TOKEN"],
                @page = nextQuestionNumber
            });
        }
        [ValidateInput(false)]
        [HttpPost]
        public ActionResult PostAnswerSimulation(AnswerViewModel choices)
        {
            Session session = db.Sessions.Where(x => x.Token.Equals(choices.Token)).FirstOrDefault();
            if (session == null)
            {
                TempData["message"] = Resources.Resources.InvalidToken;
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
                    List<Answer> allPointValueOfChoices =
                        (
                            from a in db.Choices.Where(x => x.IsActive)
                            join b in choices.UserSelectedId on a.Id equals b
                            select new { a.Id, Points = a.Points }).AsEnumerable()
                            .Select(x => new Answer()
                            {
                                SessionId = session.Id,
                                ExamQuestionId = testQuestionInfo.QID,
                                ChoiceId = x.Id,
                                Answer1 = "CHECKED",
                                MarkScored = Math.Floor((testQuestionInfo.POINT / 100.00M) * x.Points)
                            }
                        ).ToList();
                    List<Answer> oldChoices = db.Answers.Where(p => p.ExamQuestionId == choices.QuestionId && p.SessionId == session.Id).ToList();
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
            int nextQuestionNumber = 1;
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
            {
                nextQuestionNumber = 1;
            }

            return RedirectToAction("ExamPaperSimulation", new
            {
                @token = Session["TOKEN"],
                @page = nextQuestionNumber
            });
        }

        public ActionResult Completion(bool hasExpired, Guid token)
        {
            CalculateFinalMark(token, out IQueryable<ExamQuestion> questions, out int sum);
            if (hasExpired)
            {
                ViewBag.Status = "Your exam time has expired.";
                ApplicationUser user = UserManager.FindById(System.Web.HttpContext.Current.User.Identity.GetUserId());
                user.IsEnabled = false;
                UserManager.Update(user);
                AuthenticationManager.SignOut();
            }
            else
            {
                ViewBag.Status = "You have finished your exam";
            }
            ViewBag.TotalMark = sum;
            ViewBag.TotalQuestions = questions.Count() - 1;
            return View();
        }

        private void CalculateFinalMark(Guid token, out IQueryable<ExamQuestion> questions, out int sum)
        {
            Session session = db.Sessions.Where(s => s.Token == token).FirstOrDefault();
            int examId = session.ExamId;
            questions = db.ExamQuestions.Where(q => q.ExamId == examId);
            List<decimal> points = new List<decimal>();
            foreach (ExamQuestion question in questions)
            {
                IQueryable<Answer> answers = db.Answers.Where(a => a.SessionId == session.Id && a.ExamQuestionId == question.QuestionId);
                decimal mark = 1;
                if (answers.Count() > 0)
                {
                    foreach (Answer answer in answers)
                    {
                        mark = answer.MarkScored.Value * mark;
                    }
                }
                else
                {
                    mark = 0;
                }
                points.Add(mark);
            }
            sum = (int)points.Sum();
        }
    }
}