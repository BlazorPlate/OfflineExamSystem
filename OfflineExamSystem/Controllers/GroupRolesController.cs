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
    public class GroupRolesController : BaseController
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

        #region Public Methods
        public ActionResult GetRoles(string searchTerm, int pageSize, int pageNum, string groupId)
        {
            var groupRoles = GroupManager.GetGroupRoles(groupId).Select(gr => gr.Id).ToList();
            //Get the paged results and the total count of the results for this query.
            GroupRoleRepository ur = new GroupRoleRepository();
            List<ApplicationRole> roles = ur.GetRoles(searchTerm, pageSize, pageNum, groupRoles).ToList();
            int count = ur.GetRolesCount(searchTerm, pageSize, pageNum, groupRoles);
            //Translate the roles into a format the select2 dropdown expects
            Select2PagedResult pagedRoles = ur.ToSelect2Format(roles, count);

            //Return the data as a jsonp result
            return new JsonResultHelper
            {
                Data = pagedRoles,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        // GET: GroupRoles
        public ActionResult Index(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ViewBag.GroupId = id;
            ViewBag.GroupName = GroupManager.FindById(id).Name;
            return View();
        }
        public PartialViewResult GroupRolesGrid(string id, string name, string description, int? page, int? rows, string sort, string order)
        {
            var query = (from r in GroupManager.GetGroupRoles(id)
                         join gr in db.ApplicationGroupRoles on r.Id equals gr.ApplicationRoleId
                         join g in db.ApplicationGroups on gr.ApplicationGroupId equals g.Id
                         select new IndexGroupRoleViewModel { RoleId = r.Id, RoleName = r.Name, GroupId = g.Id }).DistinctBy(r => r.RoleName).AsQueryable();
            if (!string.IsNullOrEmpty(name))
                query = query.Where(q => q.RoleName.ToLower().Contains(name.ToLower()));
            if (!string.IsNullOrWhiteSpace(sort))
            {
                switch (sort.ToLower())
                {
                    case "name":
                        if (order == "asc")
                            query = query.OrderBy(q => q.RoleName);
                        else
                            query = query.OrderByDescending(q => q.RoleName);
                        break;
                    default:
                        query = query.OrderByDescending(q => q.RoleId);
                        break;
                }
            }
            else
            {
                query = query.OrderBy(q => q.RoleName);
            }
            ViewBag.TotalRows = query.Count();
            return PartialView("_GroupRolesGrid", query.Skip((page - 1 ?? 0) * (rows ?? 10)).Take(rows ?? 10).ToList());
        }

        public ActionResult Add(string id)
        {
            var groupRole = new ApplicationGroupRole();
            groupRole.ApplicationGroupId = id;
            return PartialView("Add", groupRole);
        }
        [HttpPost]
        public async Task<ActionResult> Add([Bind(Include = "ApplicationGroupId, ApplicationRoleId")] ApplicationGroupRole groupRole)
        {
            var roles = await GroupManager.AddGroupRoleAsync(groupRole.ApplicationGroupId, groupRole.ApplicationRoleId);
            return Json(new { Success = true });
        }

        public async Task<ActionResult> Delete(string groupId, string roleId)
        {
            if (groupId == null || roleId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var groupRole = await GroupManager.GetGroupRoleAsync(groupId, roleId);
            if (groupRole == null)
            {
                return HttpNotFound();
            }
            return PartialView("Delete", groupRole);
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string groupId, string roleId)
        {
            if (groupId == null || roleId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            await GroupManager.RemoveGroupRoleAsync(groupId, roleId);
            return Json(new { Success = true });
        }
        #endregion Public Methods
    }
}