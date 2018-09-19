using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace OfflineExamSystem.Models
{
    public class ApplicationGroupManager
    {
        #region Private Fields
        private ApplicationGroupStore _groupStore;
        private ApplicationDbContext _db;
        private ApplicationUserManager _userManager;
        private ApplicationRoleManager _roleManager;
        #endregion Private Fields

        #region Public Constructors
        public ApplicationGroupManager()
        {
            _db = HttpContext.Current.GetOwinContext().Get<ApplicationDbContext>();
            _userManager = HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
            _roleManager = HttpContext.Current.GetOwinContext().Get<ApplicationRoleManager>();
            _groupStore = new ApplicationGroupStore(_db);
        }
        #endregion Public Constructors

        #region Public Properties
        public IQueryable<ApplicationGroup> Groups
        {
            get
            {
                return _groupStore.Groups;
            }
        }
        #endregion Public Properties

        #region Public Methods
        public async Task<IdentityResult> CreateGroupAsync(ApplicationGroup group)
        {
            await _groupStore.CreateAsync(group);
            return IdentityResult.Success;
        }

        public IdentityResult CreateGroup(ApplicationGroup group)
        {
            _groupStore.Create(group);
            return IdentityResult.Success;
        }

        public IdentityResult SetGroupRoles(string groupId, params string[] roleNames)
        {
            // Clear all the roles associated with this group:
            var thisGroup = this.FindById(groupId);
            thisGroup.ApplicationRoles.Clear();
            _db.SaveChanges();

            // Add the new roles passed in:
            var newRoles = _roleManager.Roles.Where(r => roleNames.Any(n => n == r.Name));
            foreach (var role in newRoles)
            {
                thisGroup.ApplicationRoles.Add(new ApplicationGroupRole { ApplicationGroupId = groupId, ApplicationRoleId = role.Id });
            }
            _db.SaveChanges();

            // Reset the roles for all affected users:
            foreach (var groupUser in thisGroup.ApplicationUsers)
            {
                this.RefreshUserGroupRoles(groupUser.ApplicationUserId);
            }
            return IdentityResult.Success;
        }

        public async Task<IdentityResult> SetGroupRolesAsync(string groupId, params string[] roleNames)
        {
            // Clear all the roles associated with this group:
            var thisGroup = await this.FindByIdAsync(groupId);
            thisGroup.ApplicationRoles.Clear();
            await _db.SaveChangesAsync();

            // Add the new roles passed in:
            var newRoles = _roleManager.Roles.Where(r => roleNames.Any(id => id == r.Id));
            foreach (var role in newRoles)
            {
                thisGroup.ApplicationRoles.Add(new ApplicationGroupRole { ApplicationGroupId = groupId, ApplicationRoleId = role.Id });
            }
            await _db.SaveChangesAsync();

            // Reset the roles for all affected users:
            foreach (var groupUser in thisGroup.ApplicationUsers)
            {
                await this.RefreshUserGroupRolesAsync(groupUser.ApplicationUserId);
            }
            return IdentityResult.Success;
        }

        public IdentityResult AddGroupRole(string groupId, string roleIds)
        {
            var thisGroup = this.FindById(groupId);
            List<string> roleIdList = roleIds.Split(',').ToList();
            foreach (var roleId in roleIdList)
                thisGroup.ApplicationRoles.Add(new ApplicationGroupRole { ApplicationGroupId = groupId, ApplicationRoleId = roleId });
            _db.SaveChanges();
            // Reset the roles for all affected users:
            foreach (var groupUser in thisGroup.ApplicationUsers)
            {
                this.RefreshUserGroupRoles(groupUser.ApplicationUserId);
            }
            return IdentityResult.Success;
        }
        public async Task<IdentityResult> AddGroupRoleAsync(string groupId, string roleIds)
        {
            var thisGroup = await this.FindByIdAsync(groupId);
            List<string> roleIdList = roleIds.Split(',').ToList();
            foreach (var roleId in roleIdList)
                thisGroup.ApplicationRoles.Add(new ApplicationGroupRole { ApplicationGroupId = groupId, ApplicationRoleId = roleId });
            await _db.SaveChangesAsync();
            // Reset the roles for all affected users:
            foreach (var groupUser in thisGroup.ApplicationUsers)
            {
                this.RefreshUserGroupRoles(groupUser.ApplicationUserId);
            }
            return IdentityResult.Success;
        }
        public IdentityResult RemoveUserGroup(string userId, string groupId)
        {
            var userGroup = _db.ApplicationUserGroups.Where(ug => ug.ApplicationUserId == userId && ug.ApplicationGroupId == groupId).FirstOrDefault();
            _db.ApplicationUserGroups.Remove(userGroup);
            _db.SaveChanges();
            // Reset the roles for the affected user:
            this.RefreshUserGroupRoles(userId);
            return IdentityResult.Success;
        }
        public async Task<IdentityResult> RemoveUserGroupAsync(string userId, string groupId)
        {
            var userGroup = await _db.ApplicationUserGroups.Where(ug => ug.ApplicationUserId == userId && ug.ApplicationGroupId == groupId).FirstOrDefaultAsync();
            _db.ApplicationUserGroups.Remove(userGroup);
            await _db.SaveChangesAsync();
            // Reset the roles for the affected user:
            await this.RefreshUserGroupRolesAsync(userId);
            return IdentityResult.Success;
        }
        public IdentityResult RemoveGroupRole(string groupId, string roleId)
        {
            var groupRole = _db.ApplicationGroupRoles.Where(gr => gr.ApplicationGroupId == groupId && gr.ApplicationRoleId == roleId).FirstOrDefault();
            _db.ApplicationGroupRoles.Remove(groupRole);
            _db.SaveChanges();
            // Reset the roles for all affected users:
            var thisGroup = this.FindById(groupId);
            foreach (var groupUser in thisGroup.ApplicationUsers)
            {
                this.RefreshUserGroupRoles(groupUser.ApplicationUserId);
            }
            return IdentityResult.Success;
        }

        public async Task<IdentityResult> RemoveGroupRoleAsync(string groupId, string roleId)
        {
            var groupRole = await _db.ApplicationGroupRoles.Where(gr => gr.ApplicationGroupId == groupId && gr.ApplicationRoleId == roleId).FirstOrDefaultAsync();
            _db.ApplicationGroupRoles.Remove(groupRole);
            await _db.SaveChangesAsync();
            // Reset the roles for all affected users:
            var thisGroup = this.FindById(groupId);
            foreach (var groupUser in thisGroup.ApplicationUsers)
            {
                await this.RefreshUserGroupRolesAsync(groupUser.ApplicationUserId);
            }
            return IdentityResult.Success;
        }
        public async Task<IdentityResult> SetUserGroupsAsync(string userId, params string[] groupIds)
        {
            // Clear current group membership:
            var currentGroups = await this.GetUserGroupsAsync(userId);
            foreach (var group in currentGroups)
            {
                group.ApplicationUsers.Remove(group.ApplicationUsers.FirstOrDefault(gr => gr.ApplicationUserId == userId));
            }
            await _db.SaveChangesAsync();

            // Add the user to the new groups:
            foreach (string groupId in groupIds)
            {
                var newGroup = await this.FindByIdAsync(groupId);
                newGroup.ApplicationUsers.Add(new ApplicationUserGroup { ApplicationUserId = userId, ApplicationGroupId = groupId });
            }
            await _db.SaveChangesAsync();

            await this.RefreshUserGroupRolesAsync(userId);
            return IdentityResult.Success;
        }

        public IdentityResult SetUserGroups(string userId, params string[] groupIds)
        {
            // Clear current group membership:
            var currentGroups = this.GetUserGroups(userId);
            foreach (var group in currentGroups)
            {
                group.ApplicationUsers
                    .Remove(group.ApplicationUsers.FirstOrDefault(gr => gr.ApplicationUserId == userId));
            }
            _db.SaveChanges();

            // Add the user to the new groups:
            foreach (string groupId in groupIds)
            {
                var newGroup = this.FindById(groupId);
                newGroup.ApplicationUsers.Add(new ApplicationUserGroup { ApplicationUserId = userId, ApplicationGroupId = groupId });
            }
            _db.SaveChanges();

            this.RefreshUserGroupRoles(userId);
            return IdentityResult.Success;
        }

        public IdentityResult SetUserGroup(string groupId, string roleIds)
        {
            var thisGroup = this.FindById(groupId);
            List<string> roleIdList = roleIds.Split(',').ToList();
            foreach (var roleId in roleIdList)
                thisGroup.ApplicationRoles.Add(new ApplicationGroupRole { ApplicationGroupId = groupId, ApplicationRoleId = roleId });
            _db.SaveChanges();
            // Reset the roles for all affected users:
            foreach (var groupUser in thisGroup.ApplicationUsers)
            {
                this.RefreshUserGroupRoles(groupUser.ApplicationUserId);
            }
            return IdentityResult.Success;
        }

        public async Task<IdentityResult> SetUserGroupAsync(string userId, string groupIds)
        {
            List<string> groupIdList = groupIds.Split(',').ToList();
            // Add the user to the new groups:
            foreach (var groupId in groupIdList)
            {
                var newGroup = await this.FindByIdAsync(groupId);
                newGroup.ApplicationUsers.Add(new ApplicationUserGroup { ApplicationUserId = userId, ApplicationGroupId = groupId });
            }
            await _db.SaveChangesAsync();
            await this.RefreshUserGroupRolesAsync(userId);
            return IdentityResult.Success;
        }

        public IdentityResult RefreshUserGroupRoles(string userId)
        {
            var user = _userManager.FindById(userId);
            if (user == null)
            {
                throw new ArgumentNullException("User");
            }
            // Remove user from previous roles:
            var oldUserRoles = _userManager.GetRoles(userId);
            if (oldUserRoles.Count > 0)
            {
                _userManager.RemoveFromRoles(userId, oldUserRoles.ToArray());
            }

            // Find teh roles this user is entitled to from group membership:
            var newGroupRoles = this.GetUserGroupRoles(userId);

            // Get the damn role names:
            var allRoles = _roleManager.Roles.ToList();
            var addTheseRoles = allRoles.Where(r => newGroupRoles.Any(gr => gr.ApplicationRoleId == r.Id));
            var roleNames = addTheseRoles.Select(n => n.Name).ToArray();

            // Add the user to the proper roles
            _userManager.AddToRoles(userId, roleNames);

            return IdentityResult.Success;
        }

        public async Task<IdentityResult> RefreshUserGroupRolesAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                throw new ArgumentNullException("User");
            }
            // Remove user from previous roles:
            var oldUserRoles = await _userManager.GetRolesAsync(userId);
            if (oldUserRoles.Count > 0)
            {
                await _userManager.RemoveFromRolesAsync(userId, oldUserRoles.ToArray());
            }

            // Find the roles this user is entitled to from group membership:
            var newGroupRoles = await this.GetUserGroupRolesAsync(userId);

            // Get the damn role names:
            var allRoles = await _roleManager.Roles.ToListAsync();
            var addTheseRoles = allRoles.Where(r => newGroupRoles.Any(gr => gr.ApplicationRoleId == r.Id));
            var roleNames = addTheseRoles.Select(n => n.Name).ToArray();

            // Add the user to the proper roles
            await _userManager.AddToRolesAsync(userId, roleNames);

            return IdentityResult.Success;
        }

        public async Task<IdentityResult> DeleteGroupAsync(string groupId)
        {
            var group = await this.FindByIdAsync(groupId);
            if (group == null)
            {
                throw new ArgumentNullException("User");
            }

            var currentGroupMembers = (await this.GetGroupUsersAsync(groupId)).ToList();
            // remove the roles from the group:
            group.ApplicationRoles.Clear();

            // Remove all the users:
            group.ApplicationUsers.Clear();

            // Remove the group itself:
            _db.ApplicationGroups.Remove(group);

            await _db.SaveChangesAsync();

            // Reset all the user roles:
            foreach (var user in currentGroupMembers)
            {
                await this.RefreshUserGroupRolesAsync(user.Id);
            }
            return IdentityResult.Success;
        }

        public IdentityResult DeleteGroup(string groupId)
        {
            var group = this.FindById(groupId);
            if (group == null)
            {
                throw new ArgumentNullException("User");
            }

            var currentGroupMembers = this.GetGroupUsers(groupId).ToList();
            // remove the roles from the group:
            group.ApplicationRoles.Clear();

            // Remove all the users:
            group.ApplicationUsers.Clear();

            // Remove the group itself:
            _db.ApplicationGroups.Remove(group);

            _db.SaveChanges();

            // Reset all the user roles:
            foreach (var user in currentGroupMembers)
            {
                this.RefreshUserGroupRoles(user.Id);
            }
            return IdentityResult.Success;
        }

        public async Task<IdentityResult> UpdateGroupAsync(ApplicationGroup group)
        {
            await _groupStore.UpdateAsync(group);
            foreach (var groupUser in group.ApplicationUsers)
            {
                await this.RefreshUserGroupRolesAsync(groupUser.ApplicationUserId);
            }
            return IdentityResult.Success;
        }

        public IdentityResult UpdateGroup(ApplicationGroup group)
        {
            _groupStore.Update(group);
            foreach (var groupUser in group.ApplicationUsers)
            {
                this.RefreshUserGroupRoles(groupUser.ApplicationUserId);
            }
            return IdentityResult.Success;
        }

        public IdentityResult ClearUserGroups(string userId)
        {
            return this.SetUserGroups(userId, new string[] { });
        }

        public async Task<IdentityResult> ClearUserGroupsAsync(string userId)
        {
            return await this.SetUserGroupsAsync(userId, new string[] { });
        }

        public async Task<IEnumerable<ApplicationGroup>> GetUserGroupsAsync(string userId)
        {
            var userGroups = (from g in this.Groups
                              where g.ApplicationUsers.Any(u => u.ApplicationUserId == userId)
                              orderby g.Name
                              select g).ToListAsync();
            return await userGroups;
        }

        public IEnumerable<ApplicationGroup> GetUserGroups(string userId)
        {
            var userGroups = (from g in this.Groups
                              where g.ApplicationUsers.Any(u => u.ApplicationUserId == userId)
                              orderby g.Name
                              select g).ToList();
            return userGroups;
        }
        public UserGroupViewModel GetUserGroup(string userId, string groupId)
        {
            var userGroup = (from g in this.Groups
                             join ug in _db.ApplicationUserGroups on g.Id equals ug.ApplicationGroupId
                             join u in _db.Users on ug.ApplicationUserId equals u.Id
                             where ug.ApplicationGroupId == groupId && ug.ApplicationUserId == userId
                             select new UserGroupViewModel { UserId = u.Id, UserName = u.UserName, GroupId = g.Id, GroupName = g.Name }).FirstOrDefault();
            return userGroup;
        }
        public async Task<UserGroupViewModel> GetUserGroupAsync(string userId, string groupId)
        {
            var userGroup = (from g in this.Groups
                             join ug in _db.ApplicationUserGroups on g.Id equals ug.ApplicationGroupId
                             join u in _db.Users on ug.ApplicationUserId equals u.Id
                             where ug.ApplicationGroupId == groupId && ug.ApplicationUserId == userId
                             select new UserGroupViewModel { UserId = u.Id, UserName = u.UserName, GroupId = g.Id, GroupName = g.Name }).FirstOrDefaultAsync();
            return await userGroup;
        }
        public async Task<GroupRoleViewModel> GetGroupRoleAsync(string groupId, string roleId)
        {
            var groupRole = (from r in _db.Roles
                             join gr in _db.ApplicationGroupRoles on r.Id equals gr.ApplicationRoleId
                             join g in _db.ApplicationGroups on gr.ApplicationGroupId equals g.Id
                             where gr.ApplicationGroupId == groupId && gr.ApplicationRoleId == roleId
                             select new GroupRoleViewModel { RoleId = r.Id, RoleName = r.Name, GroupId = g.Id, GroupName = g.Name }).FirstOrDefaultAsync();
            return await groupRole;
        }

        public GroupRoleViewModel GetGroupRole(string groupId, string roleId)
        {
            var groupRole = (from r in _db.Roles
                             join gr in _db.ApplicationGroupRoles on r.Id equals gr.ApplicationRoleId
                             join g in _db.ApplicationGroups on gr.ApplicationGroupId equals g.Id
                             where gr.ApplicationGroupId == groupId && gr.ApplicationRoleId == roleId
                             select new GroupRoleViewModel { RoleId = r.Id, RoleName = r.Name, GroupId = g.Id, GroupName = g.Name }).FirstOrDefault();
            return groupRole;
        }

        public async Task<IEnumerable<ApplicationRole>> GetGroupRolesAsync(string groupId)
        {
            var grp = await _db.ApplicationGroups.Include(g => g.ApplicationRoles).FirstOrDefaultAsync(g => g.Id == groupId);
            var roles = await _roleManager.Roles.ToListAsync();
            var groupRoles = (from r in roles
                              where grp.ApplicationRoles.Any(ap => ap.ApplicationRoleId == r.Id)
                              orderby r.Name
                              select r).ToList();
            return groupRoles;
        }

        public IEnumerable<ApplicationRole> GetGroupRoles(string groupId)
        {
            var grp = _db.ApplicationGroups.Include(g => g.ApplicationRoles).FirstOrDefault(g => g.Id == groupId);
            var roles = _roleManager.Roles.ToList();
            var groupRoles = from r in roles
                             where grp.ApplicationRoles.Any(ap => ap.ApplicationRoleId == r.Id)
                             orderby r.Name
                             select r;
            return groupRoles;
        }

        public IEnumerable<ApplicationUser> GetGroupUsers(string groupId)
        {
            var group = this.FindById(groupId);
            var users = new List<ApplicationUser>();
            foreach (var groupUser in group.ApplicationUsers)
            {
                var user = _db.Users.Find(groupUser.ApplicationUserId);
                users.Add(user);
            }
            return users;
        }

        public async Task<IEnumerable<ApplicationUser>> GetGroupUsersAsync(string groupId)
        {
            var group = await this.FindByIdAsync(groupId);
            var users = new List<ApplicationUser>();
            foreach (var groupUser in group.ApplicationUsers)
            {
                var user = await _db.Users
                    .FirstOrDefaultAsync(u => u.Id == groupUser.ApplicationUserId);
                users.Add(user);
            }
            return users;
        }

        public IEnumerable<ApplicationGroupRole> GetUserGroupRoles(string userId)
        {
            var userGroups = this.GetUserGroups(userId);
            var userGroupRoles = new List<ApplicationGroupRole>();
            foreach (var group in userGroups)
            {
                userGroupRoles.AddRange(group.ApplicationRoles.ToArray());
            }
            return userGroupRoles;
        }

        public async Task<IEnumerable<ApplicationGroupRole>> GetUserGroupRolesAsync(string userId)
        {
            var userGroups = await this.GetUserGroupsAsync(userId);
            var userGroupRoles = new List<ApplicationGroupRole>();
            foreach (var group in userGroups)
            {
                userGroupRoles.AddRange(group.ApplicationRoles.ToArray());
            }
            return userGroupRoles;
        }

        public async Task<ApplicationGroup> FindByIdAsync(string id)
        {
            return await _groupStore.FindByIdAsync(id);
        }

        public ApplicationGroup FindById(string id)
        {
            return _groupStore.FindById(id);
        }

        public bool GroupExist(string id, string name)
        {
            return _db.ApplicationGroups.Where(g => g.Id != id || string.IsNullOrEmpty(id)).Any(g => g.Name == name || string.IsNullOrEmpty(name));
        }
        public async Task<bool> GroupExistAsync(string id, string name)
        {
            return await _db.ApplicationGroups.Where(g => g.Id != id || string.IsNullOrEmpty(id)).AnyAsync(g => g.Name == name || string.IsNullOrEmpty(name));
        }
        #endregion Public Methods
    }
}