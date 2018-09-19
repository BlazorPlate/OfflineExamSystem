using OfflineExamSystem.Helpers;
using OfflineExamSystem.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OfflineExamSystem.DDL.Repository
{
    public class UserRepository
    {
        #region Private Fields
        private ApplicationDbContext db = new ApplicationDbContext();
        #endregion Private Fields

        #region Public Constructors
        public UserRepository()
        {
            users = GenerateUser();
        }
        #endregion Public Constructors

        #region Public Properties
        public IQueryable<ApplicationUser> users { get; set; }
        #endregion Public Properties

        #region Private Methods
        //Our search term
        private IQueryable<ApplicationUser> GetUsersQuery(string searchTerm)
        {
            searchTerm = searchTerm.ToLower();

            return users
                .Where(
                    m =>
                    m.UserName.ToLower().Contains(searchTerm)
                ).OrderBy(m => m.UserName);
        }
        //Generate test data
        private IQueryable<ApplicationUser> GenerateUser()
        {
            //Check cache first before regenerating test data
            string cacheKey = "users";
            if (HttpContext.Current.Cache[cacheKey] != null)
            {
                return (IQueryable<ApplicationUser>)HttpContext.Current.Cache[cacheKey];
            }
            var users = db.Users;
            var result = users.AsQueryable();
            //Cache results
            HttpContext.Current.Cache[cacheKey] = result;

            return result;
        }
        #endregion Private Methods

        #region Public Methods
        //Return only the results we want
        public List<ApplicationUser> GetUsers(string searchTerm, int pageSize, int pageNum)
        {
            return GetUsersQuery(searchTerm)
                .Skip(pageSize * (pageNum - 1))
                .Take(pageSize)
                .ToList();
        }
        //And the total count of records
        public int GetUsersCount(string searchTerm, int pageSize, int pageNum)
        {
            return GetUsersQuery(searchTerm)
                .Count();
        }
        public Select2PagedResult ToSelect2Format(List<ApplicationUser> users, int totalUsers)
        {
            Select2PagedResult json = new Select2PagedResult();
            json.Results = new List<Select2Result>();

            //Loop through our users and translate it into a text value and an id for the select list
            foreach (ApplicationUser m in users)
            {
                json.Results.Add(new Select2Result { id = m.UserName, text = m.UserName });
            }
            //Set the total count of the results from the query.
            json.Total = totalUsers;

            return json;
        }
        #endregion Public Methods
    }
}