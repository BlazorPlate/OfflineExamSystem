namespace OfflineExamSystem.Areas.Examination.Models
{
    using Resources;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Question
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Question()
        {
            Choices = new HashSet<Choice>();
            ExamQuestions = new HashSet<ExamQuestion>();
        }
        [Display(Name = "Id", ResourceType = typeof(Resources))]
        public int Id { get; set; }
        [Required(ErrorMessageResourceType = typeof(ValidationResources), ErrorMessageResourceName = "PropertyValueRequired")]
        [Display(Name = "Category", ResourceType = typeof(Resources))]
        public int CategoryId { get; set; }
        [Required(ErrorMessageResourceType = typeof(ValidationResources), ErrorMessageResourceName = "PropertyValueRequired")]
        [Display(Name = "QuestionType", ResourceType = typeof(Resources))]
        public string QuestionType { get; set; }
        [Required(ErrorMessageResourceType = typeof(ValidationResources), ErrorMessageResourceName = "PropertyValueRequired")]
        [Display(Name = "Question_En", ResourceType = typeof(Resources))]
        public string Question_En { get; set; }
        [Required(ErrorMessageResourceType = typeof(ValidationResources), ErrorMessageResourceName = "PropertyValueRequired")]
        [Display(Name = "Question_Ar", ResourceType = typeof(Resources))]
        public string Question_Ar { get; set; }
        [Required(ErrorMessageResourceType = typeof(ValidationResources), ErrorMessageResourceName = "PropertyValueRequired")]
        [Display(Name = "CorrectHint_En", ResourceType = typeof(Resources))]
        public string CorrectHint_En { get; set; }
        [Display(Name = "CorrectHint_Ar", ResourceType = typeof(Resources))]
        public string CorrectHint_Ar { get; set; }
        [Display(Name = "WrongHint_En", ResourceType = typeof(Resources))]
        public string WrongHint_En { get; set; }
        [Display(Name = "WrongHint_Ar", ResourceType = typeof(Resources))]
        public string WrongHint_Ar { get; set; }
        [Required(ErrorMessageResourceType = typeof(ValidationResources), ErrorMessageResourceName = "PropertyValueRequired")]
        [Display(Name = "Points", ResourceType = typeof(Resources))]
        public int Points { get; set; }
        [Display(Name = "IsActive", ResourceType = typeof(Resources))]
        public bool IsActive { get; set; }
        public virtual Category Category { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Choice> Choices { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ExamQuestion> ExamQuestions { get; set; }
    }
}
