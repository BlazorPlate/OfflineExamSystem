namespace OfflineExamSystem.Areas.Examination.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    using Resources;
    public partial class Session
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Session()
        {
            Answers = new HashSet<Answer>();
        }
        [Display(Name = "Id", ResourceType = typeof(Resources))]
        public int Id { get; set; }
        [Display(Name = "Examinee", ResourceType = typeof(Resources))]
        public int ExamineeId { get; set; }
        [Display(Name = "Exam", ResourceType = typeof(Resources))]
        public int ExamId { get; set; }
        [Display(Name = "RegistrationDate", ResourceType = typeof(Resources))]
        public DateTime RegistrationDate { get; set; }
        [Display(Name = "Token", ResourceType = typeof(Resources))]
        public Guid Token { get; set; }
        [Display(Name = "TokenExpireTime", ResourceType = typeof(Resources))]
        public DateTime TokenExpireTime { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Answer> Answers { get; set; }

        public virtual Examinee Examinee { get; set; }

        public virtual Exam Exam { get; set; }
    }
}
