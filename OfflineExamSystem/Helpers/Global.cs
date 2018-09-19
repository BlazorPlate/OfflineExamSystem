using System.Web;

namespace OfflineExamSystem.Helpers
{
    public class Tenant
    {
        #region Public Properties
        public static string TenantId { get; set; }
        #endregion Public Properties

        #region Public Methods
        public static string GetCurrentTenantId()
        {
            var fullAddress = HttpContext.Current.Request.Headers["Host"].Split('.');
            if (fullAddress.Length < 2)
                throw new HttpException(400, "Bad Request");
            TenantId = fullAddress[0];
            //TenantId = "services";
            return TenantId;
        }
        #endregion Public Methods
    }
}