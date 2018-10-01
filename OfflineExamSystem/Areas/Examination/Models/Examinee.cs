namespace OfflineExamSystem.Areas.Examination.Models
{
    using Resources;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Examinee
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Examinee()
        {
            Sessions = new HashSet<Session>();
        }

        public int Id { get; set; }

        [Required(ErrorMessageResourceType = typeof(ValidationResources), ErrorMessageResourceName = "PropertyValueRequired")]
        [Display(Name = "UserName", ResourceType = typeof(Resources))]
        public string UserName { get; set; }
        [Required(ErrorMessageResourceType = typeof(ValidationResources), ErrorMessageResourceName = "PropertyValueRequired")]
        [Display(Name = "FullName_En", ResourceType = typeof(Resources))]
        public string FullName_En { get; set; }
        [Required(ErrorMessageResourceType = typeof(ValidationResources), ErrorMessageResourceName = "PropertyValueRequired")]
        [Display(Name = "FullName_Ar", ResourceType = typeof(Resources))]
        public string FullName_Ar { get; set; }
        [Required(ErrorMessageResourceType = typeof(ValidationResources), ErrorMessageResourceName = "PropertyValueRequired")]
        [Display(Name = "EntryDate", ResourceType = typeof(Resources))]
        public DateTime EntryDate { get; set; }
        [Required(ErrorMessageResourceType = typeof(ValidationResources), ErrorMessageResourceName = "PropertyValueRequired")]
        [Display(Name = "Email", ResourceType = typeof(Resources))]
        public string Email { get; set; }
        [Display(Name = "Phone", ResourceType = typeof(Resources))]
        public string Phone { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Session> Sessions { get; set; }
    }
}
