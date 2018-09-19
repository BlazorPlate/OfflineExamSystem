using OfflineExamSystem.Helpers;
using OfflineExamSystem.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OfflineExamSystem.DDL.Repository
{
    public class GroupRepository
    {
        #region Private Fields
        private ApplicationDbContext db = new ApplicationDbContext();
        #endregion Private Fields

        #region Public Constructors
        public GroupRepository()
        {
            groups = GenerateGroup();
        }
        #endregion Public Constructors

        #region Public Properties
        public IQueryable<ApplicationGroup> groups { get; set; }
        #endregion Public Properties

        #region Private Methods
        //Our search term
        private IQueryable<ApplicationGroup> GetGroupsQuery(string searchTerm, List<string> userGroups)
        {
            searchTerm = searchTerm.ToLower();

            return groups
                .Where(
                    g =>
                    g.Name.ToLower().Contains(searchTerm)
                ).OrderBy(g => g.Name).ToList().Where(g => !userGroups.Contains(g.Id) || userGroups == null).AsQueryable();
        }
        //Generate test data
        private IQueryable<ApplicationGroup> GenerateGroup()
        {
            //Check cache first before regenerating test data
            string cacheKey = "groups";
            if (HttpContext.Current.Cache[cacheKey] != null)
            {
                return (IQueryable<ApplicationGroup>)HttpContext.Current.Cache[cacheKey];
            }
            var groups = db.ApplicationGroups;
            var result = groups.AsQueryable();
            //Cache results
            HttpContext.Current.Cache[cacheKey] = result;

            return result;
        }
        #endregion Private Methods

        #region Public Methods
        //Return only the results we want
        public List<ApplicationGroup> GetGroups(string searchTerm, int pageSize, int pageNum, List<string> userGroups)
        {
            return GetGroupsQuery(searchTerm, userGroups)
                .Skip(pageSize * (pageNum - 1))
                .Take(pageSize)
                .ToList();
        }
        //And the total count of records
        public int GetGroupsCount(string searchTerm, int pageSize, int pageNum, List<string> userGroups)
        {
            return GetGroupsQuery(searchTerm, userGroups)
                .Count();
        }
        public Select2PagedResult ToSelect2Format(List<ApplicationGroup> groups, int totalGroups)
        {
            Select2PagedResult json = new Select2PagedResult();
            json.Results = new List<Select2Result>();

            //Loop through our groups and translate it into a text value and an id for the select list
            foreach (ApplicationGroup g in groups)
            {
                json.Results.Add(new Select2Result { id = g.Id, text = g.Name });
            }
            //Set the total count of the results from the query.
            json.Total = totalGroups;

            return json;
        }
        #endregion Public Methods
    }
}