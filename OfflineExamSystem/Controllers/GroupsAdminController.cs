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
    public class GroupsAdminController : BaseController
    {
        #region Private Fields
        private ApplicationDbContext db = new ApplicationDbContext();
        private ApplicationGroupManager _groupManager;
        private ApplicationRoleManager _roleManager;
        #endregion Private Fields

        #region Public Properties
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
                return _roleManager ?? HttpContext.GetOwinContext().Get<ApplicationRoleManager>();
            }
            private set
            {
                _roleManager = value;
            }
        }
        #endregion Public Properties

        #region Protected Methods
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        #endregion Protected Methods

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
        public PartialViewResult GroupGrid(string name, string description, string roleId, int? page, int? rows, string sort, string order)
        {
            var query = (from g in db.ApplicationGroups select new IndexGroupViewModel { Id = g.Id, Name = g.Name, Description = g.Description, ApplicationRoles = g.ApplicationRoles }).AsQueryable();
            if (!string.IsNullOrEmpty(name))
                query = query.Where(q => q.Name.ToLower().Contains(name.ToLower()));
            if (!string.IsNullOrEmpty(description))
                query = query.Where(q => q.Description.ToLower().Contains(description.ToLower()));
            if (!string.IsNullOrEmpty(roleId))
                query = query.Where(q => q.ApplicationRoles.Any(r => r.ApplicationRoleId == roleId));
            if (!string.IsNullOrWhiteSpace(sort))
            {
                switch (sort.ToLower())
                {
                    case "name":
                        if (order == "asc")
                            query = query.OrderBy(q => q.Name);
                        else
                            query = query.OrderByDescending(q => q.Name);
                        break;
                    case "description":
                        if (order == "asc")
                            query = query.OrderBy(q => q.Description);
                        else
                            query = query.OrderByDescending(q => q.Description);
                        break;

                    default:
                        query = query.OrderByDescending(q => q.Id);
                        break;
                }
            }
            else
            {
                query = query.OrderBy(q => q.Name);
            }
            ViewBag.TotalRows = query.Count();
            return PartialView("_GroupGrid", query.Skip((page - 1 ?? 0) * (rows ?? 10)).Take(rows ?? 10).ToList());
        }
        public async Task<ActionResult> Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ApplicationGroup applicationGroup = await this.GroupManager.Groups.FirstOrDefaultAsync(g => g.Id == id);
            if (applicationGroup == null)
            {
                return HttpNotFound();
            }
            var groupRoles = this.GroupManager.GetGroupRoles(applicationGroup.Id);
            string[] RoleNames = groupRoles.Select(p => p.Name).ToArray();
            ViewBag.RolesList = RoleNames;
            ViewBag.RolesCount = RoleNames.Count();
            return PartialView("Details", applicationGroup);
        }
        public ActionResult Create()
        {
            CreateGroupViewModel createGroupViewModel = new CreateGroupViewModel();
            //Get a SelectList of Roles to choose from in the View:
            foreach (var role in RoleManager.Roles)
            {
                var listItem = new SelectListItem()
                {
                    Text = role.Name,
                    Value = role.Id,
                    Selected = false
                };
                createGroupViewModel.RolesList.Add(listItem);
            }
            return PartialView("Create", createGroupViewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Name,Description")] CreateGroupViewModel createGroupViewModel, params string[] selectedRoles)
        {
            if (ModelState.IsValid)
            {
                selectedRoles = selectedRoles ?? new string[] { };
                if (!await GroupManager.GroupExistAsync(null, createGroupViewModel.Name))
                {
                    var group = new ApplicationGroup();
                    group.Name = createGroupViewModel.Name;
                    group.Description = createGroupViewModel.Description;
                    // Create the new Group:
                    var result = await this.GroupManager.CreateGroupAsync(group);
                    if (result.Succeeded)
                    {
                        // Add the roles selected:
                        await this.GroupManager.SetGroupRolesAsync(group.Id, selectedRoles);
                    }
                    return Json(new { Success = true });
                }
                else
                {
                    ModelState.AddModelError(string.Empty, Resources.Resources.GroupExist);
                    foreach (var role in RoleManager.Roles)
                    {
                        var listItem = new SelectListItem()
                        {
                            Text = role.Name,
                            Value = role.Id,
                            Selected = selectedRoles.Any(r => r == role.Id)
                        };
                        createGroupViewModel.RolesList.Add(listItem);
                    }
                    return PartialView("Create", createGroupViewModel);
                }
            }
            // Otherwise, start over:
            foreach (var role in RoleManager.Roles)
            {
                SelectListItem listItem = new SelectListItem();
                listItem.Text = role.Name;
                listItem.Value = role.Id;
                listItem.Selected = selectedRoles == null ? false : selectedRoles.Any(r => r == role.Id);
                createGroupViewModel.RolesList.Add(listItem);
            }
            return PartialView("Create", createGroupViewModel);
        }
        public async Task<ActionResult> Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ApplicationGroup applicationGroup = await this.GroupManager.FindByIdAsync(id);
            if (applicationGroup == null)
            {
                return HttpNotFound();
            }

            // Get a list, not a DbSet or queryable:
            var allRoles = await this.RoleManager.Roles.ToListAsync();
            var groupRoles = await this.GroupManager.GetGroupRolesAsync(id);

            var editGroupViewModel = new EditGroupViewModel()
            {
                Id = applicationGroup.Id,
                Name = applicationGroup.Name,
                Description = applicationGroup.Description
            };

            // load the roles/Roles for selection in the form:
            foreach (var role in allRoles)
            {
                var listItem = new SelectListItem()
                {
                    Text = role.Name,
                    Value = role.Id,
                    Selected = groupRoles.Any(g => g.Id == role.Id)
                };
                editGroupViewModel.RolesList.Add(listItem);
            }
            return PartialView("Edit", editGroupViewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Name,Description")] EditGroupViewModel editGroupViewModel, params string[] selectedRoles)
        {
            var group = await this.GroupManager.FindByIdAsync(editGroupViewModel.Id);
            if (group == null)
            {
                return HttpNotFound();
            }
            if (ModelState.IsValid)
            {
                if (!await GroupManager.GroupExistAsync(editGroupViewModel.Id, editGroupViewModel.Name))
                {
                    group.Name = editGroupViewModel.Name;
                    group.Description = editGroupViewModel.Description;
                    await this.GroupManager.UpdateGroupAsync(group);
                    selectedRoles = selectedRoles ?? new string[] { };
                    await this.GroupManager.SetGroupRolesAsync(group.Id, selectedRoles);
                    return Json(new { Success = true });
                }
                else
                {
                    // Get a list, not a DbSet or queryable:
                    var roles = await this.RoleManager.Roles.ToListAsync();
                    var groupRoles = await this.GroupManager.GetGroupRolesAsync(editGroupViewModel.Id);
                    // load the roles for selection in the form:
                    foreach (var role in roles)
                    {
                        var listItem = new SelectListItem()
                        {
                            Text = role.Name,
                            Value = role.Id,
                            Selected = selectedRoles.Any(g => g == role.Id)
                        };
                        editGroupViewModel.RolesList.Add(listItem);
                    }
                    ModelState.AddModelError(string.Empty, Resources.Resources.GroupExist);
                    return PartialView("Edit", editGroupViewModel);
                }
            }
            else
            {
                // Get a list, not a DbSet or queryable:
                var roles = await this.RoleManager.Roles.ToListAsync();
                // load the roles/Roles for selection in the form:
                foreach (var role in roles)
                {
                    SelectListItem listItem = new SelectListItem();
                    listItem.Text = role.Name;
                    listItem.Value = role.Id;
                    listItem.Selected = selectedRoles == null ? false : selectedRoles.Any(r => r == role.Id);
                    editGroupViewModel.RolesList.Add(listItem);
                }
                return PartialView("Edit", editGroupViewModel);
            }
        }
        public async Task<ActionResult> Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ApplicationGroup applicationGroup = await this.GroupManager.FindByIdAsync(id);
            var groupRoles = await this.GroupManager.GetGroupRolesAsync(applicationGroup.Id);
            string[] RoleNames = groupRoles.Select(p => p.Name).ToArray();
            ViewBag.RolesList = RoleNames;
            ViewBag.RolesCount = RoleNames.Count();
            if (applicationGroup == null)
            {
                return HttpNotFound();
            }
            return PartialView("Delete", applicationGroup);
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ApplicationGroup applicationGroup = await this.GroupManager.FindByIdAsync(id);
            await this.GroupManager.DeleteGroupAsync(id);
            return Json(new { Success = true });
        }
        #endregion Public Methods
    }
}