using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;

namespace OfflineExamSystem.Models
{
    // You will not likely need to customize there, but it is necessary/easier to create our own
    // project-specific implementations, so here they are:
    public class ApplicationUserLogin : IdentityUserLogin<string> { }
    public class ApplicationUserClaim : IdentityUserClaim<string> { }
    public class ApplicationUserRole : IdentityUserRole<string> { }

    // Must be expressed in terms of our custom Role and other types:
    public class ApplicationUser : IdentityUser<string, ApplicationUserLogin, ApplicationUserRole, ApplicationUserClaim>
    {
        #region Public Properties
        public int EmpId { get; set; }
        public string FullName_En { get; set; }
        public string FullName_Ar { get; set; }

        #endregion Public Constructors

        #region Public Properties
        public ApplicationUser()
        {
            this.Id = Guid.NewGuid().ToString();

            // Add any custom User properties/code here
        }
        #endregion Public Constructors

        #region Public Methods
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(ApplicationUserManager manager)
        {
            var userIdentity = await manager
                .CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            return userIdentity;
        }
        #endregion Public Methods
    }

    // Must be expressed in terms of our custom UserRole:
    public class ApplicationRole : IdentityRole<string, ApplicationUserRole>
    {
        #region Public Constructors
        public ApplicationRole()
        {
            this.Id = Guid.NewGuid().ToString();
        }

        public ApplicationRole(string name)
            : this()
        {
            this.Name = name;
        }
        #endregion Public Constructors

        // Add any custom Role properties/code here
    }

    // Must be expressed in terms of our custom types:
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string, ApplicationUserLogin, ApplicationUserRole, ApplicationUserClaim>
    {
        #region Public Constructors
        static ApplicationDbContext()
        {
            //Database.SetInitializer<ApplicationDbContext>(new ApplicationDbInitializer());
        }
        public ApplicationDbContext()
                    : base("OfflineExamDBEntities")
        {
            this.Configuration.LazyLoadingEnabled = false;
        }
        #endregion Public Constructors

        #region Public Properties
        // Add the ApplicationGroups property:
        public virtual IDbSet<ApplicationGroup> ApplicationGroups { get; set; }
        public virtual IDbSet<ApplicationGroupRole> ApplicationGroupRoles { get; set; }
        public System.Data.Entity.DbSet<OfflineExamSystem.Models.ApplicationUserRole> ApplicationUserRole { get; set; }
        public System.Data.Entity.DbSet<OfflineExamSystem.Models.ApplicationUserGroup> ApplicationUserGroups { get; set; }
        #endregion Public Properties

        #region Protected Methods
        //public virtual IDbSet<ApplicationRole> ApplicationRoles { get; set; }
        // Override OnModelsCreating:
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ApplicationGroup>()
                .HasMany<ApplicationUserGroup>((ApplicationGroup g) => g.ApplicationUsers)
                .WithRequired().HasForeignKey<string>((ApplicationUserGroup ag) => ag.ApplicationGroupId);
            modelBuilder.Entity<ApplicationUserGroup>()
                .HasKey((ApplicationUserGroup r) =>
                    new
                    {
                        ApplicationUserId = r.ApplicationUserId,
                        ApplicationGroupId = r.ApplicationGroupId
                    }).ToTable("ApplicationUserGroups");

            modelBuilder.Entity<ApplicationGroup>()
                .HasMany<ApplicationGroupRole>((ApplicationGroup g) => g.ApplicationRoles)
                .WithRequired().HasForeignKey<string>((ApplicationGroupRole ap) => ap.ApplicationGroupId);
            modelBuilder.Entity<ApplicationGroupRole>().HasKey((ApplicationGroupRole gr) =>
                new
                {
                    ApplicationRoleId = gr.ApplicationRoleId,
                    ApplicationGroupId = gr.ApplicationGroupId
                }).ToTable("ApplicationGroupRoles");
        }
        #endregion Protected Methods

        #region Public Methods
        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
        #endregion Public Methods
    }

    // Most likely won't need to customize these either, but they were needed because we implemented
    // custom versions of all the other types:
    public class ApplicationUserStore
        : UserStore<ApplicationUser, ApplicationRole, string,
            ApplicationUserLogin, ApplicationUserRole,
            ApplicationUserClaim>, IUserStore<ApplicationUser, string>,
        IDisposable
    {
        #region Public Constructors
        public ApplicationUserStore()
            : this(new IdentityDbContext())
        {
            base.DisposeContext = true;
        }

        public ApplicationUserStore(DbContext context)
            : base(context)
        {
        }
        #endregion Public Constructors
    }

    public class ApplicationRoleStore
    : RoleStore<ApplicationRole, string, ApplicationUserRole>,
    IQueryableRoleStore<ApplicationRole, string>,
    IRoleStore<ApplicationRole, string>, IDisposable
    {
        #region Public Constructors
        public ApplicationRoleStore()
            : base(new IdentityDbContext())
        {
            base.DisposeContext = true;
        }

        public ApplicationRoleStore(DbContext context)
            : base(context)
        {
        }
        #endregion Public Constructors
    }

    public class ApplicationGroup
    {
        #region Public Constructors
        public ApplicationGroup()
        {
            this.Id = Guid.NewGuid().ToString();
            this.ApplicationRoles = new List<ApplicationGroupRole>();
            this.ApplicationUsers = new List<ApplicationUserGroup>();
        }

        public ApplicationGroup(string name)
            : this()
        {
            this.Name = name;
        }

        public ApplicationGroup(string name, string description)
            : this(name)
        {
            this.Description = description;
        }
        #endregion Public Constructors

        #region Public Properties
        [Key]
        [Required(ErrorMessageResourceType = typeof(ValidationResources), ErrorMessageResourceName = "PropertyValueRequired")]
        public string Id { get; set; }
        [MaxLength(2000)]
        [Display(Name = "Name", ResourceType = typeof(Resources.Resources))]
        [Required(ErrorMessageResourceType = typeof(ValidationResources), ErrorMessageResourceName = "PropertyValueRequired")]
        public string Name { get; set; }
        [Display(Name = "Description", ResourceType = typeof(Resources.Resources))]
        public string Description { get; set; }
        [Display(Name = "Roles", ResourceType = typeof(Resources.Resources))]
        public virtual ICollection<ApplicationGroupRole> ApplicationRoles { get; set; }
        [Display(Name = "Users", ResourceType = typeof(Resources.Resources))]
        public virtual ICollection<ApplicationUserGroup> ApplicationUsers { get; set; }
        #endregion Public Properties
    }

    public class ApplicationUserGroup
    {
        #region Public Properties
        public string ApplicationUserId { get; set; }
        public string ApplicationGroupId { get; set; }
        #endregion Public Properties
    }

    public class ApplicationGroupRole
    {
        #region Public Properties
        public string ApplicationGroupId { get; set; }
        public string ApplicationRoleId { get; set; }
        #endregion Public Properties
    }
}