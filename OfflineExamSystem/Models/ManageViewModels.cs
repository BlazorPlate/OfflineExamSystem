using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using Resources;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OfflineExamSystem.Models
{
    public class IndexViewModel
    {
        #region Public Properties
        public bool HasPassword { get; set; }
        public IList<UserLoginInfo> Logins { get; set; }
        public string PhoneNumber { get; set; }
        public bool TwoFactor { get; set; }
        public bool BrowserRemembered { get; set; }
        #endregion Public Properties
    }

    public class ManageLoginsViewModel
    {
        #region Public Properties
        public IList<UserLoginInfo> CurrentLogins { get; set; }
        public IList<AuthenticationDescription> OtherLogins { get; set; }
        #endregion Public Properties
    }

    public class FactorViewModel
    {
        #region Public Properties
        public string Purpose { get; set; }
        #endregion Public Properties
    }

    public class SetPasswordViewModel
    {
        #region Public Properties
        [Required(ErrorMessageResourceType = typeof(ValidationResources), ErrorMessageResourceName = "PropertyValueRequired")]
        [StringLength(100, ErrorMessageResourceType = typeof(Resources.Resources), ErrorMessageResourceName = "StringLength", MinimumLength = 4)]
        [DataType(System.ComponentModel.DataAnnotations.DataType.Password)]
        [Display(Name = "NewPassword", ResourceType = typeof(Resources.Resources))]
        public string NewPassword { get; set; }

        [DataType(System.ComponentModel.DataAnnotations.DataType.Password)]
        [Display(Name = "ConfirmNewPassword", ResourceType = typeof(Resources.Resources))]
        [Compare("NewPassword", ErrorMessageResourceType = typeof(Resources.Resources), ErrorMessageResourceName = "PasswordMatch")]
        public string ConfirmPassword { get; set; }
        #endregion Public Properties
    }

    public class ChangePasswordViewModel
    {
        #region Public Properties
        [Required(ErrorMessageResourceType = typeof(ValidationResources), ErrorMessageResourceName = "PropertyValueRequired")]
        [DataType(System.ComponentModel.DataAnnotations.DataType.Password)]
        [Display(Name = "CurrentPassword", ResourceType = typeof(Resources.Resources))]
        public string OldPassword { get; set; }

        [Required(ErrorMessageResourceType = typeof(ValidationResources), ErrorMessageResourceName = "PropertyValueRequired")]
        [StringLength(100, ErrorMessageResourceType = typeof(Resources.Resources), ErrorMessageResourceName = "StringLength", MinimumLength = 4)]
        [DataType(System.ComponentModel.DataAnnotations.DataType.Password)]
        [Display(Name = "NewPassword", ResourceType = typeof(Resources.Resources))]
        public string NewPassword { get; set; }

        [DataType(System.ComponentModel.DataAnnotations.DataType.Password)]
        [Display(Name = "ConfirmNewPassword", ResourceType = typeof(Resources.Resources))]
        [Compare("NewPassword", ErrorMessageResourceType = typeof(Resources.Resources), ErrorMessageResourceName = "PasswordMatch")]
        public string ConfirmPassword { get; set; }
        #endregion Public Properties
    }

    public class AddPhoneNumberViewModel
    {
        #region Public Properties
        [Required(ErrorMessageResourceType = typeof(ValidationResources), ErrorMessageResourceName = "PropertyValueRequired")]
        [Phone]
        [Display(Name = "PhoneNumber", ResourceType = typeof(Resources.Resources))]
        public string Number { get; set; }
        #endregion Public Properties
    }

    public class VerifyPhoneNumberViewModel
    {
        #region Public Properties
        [Required(ErrorMessageResourceType = typeof(ValidationResources), ErrorMessageResourceName = "PropertyValueRequired")]
        [Display(Name = "PhoneNumber", ResourceType = typeof(Resources.Resources))]
        public string Code { get; set; }

        [Required(ErrorMessageResourceType = typeof(ValidationResources), ErrorMessageResourceName = "PropertyValueRequired")]
        [Phone]
        [Display(Name = "PhoneNumber", ResourceType = typeof(Resources.Resources))]
        public string PhoneNumber { get; set; }
        #endregion Public Properties
    }

    public class ConfigureTwoFactorViewModel
    {
        #region Public Properties
        public string SelectedProvider { get; set; }
        public ICollection<System.Web.Mvc.SelectListItem> Providers { get; set; }
        #endregion Public Properties
    }
}