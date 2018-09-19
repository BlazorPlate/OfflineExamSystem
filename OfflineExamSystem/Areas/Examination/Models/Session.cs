namespace OfflineExamSystem.Areas.Examination.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Session
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Session()
        {
            Answers = new HashSet<Answer>();
        }

        public int Id { get; set; }

        public int ExamineeId { get; set; }

        public int ExamId { get; set; }

        public DateTime RegistrationDate { get; set; }

        public Guid Token { get; set; }

        public DateTime TokenExpireTime { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Answer> Answers { get; set; }

        public virtual Examinee Examinee { get; set; }

        public virtual Exam Exam { get; set; }
    }
}
