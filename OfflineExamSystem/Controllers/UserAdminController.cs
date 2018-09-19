using OfflineExamSystem.DDL.Repository;
using OfflineExamSystem.Helpers;
using OfflineExamSystem.Models;
using Microsoft.AspNet.Identity.Owin;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace OfflineExamSystem.Controllers
{
    [LocalizedAuthorize(Roles = "Admin")]
    public class UsersAdminController : BaseController
    {
        #region Private Fields
        private ApplicationDbContext db = new ApplicationDbContext();

        private ApplicationUserManager _userManager;
        // Add the Group Manager (NOTE: only access through the public
        // Property, not by the instance variable!)
        private ApplicationGroupManager _groupManager;
        private ApplicationRoleManager _roleManager;
        #endregion Private Fields

        #region Public Constructors
        public UsersAdminController()
        {
        }

        public UsersAdminController(ApplicationUserManager userManager,
            ApplicationRoleManager roleManager)
        {
            UserManager = userManager;
            RoleManager = roleManager;
        }
        #endregion Public Constructors

        #region Public Properties
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext()
                    .GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }
        public ApplicationGroupManager GroupManager
        {
            get
            {
                return _groupManager ?? new ApplicationGroupManager();
            }
            private set
            {
                _groupManager = value;
            }
        }
        public ApplicationRoleManager RoleManager
        {
            get
            {
                return _roleManager ?? HttpContext.GetOwinContext()
                    .Get<ApplicationRoleManager>();
            }
            private set
            {
                _roleManager = value;
            }
        }
        #endregion Public Properties

        #region Public Methods
        public ActionResult GetRoles(string searchTerm, int pageSize, int pageNum)
        {
            //Get the paged results and the total count of the results for this query.
            RoleRepository ur = new RoleRepository();
            List<ApplicationRole> roles = ur.GetRoles(searchTerm, pageSize, pageNum);
            int count = ur.GetRolesCount(searchTerm, pageSize, pageNum);
            //Translate the roles into a format the select2 dropdown expects
            Select2PagedResult pagedRoles = ur.ToSelect2Format(roles, count);

            //Return the data as a jsonp result
            return new JsonResultHelper
            {
                Data = pagedRoles,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public PartialViewResult UserGrid(string username, string roleId, string emailConfirmed, int? page, int? rows, string sort, string order)
        {
            var query = (from u in UserManager.Users select new IndexUserViewModel { Id = u.Id, UserName = u.UserName, Email = u.Email, EmailConfirmed = u.EmailConfirmed, Roles = u.Roles }).AsQueryable();
            if (!string.IsNullOrEmpty(username))
                query = query.Where(q => q.UserName.ToLower().Contains(username.ToLower()));
            if (!string.IsNullOrEmpty(roleId))
                query = query.Where(q => q.Roles.Any(r => r.RoleId == roleId));
            if (!string.IsNullOrEmpty(emailConfirmed))
            {
                bool isEmailConfirmed = bool.Parse(emailConfirmed);
                query = query.Where(f => f.EmailConfirmed == isEmailConfirmed);
            }
            if (!string.IsNullOrWhiteSpace(sort))
            {
                switch (sort.ToLower())
                {
                    case "username":
                        if (order == "asc")
                            query = query.OrderBy(q => q.UserName);
                        else
                            query = query.OrderByDescending(q => q.UserName);
                        break;
                    case "email":
                        if (order == "asc")
                            query = query.OrderBy(q => q.Email);
                        else
                            query = query.OrderByDescending(q => q.Email);
                        break;

                    case "emailconfirmed":
                        if (order == "asc")
                            query = query.OrderBy(q => q.EmailConfirmed);
                        else
                            query = query.OrderByDescending(q => q.EmailConfirmed);
                        break;

                    case "id":
                        if (order == "asc")
                            query = query.OrderBy(q => q.Id);
                        else
                            query = query.OrderByDescending(q => q.Id);
                        break;

                    default:
                        query = query.OrderByDescending(q => q.Id);
                        break;
                }
            }
            else
            {
                query = query.OrderBy(q => q.Id);
            }
            ViewBag.TotalRows = query.Count();
            return PartialView("_UserGrid", query.Skip((page - 1 ?? 0) * (rows ?? 10)).Take(rows ?? 10).ToList());
        }

        public async Task<ActionResult> Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var user = await UserManager.FindByIdAsync(id);
            // Show the groups the user belongs to:
            var userGroups = await this.GroupManager.GetUserGroupsAsync(id);
            var detailsUserViewModel = new DetailsUserViewModel(user, userGroups);
            return PartialView("Details", detailsUserViewModel);
        }
        public async Task<ActionResult> Roles(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var user = await UserManager.FindByIdAsync(id);
            ViewBag.UserName = user.UserName;
            var userRoles = from r in db.Roles
                            join ur in db.ApplicationUserRole on r.Id equals ur.RoleId
                            join u in db.Users on user.Id equals u.Id
                            where ur.UserId == id
                            select new IndexUserRolesViewModel { RoleName = r.Name };

            return PartialView("Roles", await userRoles.ToListAsync());
        }

        public ActionResult Create()
        {
            // Show a list of available groups:
            CreateUserViewModel createUserViewModel = new CreateUserViewModel();
            var groups = this.GroupManager.Groups;

            foreach (var group in groups)
            {
                var listItem = new SelectListItem()
                {
                    Text = group.Name,
                    Value = group.Id,
                    Selected = false
                };
                createUserViewModel.GroupsList.Add(listItem);
            }
            return PartialView("Create", createUserViewModel);
        }

        [HttpPost]
        public async Task<ActionResult> Create(CreateUserViewModel createUserViewModel, params string[] selectedGroups)
        {
            if (ModelState.IsValid)
            {
                if (await UserManager.UserExistAsync(null, createUserViewModel.Email))
                {
                    // Display a list of available Groups:
                    foreach (var group in GroupManager.Groups)
                    {
                        var listItem = new SelectListItem()
                        {
                            Text = group.Name,
                            Value = group.Id,
                            Selected = selectedGroups.Any(g => g == group.Id)
                        };
                        createUserViewModel.GroupsList.Add(listItem);
                    }
                    ModelState.AddModelError(string.Empty, Resources.Resources.UserExist);
                    return PartialView("Create", createUserViewModel);
                }
                var user = new ApplicationUser
                {
                    EmpId = createUserViewModel.EmpId,
                    FullName_En = createUserViewModel.FullName_En,
                    FullName_Ar = createUserViewModel.FullName_Ar,
                    UserName = createUserViewModel.Email,
                    Email = createUserViewModel.Email,
                    EmailConfirmed = true
                };
                var result = await UserManager.CreateAsync(user, createUserViewModel.Password);

                //Add User to the selected Groups
                if (result.Succeeded)
                {
                    if (selectedGroups != null)
                    {
                        selectedGroups = selectedGroups ?? new string[] { };
                        await this.GroupManager
                            .SetUserGroupsAsync(user.Id, selectedGroups);
                    }
                    return Json(new { Success = true });
                }
                else
                {
                    foreach (var item in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, item);
                    }
                    // Display a list of available Groups:
                    foreach (var group in GroupManager.Groups)
                    {
                        var listItem = new SelectListItem()
                        {
                            Text = group.Name,
                            Value = group.Id,
                            Selected = selectedGroups.Any(g => g == group.Id)
                        };
                        createUserViewModel.GroupsList.Add(listItem);
                    }
                    return PartialView("Create", createUserViewModel);
                }
            }
            // Display a list of available Groups:
            foreach (var group in GroupManager.Groups)
            {
                SelectListItem listItem = new SelectListItem();
                listItem.Text = group.Name;
                listItem.Value = group.Id;
                listItem.Selected = selectedGroups == null ? false : selectedGroups.Any(g => g == group.Id);
                createUserViewModel.GroupsList.Add(listItem);
            }
            return PartialView("Create", createUserViewModel);
        }

        public async Task<ActionResult> Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var user = await UserManager.FindByIdAsync(id);
            if (user == null)
            {
                return HttpNotFound();
            }

            // Display a list of available Groups:
            var allGroups = this.GroupManager.Groups;
            var userGroups = await this.GroupManager.GetUserGroupsAsync(id);

            var editUserViewModel = new EditUserViewModel()
            {
                Id = user.Id,
                EmpId = user.EmpId,
                FullName_En = user.FullName_En,
                FullName_Ar = user.FullName_Ar,
                Email = user.Email
            };

            foreach (var group in allGroups)
            {
                var listItem = new SelectListItem()
                {
                    Text = group.Name,
                    Value = group.Id,
                    Selected = userGroups.Any(g => g.Id == group.Id)
                };
                editUserViewModel.GroupsList.Add(listItem);
            }
            return PartialView("Edit", editUserViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "EmpId,FullName_EN,FullName_Ar,Email,Id")] EditUserViewModel editUserViewModel, params string[] selectedGroups)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByIdAsync(editUserViewModel.Id);
                if (user == null)
                {
                    return HttpNotFound();
                }
                if (await UserManager.UserExistAsync(user.Id, editUserViewModel.Email))
                {
                    // Display a list of available Groups:
                    var groups = GroupManager.Groups;
                    //var userGroups = await GroupManager.GetUserGroupsAsync(editUser.Id);
                    foreach (var group in groups)
                    {
                        var listItem = new SelectListItem()
                        {
                            Text = group.Name,
                            Value = group.Id,
                            Selected = selectedGroups.Any(g => g == group.Id)
                        };
                        editUserViewModel.GroupsList.Add(listItem);
                    }
                    ModelState.AddModelError(string.Empty, Resources.Resources.UserExist);
                    return PartialView("Edit", editUserViewModel);
                }
                // Update the User:
                user.EmpId = editUserViewModel.EmpId;
                user.FullName_En = editUserViewModel.FullName_En;
                user.FullName_Ar = editUserViewModel.FullName_Ar;
                user.UserName = editUserViewModel.Email;
                user.Email = editUserViewModel.Email;
                await this.UserManager.UpdateAsync(user);

                // Update the Groups:
                selectedGroups = selectedGroups ?? new string[] { };
                await GroupManager.SetUserGroupsAsync(user.Id, selectedGroups);
                return Json(new { Success = true });
            }
            else
            {
                // Display a list of available Groups:
                var groups = GroupManager.Groups;
                foreach (var group in groups)
                {
                    SelectListItem listItem = new SelectListItem();
                    listItem.Text = group.Name;
                    listItem.Value = group.Id;
                    listItem.Selected = selectedGroups == null ? false : selectedGroups.Any(g => g == group.Id);
                    editUserViewModel.GroupsList.Add(listItem);
                }
                return PartialView("Edit", editUserViewModel);
            }
        }

        public async Task<ActionResult> Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var user = await UserManager.FindByIdAsync(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            // Show the groups the user belongs to:
            var userGroups = await this.GroupManager.GetUserGroupsAsync(id);
            var deleteUserViewModel = new DeleteUserViewModel(user, userGroups);
            return PartialView("Delete", deleteUserViewModel);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string id)
        {
            if (ModelState.IsValid)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                var user = await UserManager.FindByIdAsync(id);
                if (user == null)
                {
                    return HttpNotFound();
                }

                // Remove all the User Group references:
                await this.GroupManager.ClearUserGroupsAsync(id);

                // Then Delete the User:
                var result = await UserManager.DeleteAsync(user);
                if (!result.Succeeded)
                {
                    ModelState.AddModelError("", result.Errors.First());
                    return View();
                }
                return Json(new { Success = true });
            }
            var userGroups = await this.GroupManager.GetUserGroupsAsync(id);
            ViewBag.GroupNames = userGroups.Select(u => u.Name).ToList();
            return PartialView("Delete");
        }
        #endregion Public Methods
    }
}