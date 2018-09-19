using OfflineExamSystem.Helpers;
using OfflineExamSystem.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OfflineExamSystem.DDL.Repository
{
    public class GroupRoleRepository
    {
        #region Private Fields
        private ApplicationDbContext db = new ApplicationDbContext();
        #endregion Private Fields

        #region Public Constructors
        public GroupRoleRepository()
        {
            roles = GenerateRole();
        }
        #endregion Public Constructors

        #region Public Properties
        public IQueryable<ApplicationRole> roles { get; set; }
        #endregion Public Properties

        #region Private Methods
        //Our search term
        private IQueryable<ApplicationRole> GetRolesQuery(string searchTerm, List<string> groupRoles)
        {
            searchTerm = searchTerm.ToLower();
            return roles.Where(r => r.Name.ToLower().Contains(searchTerm)).OrderBy(r => r.Name).ToList().Where(r => !groupRoles.Contains(r.Id) || groupRoles == null).AsQueryable();
        }
        //Generate test data
        private IQueryable<ApplicationRole> GenerateRole()
        {
            //Check cache first before regenerating test data
            string cacheKey = "roles";
            if (HttpContext.Current.Cache[cacheKey] != null)
            {
                return (IQueryable<ApplicationRole>)HttpContext.Current.Cache[cacheKey];
            }
            var roles = db.Roles;
            var result = roles.AsQueryable();
            //Cache results
            HttpContext.Current.Cache[cacheKey] = result;

            return result;
        }
        #endregion Private Methods

        #region Public Methods
        //Return only the results we want
        public List<ApplicationRole> GetRoles(string searchTerm, int pageSize, int pageNum, List<string> groupRoles)
        {
            return GetRolesQuery(searchTerm, groupRoles)
                .Skip(pageSize * (pageNum - 1))
                .Take(pageSize)
                .ToList();
        }
        //And the total count of records
        public int GetRolesCount(string searchTerm, int pageSize, int pageNum, List<string> groupRoles)
        {
            return GetRolesQuery(searchTerm, groupRoles).Count();
        }
        public Select2PagedResult ToSelect2Format(List<ApplicationRole> roles, int totalRoles)
        {
            Select2PagedResult json = new Select2PagedResult();
            json.Results = new List<Select2Result>();

            //Loop through our roles and translate it into a text value and an id for the select list
            foreach (ApplicationRole r in roles)
            {
                json.Results.Add(new Select2Result { id = r.Id, text = r.Name });
            }
            //Set the total count of the results from the query.
            json.Total = totalRoles;

            return json;
        }
        #endregion Public Methods
    }
}