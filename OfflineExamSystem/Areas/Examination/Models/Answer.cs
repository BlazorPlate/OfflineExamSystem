namespace OfflineExamSystem.Areas.Examination.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Answer
    {
        public int Id { get; set; }

        public int SessionId { get; set; }

        public int ExamQuestionId { get; set; }

        public int ChoiceId { get; set; }

        [Column("Answer")]
        public string Answer1 { get; set; }

        public decimal? MarkScored { get; set; }

        public virtual Choice Choice { get; set; }

        public virtual ExamQuestion ExamQuestion { get; set; }

        public virtual Session Session { get; set; }
    }
}
