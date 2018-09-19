using OfflineExamSystem.DDL.Repository;
using OfflineExamSystem.Helpers;
using OfflineExamSystem.Models;
using Microsoft.AspNet.Identity;
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
    public class RolesAdminController : BaseController
    {
        #region Private Fields
        private ApplicationDbContext db = new ApplicationDbContext();
        private ApplicationUserManager _userManager;
        private ApplicationRoleManager _roleManager;
        private ApplicationGroupManager _groupManager;
        #endregion Private Fields

        #region Public Constructors
        public RolesAdminController()
        {
        }

        public RolesAdminController(ApplicationUserManager userManager,
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
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            set
            {
                _userManager = value;
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
        public ActionResult GetGroups(string searchTerm, int pageSize, int pageNum, string userId)
        {
            var userGroups = GroupManager.GetUserGroups(userId).Select(ug => ug.Id).ToList();
            //Get the paged results and the total count of the results for this query.
            GroupRepository gr = new GroupRepository();
            List<ApplicationGroup> Groups = gr.GetGroups(searchTerm, pageSize, pageNum, userGroups);
            int count = gr.GetGroupsCount(searchTerm, pageSize, pageNum, userGroups);
            //Translate the Groups into a format the select2 dropdown expects
            Select2PagedResult pagedGroups = gr.ToSelect2Format(Groups, count);

            //Return the data as a jsonp result
            return new JsonResultHelper
            {
                Data = pagedGroups,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public ActionResult Index()
        {
            return View();
        }
        public PartialViewResult RoleGrid(string name, string description, string groupId, int? page, int? rows, string sort, string order)
        {
            var query = (from r in db.Roles select new IndexRoleViewModel { Id = r.Id, Name = r.Name }).AsQueryable();
            if (!string.IsNullOrEmpty(name))
                query = query.Where(q => q.Name.ToLower().Contains(name.ToLower()));
            if (!string.IsNullOrEmpty(groupId))
                query = from r in query
                        join gr in db.ApplicationGroupRoles on r.Id equals gr.ApplicationRoleId
                        where gr.ApplicationGroupId == groupId
                        select new IndexRoleViewModel { Name = r.Name };
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
            return PartialView("_RoleGrid", query.Skip((page - 1 ?? 0) * (rows ?? 10)).Take(rows ?? 10).ToList());
        }
        public async Task<ActionResult> Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var role = await RoleManager.FindByIdAsync(id);
            if (role == null)
            {
                return HttpNotFound();
            }
            DetailsRoleViewModel roleViewModel = new DetailsRoleViewModel(role);
            // Get the list of Users in this Role
            var users = new List<ApplicationUser>();

            // Get the list of Users in this Role
            foreach (var user in UserManager.Users.ToList())
            {
                if (await UserManager.IsInRoleAsync(user.Id, role.Name))
                {
                    users.Add(user);
                }
            }

            ViewBag.Users = users;
            ViewBag.UserCount = users.Count();
            return PartialView(roleViewModel);
        }
        public ActionResult Create()
        {
            //Get a SelectList of Roles to choose from in the View:
            ViewBag.RolesList = new SelectList(this.RoleManager.Roles.ToList(), "Id", "Name");
            return PartialView("Create");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(CreateRoleViewModel createRoleViewModel)
        {
            if (ModelState.IsValid)
            {
                var role = new ApplicationRole(createRoleViewModel.Name);
                var roleResult = await RoleManager.CreateAsync(role);
                if (!roleResult.Succeeded)
                {
                    ModelState.AddModelError(string.Empty, Resources.Resources.RoleInUse);
                    return PartialView("Create", createRoleViewModel);
                }
                return Json(new { Success = true });
            }
            return PartialView("Create", createRoleViewModel);
        }
        public async Task<ActionResult> Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var role = await RoleManager.FindByIdAsync(id);
            if (role == null)
            {
                return HttpNotFound();
            }
            EditRoleViewModel editRoleViewModel = new EditRoleViewModel { Id = role.Id, Name = role.Name };
            return PartialView("Edit", editRoleViewModel);
        }
        // POST: /Roles/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Name")] EditRoleViewModel editRoleViewModel)
        {
            if (ModelState.IsValid)
            {
                var role = await RoleManager.FindByIdAsync(editRoleViewModel.Id);
                role.Name = editRoleViewModel.Name;
                var roleResult = await RoleManager.UpdateAsync(role);
                if (!roleResult.Succeeded)
                {
                    ModelState.AddModelError(string.Empty, Resources.Resources.RoleInUse);
                    return PartialView("Edit", editRoleViewModel);
                }
                return Json(new { Success = true });
            }
            return PartialView("Edit", editRoleViewModel);
        }
        //
        // GET: /Roles/Delete/5
        public async Task<ActionResult> Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var role = await RoleManager.FindByIdAsync(id);
            if (role == null)
            {
                return HttpNotFound();
            }
            DeleteRoleViewModel deleteRoleViewModel = new DeleteRoleViewModel(role);
            return PartialView("Delete", deleteRoleViewModel);
        }

        //
        // POST: /Roles/Delete/5
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
                var role = await RoleManager.FindByIdAsync(id);
                if (role == null)
                {
                    return HttpNotFound();
                }
                IdentityResult result = await RoleManager.DeleteAsync(role);
                if (!result.Succeeded)
                {
                    ModelState.AddModelError(string.Empty, Resources.Resources.RoleInUse);
                    return PartialView("Delete", role);
                }
                return Json(new { Success = true });
            }
            return PartialView("Delete");
        }
        #endregion Public Methods
    }
}