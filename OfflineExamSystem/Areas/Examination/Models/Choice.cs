namespace OfflineExamSystem.Areas.Examination.Models
{
    using Resources;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Choice
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Choice()
        {
            Answers = new HashSet<Answer>();
        }
        [Display(Name = "Id", ResourceType = typeof(Resources))]
        public int Id { get; set; }
        [Required(ErrorMessageResourceType = typeof(ValidationResources), ErrorMessageResourceName = "PropertyValueRequired")]
        [Display(Name = "Question", ResourceType = typeof(Resources))]
        public int QuestionId { get; set; }
        [Required(ErrorMessageResourceType = typeof(ValidationResources), ErrorMessageResourceName = "PropertyValueRequired")]
        [Display(Name = "Label_En", ResourceType = typeof(Resources))]
        public string Label_En { get; set; }
        [Required(ErrorMessageResourceType = typeof(ValidationResources), ErrorMessageResourceName = "PropertyValueRequired")]
        [Display(Name = "Label_Ar", ResourceType = typeof(Resources))]
        public string Label_Ar { get; set; }
        [Display(Name = "Points", ResourceType = typeof(Resources))]
        public decimal Points { get; set; }
        [Display(Name = "IsActive", ResourceType = typeof(Resources))]
        public bool IsActive { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Answer> Answers { get; set; }
        [Display(Name = "Question", ResourceType = typeof(Resources))]
        public virtual Question Question { get; set; }
    }
}
