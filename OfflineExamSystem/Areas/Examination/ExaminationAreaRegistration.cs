using OfflineExamSystem.Helpers;
using System.Web.Mvc;

namespace OfflineExamSystem.Areas.Examination
{
    public class ExaminationAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Examination";
            }
        }
        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "Exam_default",
                "{culture}/Examination/{controller}/{action}/{id}",
                new { culture = CultureHelper.GetDefaultCulture(), action = "Index", id = UrlParameter.Optional },
                namespaces: new[] { "OfflineExamSystem.Areas.Examination.Controllers" }
            );
        }
    }
}