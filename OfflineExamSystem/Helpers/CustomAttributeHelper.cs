using OfflineExamSystem.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;

namespace OfflineExamSystem.Helpers
{
    public class ValidateImageFileAttribute : ValidationAttribute
    {
        #region Public Methods
        public override bool IsValid(object value)
        {
            int MaxContentLength = 10485760; //10 MB
            string[] AllowedFileExtensions = new string[] { ".jpg", ".jpeg", ".bmp", ".png", ".doc", ".docx", ".pdf", ".xls", ".xlsx" };
            var file = value as HttpPostedFileBase;
            if (file == null)
            {
                return true;
            }
            if (file != null && file.ContentLength > 0)
            {
                if (!AllowedFileExtensions.Contains(file.FileName.Substring(file.FileName.LastIndexOf('.')).ToLower()))
                {
                    ErrorMessage = Resources.Resources.PleaseUploadFileOfType + string.Join(", ", AllowedFileExtensions);
                    return false;
                }
                else if (file.ContentLength > MaxContentLength)
                {
                    ErrorMessage = Resources.Resources.MaximumAllowedSize + (MaxContentLength / 1024 / 1024).ToString() + "MB";
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return false;
            }
        }
        #endregion Public Methods
    }

    public class ValidateImageAttribute : ValidationAttribute
    {
        #region Public Methods
        public override bool IsValid(object value)
        {
            int MaxContentLength = 10485760; //10 MB
            string[] AllowedFileExtensions = new string[] { ".jpg", ".jpeg", ".bmp", ".png" };
            var file = value as HttpPostedFileBase;
            if (file == null)
            {
                return true;
            }
            if (file != null && file.ContentLength > 0)
            {
                if (!AllowedFileExtensions.Contains(file.FileName.Substring(file.FileName.LastIndexOf('.')).ToLower()))
                {
                    ErrorMessage = Resources.Resources.PleaseUploadImageOfType + string.Join(", ", AllowedFileExtensions);
                    return false;
                }
                else if (file.ContentLength > MaxContentLength)
                {
                    ErrorMessage = Resources.Resources.MaximumAllowedSize + (MaxContentLength / 1024 / 1024).ToString() + "MB";
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return false;
            }
        }
        #endregion Public Methods
    }
    public class ValidateIconAttribute : ValidationAttribute
    {
        #region Public Methods
        public override bool IsValid(object value)
        {
            int MaxContentLength = 10485760; //10 MB
            string[] AllowedFileExtensions = new string[] { ".png" };
            var file = value as HttpPostedFileBase;
            if (file == null)
            {
                return true;
            }
            if (file != null && file.ContentLength > 0)
            {
                if (!AllowedFileExtensions.Contains(file.FileName.Substring(file.FileName.LastIndexOf('.')).ToLower()))
                {
                    ErrorMessage = Resources.Resources.PleaseUploadIconOfType + string.Join(", ", AllowedFileExtensions);
                    return false;
                }
                else if (file.ContentLength > MaxContentLength)
                {
                    ErrorMessage = Resources.Resources.MaximumAllowedSize + (MaxContentLength / 1024 / 1024).ToString() + "MB";
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return false;
            }
        }
        #endregion Public Methods
    }

    public class ValidateFileAttribute : ValidationAttribute
    {
        #region Public Methods
        public override bool IsValid(object value)
        {
            int MaxContentLength = 20971520; //In bytes == 20MB
            string[] AllowedFileExtensions = new string[] { ".doc", ".docx", ".pdf", ".xls", ".xlsx" };

            var file = value as HttpPostedFileBase;

            if (file == null)
                return true;
            else if (!AllowedFileExtensions.Contains(file.FileName.Substring(file.FileName.LastIndexOf('.'))))
            {
                ErrorMessage = Resources.Resources.PleaseUploadFileOfType + string.Join(", ", AllowedFileExtensions);
                return false;
            }
            else if (file.ContentLength > MaxContentLength)
            {
                ErrorMessage = Resources.Resources.MaximumAllowedSize + (MaxContentLength / 1024 / 1024).ToString() + "MB";
                return false;
            }
            else
                return true;
        }
        #endregion Public Methods
    }
    public class ValidateAttachmentAttribute : ValidationAttribute
    {
        #region Public Methods
        public override bool IsValid(object valueList)
        {
            int MaxContentLength = 20971520; //In bytes == 20MB
            string[] AllowedFileExtensions = new string[] { ".doc", ".docx", ".pdf", ".xls", ".xlsx" };
            var fileList = valueList as List<HttpPostedFileBase>;
            if (fileList != null)
            {
                foreach (var file in fileList)
                {
                    if (file == null)
                        return true;
                    else if (!AllowedFileExtensions.Contains(file.FileName.Substring(file.FileName.LastIndexOf('.'))))
                    {
                        ErrorMessage = Resources.Resources.PleaseUploadFileOfType + string.Join(", ", AllowedFileExtensions);
                        return false;
                    }
                    else if (file.ContentLength > MaxContentLength)
                    {
                        ErrorMessage = Resources.Resources.MaximumAllowedSize + (MaxContentLength / 1024 / 1024).ToString() + "MB";
                        return false;
                    }
                }
            }
            return true;
        }
        #endregion Public Methods
    }
    public class ValidateCVNotRequiredAttribute : ValidationAttribute
    {
        #region Public Methods
        public override bool IsValid(object value)
        {
            int MaxContentLength = 20971520; //In bytes == 20MB
            string[] AllowedFileExtensions = new string[] { ".doc", ".docx", ".pdf" };

            var file = value as HttpPostedFileBase;

            if (file == null)
                return true;
            else if (!AllowedFileExtensions.Contains(file.FileName.Substring(file.FileName.LastIndexOf('.'))))
            {
                ErrorMessage = Resources.Resources.PleaseUploadFileOfType + string.Join(", ", AllowedFileExtensions);
                return false;
            }
            else if (file.ContentLength > MaxContentLength)
            {
                ErrorMessage = Resources.Resources.MaximumAllowedSize + (MaxContentLength / 1024 / 1024).ToString() + "MB";
                return false;
            }
            else
                return true;
        }
        #endregion Public Methods
    }

    public class ValidateCVAttribute : ValidationAttribute
    {
        #region Public Methods
        public override bool IsValid(object value)
        {
            int MaxContentLength = 20971520; //In bytes == 20MB
            string[] AllowedFileExtensions = new string[] { ".doc", ".docx", ".pdf" };

            var file = value as HttpPostedFileBase;

            if (file == null)
                return false;
            else if (!AllowedFileExtensions.Contains(file.FileName.Substring(file.FileName.LastIndexOf('.'))))
            {
                ErrorMessage = Resources.Resources.PleaseUploadFileOfType + string.Join(", ", AllowedFileExtensions);
                return false;
            }
            else if (file.ContentLength > MaxContentLength)
            {
                ErrorMessage = Resources.Resources.MaximumAllowedSize + (MaxContentLength / 1024 / 1024).ToString() + "MB";
                return false;
            }
            else
                return true;
        }
        #endregion Public Methods
    }

    /// <summary>
    /// Validates users before they are saved to an IUserStore
    /// </summary>
    /// <typeparam name="TUser"></typeparam>
    public class CustomUserValidator<TUser> : UserValidator<TUser, string>
        where TUser : ApplicationUser
    {
        #region Public Constructors
        public CustomUserValidator(UserManager<TUser, string> manager) : base(manager)
        {
            this.Manager = manager;
        }
        #endregion Public Constructors

        #region Private Properties
        private UserManager<TUser, string> Manager { get; set; }
        #endregion Private Properties

        #region Private Methods
        // make sure email is not empty, valid, and unique
        private async Task ValidateEmail(TUser user, List<string> errors)
        {
            if (string.IsNullOrWhiteSpace(user.Email))
            {
                errors.Add(String.Format(CultureInfo.CurrentCulture, Resources.Resources.EnterEmail, "Email"));
                return;
            }
            try
            {
                var m = new MailAddress(user.Email);
            }
            catch (FormatException)
            {
                errors.Add(String.Format(CultureInfo.CurrentCulture, Resources.Resources.EmailInvalid, "Email"));
                return;
            }
            //if (!user.Email.Contains("@psut.edu.jo"))
            //{
            //    errors.Add(String.Format(CultureInfo.CurrentCulture, Resources.Resources.InvalidOrganizationEmail, "Email"));
            //    return;
            //}
            var owner = await Manager.FindByEmailAsync(user.Email);
            if (owner != null && !EqualityComparer<string>.Default.Equals(owner.Id, user.Id))
            {
                errors.Add(String.Format(CultureInfo.CurrentCulture, Resources.Resources.DuplicateEmail, "Email"));
            }
        }
        #endregion Private Methods

        #region Public Methods
        public override async Task<IdentityResult> ValidateAsync(TUser item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }
            var errors = new List<string>();
            //await ValidateUserName(item, errors);
            if (RequireUniqueEmail)
            {
                await ValidateEmail(item, errors);
            }
            if (errors.Count > 0)
            {
                return IdentityResult.Failed(errors.ToArray());
            }
            return IdentityResult.Success;
        }
        #endregion Public Methods

        //private async Task ValidateUserName(TUser user, List<string> errors)
        //{
        //    if (string.IsNullOrWhiteSpace(user.UserName))
        //    {
        //        errors.Add(String.Format(CultureInfo.CurrentCulture, Resources.Resources.EmailInvalid, "Name"));
        //    }
        //    else if (AllowOnlyAlphanumericUserNames && !Regex.IsMatch(user.UserName, @"^[A-Za-z0-9@_\.]+$"))
        //    {
        //        // If any characters are not letters or digits, its an illegal user name
        //        errors.Add(String.Format(CultureInfo.CurrentCulture, Resources.Resources.InvalidUserName, user.UserName));
        //    }
        //    else
        //    {
        //        var owner = await Manager.FindByNameAsync(user.UserName);
        //        if (owner != null && !EqualityComparer<string>.Default.Equals(owner.Id, user.Id))
        //        {
        //            errors.Add(String.Format(CultureInfo.CurrentCulture, Resources.Resources.DuplicateUserName, user.UserName));
        //        }
        //    }
        //}
    }
}