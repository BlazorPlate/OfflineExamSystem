namespace OfflineExamSystem.Areas.Examination.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    using Resources;
    public partial class ExamQuestion
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ExamQuestion()
        {
            Answers = new HashSet<Answer>();
        }
        [Display(Name = "Id", ResourceType = typeof(Resources))]

        public int Id { get; set; }
        [Display(Name = "Exam", ResourceType = typeof(Resources))]

        public int ExamId { get; set; }
        [Display(Name = "Question", ResourceType = typeof(Resources))]

        public int QuestionId { get; set; }
        [Display(Name = "QuestionNumber", ResourceType = typeof(Resources))]

        public int QuestionNumber { get; set; }
        [Display(Name = "IsActive", ResourceType = typeof(Resources))]

        public bool IsActive { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Answer> Answers { get; set; }

        public virtual Exam Exam { get; set; }

        public virtual Question Question { get; set; }
    }
}
