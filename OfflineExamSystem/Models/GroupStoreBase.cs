using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace OfflineExamSystem.Models
{
    public class GroupStoreBase
    {
        #region Public Constructors
        public GroupStoreBase(DbContext context)
        {
            this.Context = context;
            this.DbEntitySet = context.Set<ApplicationGroup>();
        }
        #endregion Public Constructors

        #region Public Properties
        public DbContext Context
        {
            get;
            private set;
        }

        public DbSet<ApplicationGroup> DbEntitySet
        {
            get;
            private set;
        }

        public IQueryable<ApplicationGroup> EntitySet
        {
            get
            {
                return this.DbEntitySet.Include(g => g.ApplicationUsers).Include(g => g.ApplicationRoles);
            }
        }
        #endregion Public Properties

        #region Public Methods
        public void Create(ApplicationGroup entity)
        {
            this.DbEntitySet.Add(entity);
        }

        public void Delete(ApplicationGroup entity)
        {
            this.DbEntitySet.Remove(entity);
        }

        public virtual Task<ApplicationGroup> GetByIdAsync(string id)
        {
            return this.DbEntitySet.Include(g => g.ApplicationRoles).Include(g => g.ApplicationUsers).Where(g => g.Id == id).FirstOrDefaultAsync();
        }

        public virtual ApplicationGroup GetById(string id)
        {
            return this.DbEntitySet.Include(g => g.ApplicationRoles).Include(g => g.ApplicationUsers).Where(g => g.Id == id).FirstOrDefault();
        }

        public virtual void Update(ApplicationGroup entity)
        {
            if (entity != null)
            {
                this.Context.Entry<ApplicationGroup>(entity).State = EntityState.Modified;
            }
        }
        #endregion Public Methods
    }
}