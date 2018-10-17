namespace OfflineExamSystem.Areas.Examination.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Coordinate
    {
        public int Id { get; set; }

        [Column("Coordinate")]
        [Required]
        public string Coordinate1 { get; set; }
        public string Shape { get; set; }
        public bool AnswerFlag { get; set; }
        public int ImageId { get; set; }
        public virtual Image Image { get; set; }
    }
}
