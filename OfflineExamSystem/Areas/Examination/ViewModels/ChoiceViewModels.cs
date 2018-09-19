using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OfflineExamSystem.Areas.Examination.ViewModels
{
    public class ChoiceViewModel
    {
        public int ChoiceId { get; set; }
        public string IsChecked { get; set; }

    }

    public class AnswerViewModel
    {
        public int ExamId { get; set; }
        public int QuestionId { get; set; }
        public Guid Token { get; set; }
        public List<ChoiceViewModel> UserChoices { get; set; }
        public string Answer { get; set; }
        public string Direction { get; set; }

        public List<int> UserSelectedId
        {
            get
            {
                return UserChoices == null ? new List<int>() :
                    UserChoices.Where(x => x.IsChecked == "on" || "true".Equals(x.IsChecked, StringComparison.InvariantCultureIgnoreCase)).Select(x => x.ChoiceId).ToList();
            }
        }
    }
}