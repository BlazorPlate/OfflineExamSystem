using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OfflineExamSystem.Areas.Examination.ViewModels
{
    public class QuestionViewModel
    {
        public int TotalQuestionInSet { get; set; }
        public int QuestionNumber { get; set; }
        public int ExamId { get; set; }
        public string ExamName { get; set; }
        public string Question { get; set; }
        public string QuestionType { get; set; }
        public int Point { get; set; }
        public List<QuestionOptionsViewModel> Options { get; set; }
    }
    public class QuestionOptionsViewModel
    {
        public int ChoiceId { get; set; }
        public string Label { get; set; }

        public string Answer { get; set; }
    }
}