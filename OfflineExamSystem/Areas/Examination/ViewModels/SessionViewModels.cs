﻿using Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OfflineExamSystem.Areas.Examination.ViewModels
{
    public class SessionViewModel
    {
        [Required(ErrorMessageResourceType = typeof(ValidationResources), ErrorMessageResourceName = "PropertyValueRequired")]
        public int ExamId { get; set; }
        public string FullName_Ar { get; set; }
        public string FullName_En { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        [Required(ErrorMessageResourceType = typeof(ValidationResources), ErrorMessageResourceName = "PropertyValueRequired")]
        public string ExamMode { get; set; }
    }
}