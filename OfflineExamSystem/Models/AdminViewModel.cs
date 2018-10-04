using Resources;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

namespace OfflineExamSystem.Models
{
    public class IndexGroupViewModel
    {

        #region Public Constructors
        public IndexGroupViewModel()
        {
        }
        #endregion Public Constructors

        #region Public Properties
        public string Id { get; set; }
        [Display(Name = "Name", ResourceType = typeof(Resources.Resources))]
        public string Name { get; set; }
        [Display(Name = "Description", ResourceType = typeof(Resources.Resources))]
        public string Description { get; set; }
        public ICollection<ApplicationGroupRole> ApplicationRoles { get; set; }
        #endregion Public Properties

    }
    public class CreateRoleViewModel
    {

        #region Public Constructors
        public CreateRoleViewModel()
        {
        }
        #endregion Public Constructors

        #region Public Properties
        public string Id { get; set; }
        [Required(ErrorMessageResourceType = typeof(ValidationResources), ErrorMessageResourceName = "PropertyValueRequired")]
        [Display(Name = "Name", ResourceType = typeof(Resources.Resources))]
        public string Name { get; set; }
        #endregion Public Properties

    }
    public class EditRoleViewModel
    {

        #region Public Constructors
        public EditRoleViewModel()
        {
        }
        #endregion Public Constructors

        #region Public Properties
        public string Id { get; set; }
        [Required(ErrorMessageResourceType = typeof(ValidationResources), ErrorMessageResourceName = "PropertyValueRequired")]
        [Display(Name = "Name", ResourceType = typeof(Resources.Resources))]
        public string Name { get; set; }
        #endregion Public Properties

    }
    public class DeleteRoleViewModel
    {

        #region Public Constructors
        public DeleteRoleViewModel(ApplicationRole role)
        {
            this.Id = role.Id;
            this.Name = role.Name;
        }
        #endregion Public Constructors

        #region Public Properties
        public string Id { get; set; }
        [Display(Name = "Name", ResourceType = typeof(Resources.Resources))]
        public string Name { get; set; }
        #endregion Public Properties

    }
    public class DetailsRoleViewModel
    {

        #region Public Constructors
        public DetailsRoleViewModel(ApplicationRole role)
        {
            this.Name = role.Name;
        }
        #endregion Public Constructors

        #region Public Properties
        [Display(Name = "Name", ResourceType = typeof(Resources.Resources))]
        public string Name { get; set; }
        #endregion Public Properties

    }
    public class IndexRoleViewModel
    {

        #region Public Constructors
        public IndexRoleViewModel()
        {
        }
        public IndexRoleViewModel(ApplicationRole role)
        {
            this.Id = role.Id;
            this.Name = role.Name;
        }
        #endregion Public Constructors

        #region Public Properties
        public string Id { get; set; }
        [Display(Name = "Name", ResourceType = typeof(Resources.Resources))]
        public string Name { get; set; }
        #endregion Public Properties

    }
    public class IndexUserViewModel
    {

        #region Public Constructors
        public IndexUserViewModel()
        {
        }
        #endregion Public Constructors

        #region Public Properties
        public string Id { get; set; }
        [Display(Name = "UserName", ResourceType = typeof(Resources.Resources))]
        public string UserName { get; set; }
        [Display(Name = "Email", ResourceType = typeof(Resources.Resources))]
        public string Email { get; set; }
        [Display(Name = "EmailConfirmed", ResourceType = typeof(Resources.Resources))]
        public bool EmailConfirmed { get; set; }
        public ICollection<ApplicationUserRole> Roles { get; set; }
        #endregion Public Properties

    }
    public class EditUserViewModel
    {

        #region Public Constructors
        public EditUserViewModel()
        {
            this.RolesList = new List<SelectListItem>();
            this.GroupsList = new List<SelectListItem>();
        }
        #endregion Public Constructors

        #region Public Properties
        public string Id { get; set; }
        [Required(ErrorMessageResourceType = typeof(ValidationResources), ErrorMessageResourceName = "PropertyValueRequired")]
        [Display(Name = "EmpId", ResourceType = typeof(Resources.Resources))]
        public int EmpId { get; set; }
        [Required(ErrorMessageResourceType = typeof(ValidationResources), ErrorMessageResourceName = "PropertyValueRequired")]
        [Display(Name = "FullName_Ar", ResourceType = typeof(Resources.Resources))]
        public string FullName_Ar { get; set; }
        [Required(ErrorMessageResourceType = typeof(ValidationResources), ErrorMessageResourceName = "PropertyValueRequired")]
        [Display(Name = "FullName_En", ResourceType = typeof(Resources.Resources))]
        public string FullName_En { get; set; }
        [Required(ErrorMessageResourceType = typeof(ValidationResources), ErrorMessageResourceName = "PropertyValueRequired")]
        [Display(Name = "Email", ResourceType = typeof(Resources.Resources))]
        [EmailAddress(ErrorMessageResourceType = typeof(Resources.Resources), ErrorMessageResourceName = "EmailInvalid")]
        public string Email { get; set; }
        [Display(Name = "Enabled", ResourceType = typeof(Resources.Resources))]
        public bool IsEnabled { get; internal set; }
        [Display(Name = "Roles", ResourceType = typeof(Resources.Resources))]
        public ICollection<SelectListItem> RolesList { get; set; }

        [Display(Name = "Groups", ResourceType = typeof(Resources.Resources))]
        public ICollection<SelectListItem> GroupsList { get; set; }

        #endregion Public Properties
    }
    public class CreateUserViewModel
    {

        #region Public Constructors
        public CreateUserViewModel()
        {
            this.RolesList = new List<SelectListItem>();
            this.GroupsList = new List<SelectListItem>();
        }
        #endregion Public Constructors

        #region Public Properties
        [Required(ErrorMessageResourceType = typeof(ValidationResources), ErrorMessageResourceName = "PropertyValueRequired")]
        [Display(Name = "EmpId", ResourceType = typeof(Resources.Resources))]
        public int EmpId { get; set; }
        [Required(ErrorMessageResourceType = typeof(ValidationResources), ErrorMessageResourceName = "PropertyValueRequired")]
        [Display(Name = "FullName_Ar", ResourceType = typeof(Resources.Resources))]
        public string FullName_Ar { get; set; }
        [Required(ErrorMessageResourceType = typeof(ValidationResources), ErrorMessageResourceName = "PropertyValueRequired")]
        [Display(Name = "FullName_En", ResourceType = typeof(Resources.Resources))]
        public string FullName_En { get; set; }
        [Required(ErrorMessageResourceType = typeof(ValidationResources), ErrorMessageResourceName = "PropertyValueRequired")]
        [EmailAddress(ErrorMessageResourceType = typeof(Resources.Resources), ErrorMessageResourceName = "EmailInvalid")]
        [Display(Name = "Email", ResourceType = typeof(Resources.Resources))]
        public string Email { get; set; }
        [Required(ErrorMessageResourceType = typeof(ValidationResources), ErrorMessageResourceName = "PropertyValueRequired")]
        [StringLength(100, ErrorMessageResourceType = typeof(Resources.Resources), ErrorMessageResourceName = "StringLength", MinimumLength = 4)]
        [DataType(DataType.Password)]
        [Display(Name = "Password", ResourceType = typeof(Resources.Resources))]
        public string Password { get; set; }
        [DataType(DataType.Password)]
        [Display(Name = "ConfirmPassword", ResourceType = typeof(Resources.Resources))]
        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessageResourceType = typeof(Resources.Resources), ErrorMessageResourceName = "PasswordMatch")]
        public string ConfirmPassword { get; set; }
        [Display(Name = "Enabled", ResourceType = typeof(Resources.Resources))]
        public bool IsEnabled { get; set; }
        [Display(Name = "Roles", ResourceType = typeof(Resources.Resources))]
        public ICollection<SelectListItem> RolesList { get; set; }
        [Display(Name = "Groups", ResourceType = typeof(Resources.Resources))]
        public ICollection<SelectListItem> GroupsList { get; set; }
        #endregion Public Properties

    }
    public class DetailsUserViewModel
    {

        #region Public Constructors
        public DetailsUserViewModel()
        {
        }
        public DetailsUserViewModel(ApplicationUser user, IEnumerable<ApplicationGroup> userGroups)
        {
            this.UserName = user.UserName;
            this.GroupsList = userGroups.Select(u => u.Name).ToList();
        }
        #endregion Public Constructors

        #region Public Properties
        [Display(Name = "UserName", ResourceType = typeof(Resources.Resources))]
        public string UserName { get; set; }
        [Display(Name = "Groups", ResourceType = typeof(Resources.Resources))]
        public IList<string> GroupsList { get; set; }
        #endregion Public Properties

    }
    public class DeleteUserViewModel
    {

        #region Public Constructors
        public DeleteUserViewModel()
        {
        }
        public DeleteUserViewModel(ApplicationUser user, IEnumerable<ApplicationGroup> userGroups)
        {
            this.Id = user.Id;
            this.UserName = user.UserName;
            this.GroupsList = userGroups.Select(u => u.Name).ToList();
        }
        #endregion Public Constructors
        public string Id { get; set; }
        #region Public Properties
        [Display(Name = "UserName", ResourceType = typeof(Resources.Resources))]
        public string UserName { get; set; }
        [Display(Name = "Groups", ResourceType = typeof(Resources.Resources))]
        public IList<string> GroupsList { get; set; }
        #endregion Public Properties

    }
    public class CreateGroupViewModel
    {

        #region Public Constructors
        public CreateGroupViewModel()
        {
            this.RolesList = new List<SelectListItem>();
        }
        #endregion Public Constructors

        #region Public Properties
        [Required(ErrorMessageResourceType = typeof(ValidationResources), ErrorMessageResourceName = "PropertyValueRequired")]
        [Display(Name = "Name", ResourceType = typeof(Resources.Resources))]
        public string Name { get; set; }
        [Display(Name = "Description", ResourceType = typeof(Resources.Resources))]
        public string Description { get; set; }
        public ICollection<SelectListItem> RolesList { get; set; }
        #endregion Public Properties

    }
    public class EditGroupViewModel
    {

        #region Public Constructors
        public EditGroupViewModel()
        {
            this.UsersList = new List<SelectListItem>();
            this.RolesList = new List<SelectListItem>();
        }
        #endregion Public Constructors

        #region Public Properties
        public string Id { get; set; }
        [Required(ErrorMessageResourceType = typeof(ValidationResources), ErrorMessageResourceName = "PropertyValueRequired")]
        [Display(Name = "Name", ResourceType = typeof(Resources.Resources))]
        public string Name { get; set; }
        [Display(Name = "Description", ResourceType = typeof(Resources.Resources))]
        public string Description { get; set; }
        public ICollection<SelectListItem> UsersList { get; set; }
        public ICollection<SelectListItem> RolesList { get; set; }
        #endregion Public Properties

    }
    public class IndexGroupRoleViewModel
    {

        #region Public Constructors
        public IndexGroupRoleViewModel()
        {
        }
        #endregion Public Constructors

        #region Public Properties
        public string RoleId { get; set; }
        [Display(Name = "Role", ResourceType = typeof(Resources.Resources))]
        public string RoleName { get; set; }
        public string GroupId { get; set; }
        [Display(Name = "Group", ResourceType = typeof(Resources.Resources))]
        public string GroupName { get; set; }
        #endregion Public Properties

    }
    public class IndexUserGroupViewModel
    {

        #region Public Constructors
        public IndexUserGroupViewModel()
        {
        }
        #endregion Public Constructors

        #region Public Properties
        public string GroupId { get; set; }
        [Display(Name = "Group", ResourceType = typeof(Resources.Resources))]
        public string GroupName { get; set; }
        public string UserId { get; set; }
        [Display(Name = "UserName", ResourceType = typeof(Resources.Resources))]
        public string UserName { get; set; }
        #endregion Public Properties

    }
    public class GroupRoleViewModel
    {

        #region Public Constructors
        public GroupRoleViewModel()
        {
        }
        #endregion Public Constructors

        #region Public Properties
        public string RoleId { get; set; }
        [Display(Name = "Role", ResourceType = typeof(Resources.Resources))]
        public string RoleName { get; set; }
        public string GroupId { get; set; }
        [Display(Name = "Group", ResourceType = typeof(Resources.Resources))]
        public string GroupName { get; set; }
        #endregion Public Properties

    }
    public class UserGroupViewModel
    {

        #region Public Constructors
        public UserGroupViewModel()
        {
        }
        #endregion Public Constructors

        #region Public Properties
        public string GroupId { get; set; }
        [Display(Name = "Group", ResourceType = typeof(Resources.Resources))]
        public string GroupName { get; set; }
        public string UserId { get; set; }
        [Display(Name = "UserName", ResourceType = typeof(Resources.Resources))]
        public string UserName { get; set; }
        #endregion Public Properties

    }
    public class IndexUserRolesViewModel
    {

        #region Public Constructors
        public IndexUserRolesViewModel()
        {
        }
        #endregion Public Constructors

        #region Public Properties
        public string RoleName { get; set; }
        #endregion Public Properties

    }
}