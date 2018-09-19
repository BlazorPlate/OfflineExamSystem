using OfflineExamSystem.DDL.Repository;
using OfflineExamSystem.Helpers;
using OfflineExamSystem.Models;
using Microsoft.AspNet.Identity.Owin;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace OfflineExamSystem.Controllers
{
    [LocalizedAuthorize(Roles = "Admin")]
    public class UserGroupsController : BaseController
    {
        #region Private Fields
        private ApplicationDbContext db = new ApplicationDbContext();
        private ApplicationUserManager _userManager;
        private ApplicationGroupManager _groupManager;
        #endregion Private Fields

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

        // GET: UserGroups
        public async Task<ActionResult> Index(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var user = await UserManager.FindByIdAsync(id);
            ViewBag.UserId = id;
            ViewBag.UserName = user.UserName;
            return View();
        }
        public PartialViewResult UserGroupsGrid(string id, string name, int? page, int? rows, string sort, string order)
        {
            var query = (from ug in db.ApplicationUserGroups.Where(ug => ug.ApplicationUserId == id)
                         join u in db.Users on ug.ApplicationUserId equals u.Id
                         join gr in db.ApplicationUserGroups on ug.ApplicationGroupId equals gr.ApplicationGroupId
                         join g in db.ApplicationGroups on gr.ApplicationGroupId equals g.Id
                         select new IndexUserGroupViewModel { UserName = u.UserName, UserId = u.Id, GroupName = g.Name, GroupId = g.Id }).DistinctBy(g => g.GroupName).AsQueryable();
            if (!string.IsNullOrEmpty(name))
                query = query.Where(q => q.GroupName.ToLower().Contains(name.ToLower()));
            if (!string.IsNullOrWhiteSpace(sort))
            {
                switch (sort.ToLower())
                {
                    case "name":
                        if (order == "asc")
                            query = query.OrderBy(q => q.GroupName);
                        else
                            query = query.OrderByDescending(q => q.GroupName);
                        break;
                    default:
                        query = query.OrderByDescending(q => q.GroupId);
                        break;
                }
            }
            else
            {
                query = query.OrderBy(q => q.GroupName);
            }
            ViewBag.TotalRows = query.Count();
            return PartialView("_UserGroupsGrid", query.Skip((page - 1 ?? 0) * (rows ?? 10)).Take(rows ?? 10).ToList());
        }

        // GET: UserGroups/Details/5
        public async Task<ActionResult> Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ApplicationUserGroup applicationUserGroup = await db.ApplicationUserGroups.FindAsync(id);
            if (applicationUserGroup == null)
            {
                return HttpNotFound();
            }
            return View(applicationUserGroup);
        }

        public ActionResult Add(string id)
        {
            var userGroup = new ApplicationUserGroup();
            userGroup.ApplicationUserId = id;
            return PartialView("Add", userGroup);
        }
        [HttpPost]
        public async Task<ActionResult> Add([Bind(Include = "ApplicationUserId, ApplicationGroupId")] ApplicationUserGroup userGroup, params string[] selectedGroups)
        {
            await GroupManager.SetUserGroupAsync(userGroup.ApplicationUserId, userGroup.ApplicationGroupId);
            return Json(new { Success = true });
        }

        // GET: UserGroups/Delete/5
        public async Task<ActionResult> Delete(string userId, string groupId)
        {
            if (userId == null || groupId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var userGroup = await GroupManager.GetUserGroupAsync(userId, groupId);
            if (userGroup == null)
            {
                return HttpNotFound();
            }
            return PartialView("Delete", userGroup);
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string userId, string groupId)
        {
            if (userId == null || groupId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            await GroupManager.RemoveUserGroupAsync(userId, groupId);
            return Json(new { Success = true });
        }
        #endregion Public Methods
    }
}