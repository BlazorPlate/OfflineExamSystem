namespace OfflineExamSystem.Areas.Examination.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Image
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Image()
        {
            Coordinates = new HashSet<Coordinate>();
        }

        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string Question { get; set; }
        public string Path { get; set; }
        public int PageNum { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Coordinate> Coordinates { get; set; }
    }
}
