namespace OfflineExamSystem.Areas.Examination.Models
{
    using Resources;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Exam
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Exam()
        {
            ExamQuestions = new HashSet<ExamQuestion>();
            Sessions = new HashSet<Session>();
        }
        [Display(Name = "Id", ResourceType = typeof(Resources))]
        public int Id { get; set; }
        [Required(ErrorMessageResourceType = typeof(ValidationResources), ErrorMessageResourceName = "PropertyValueRequired")]
        [Display(Name = "Name_En", ResourceType = typeof(Resources))]
        public string Name_En { get; set; }
        [Required(ErrorMessageResourceType = typeof(ValidationResources), ErrorMessageResourceName = "PropertyValueRequired")]
        [Display(Name = "Description_En", ResourceType = typeof(Resources))]
        public string Description_En { get; set; }
        [Required(ErrorMessageResourceType = typeof(ValidationResources), ErrorMessageResourceName = "PropertyValueRequired")]
        [Display(Name = "Name_Ar", ResourceType = typeof(Resources))]
        public string Name_Ar { get; set; }
        [Required(ErrorMessageResourceType = typeof(ValidationResources), ErrorMessageResourceName = "PropertyValueRequired")]
        [Display(Name = "Description_Ar", ResourceType = typeof(Resources))]
        public string Description_Ar { get; set; }
        [Display(Name = "ExamType", ResourceType = typeof(Resources))]
        public bool ExamType { get; set; }
        [Display(Name = "IsActive", ResourceType = typeof(Resources))]
        public bool IsActive { get; set; }
        [Display(Name = "DurationInMinute", ResourceType = typeof(Resources))]
        public int DurationInMinute { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ExamQuestion> ExamQuestions { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Session> Sessions { get; set; }
    }
}
