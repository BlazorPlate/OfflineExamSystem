namespace OfflineExamSystem.Areas.Examination.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    using Resources;
    public partial class Answer
    {
        [Display(Name = "Id", ResourceType = typeof(Resources))]
        public int Id { get; set; }
        [Display(Name = "Session", ResourceType = typeof(Resources))]
        public int SessionId { get; set; }
        [Display(Name = "ExamQuestion", ResourceType = typeof(Resources))]
        public int ExamQuestionId { get; set; }
        [Display(Name = "Choice", ResourceType = typeof(Resources))]
        public int ChoiceId { get; set; }
        [Column("Answer")]
        [Display(Name = "Answer", ResourceType = typeof(Resources))]
        public string Answer1 { get; set; }
        [Display(Name = "MarkScored", ResourceType = typeof(Resources))]
        public decimal? MarkScored { get; set; }

        public virtual Choice Choice { get; set; }

        public virtual ExamQuestion ExamQuestion { get; set; }

        public virtual Session Session { get; set; }
    }
}
