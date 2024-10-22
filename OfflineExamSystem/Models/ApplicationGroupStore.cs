﻿using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace OfflineExamSystem.Models
{
    public class ApplicationGroupStore : IDisposable
    {
        #region Private Fields
        private bool _disposed;
        private GroupStoreBase _groupStore;
        #endregion Private Fields

        #region Public Constructors
        public ApplicationGroupStore(DbContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            this.Context = context;
            this._groupStore = new GroupStoreBase(context);
        }
        #endregion Public Constructors

        #region Public Properties
        public IQueryable<ApplicationGroup> Groups
        {
            get
            {
                return this._groupStore.EntitySet;
            }
        }

        public DbContext Context
        {
            get;
            private set;
        }

        public bool DisposeContext
        {
            get;
            set;
        }
        #endregion Public Properties

        #region Private Methods
        // DISPOSE STUFF: ===============================================
        private void ThrowIfDisposed()
        {
            if (this._disposed)
            {
                throw new ObjectDisposedException(this.GetType().Name);
            }
        }
        #endregion Private Methods

        #region Protected Methods
        protected virtual void Dispose(bool disposing)
        {
            if (this.DisposeContext && disposing && this.Context != null)
            {
                this.Context.Dispose();
            }
            this._disposed = true;
            this.Context = null;
            this._groupStore = null;
        }
        #endregion Protected Methods

        #region Public Methods
        public virtual void Create(ApplicationGroup group)
        {
            this.ThrowIfDisposed();
            if (group == null)
            {
                throw new ArgumentNullException("role");
            }
            this._groupStore.Create(group);
            this.Context.SaveChanges();
        }

        public virtual async Task CreateAsync(ApplicationGroup group)
        {
            this.ThrowIfDisposed();
            if (group == null)
            {
                throw new ArgumentNullException("role");
            }
            this._groupStore.Create(group);
            await this.Context.SaveChangesAsync();
        }

        public virtual async Task DeleteAsync(ApplicationGroup group)
        {
            this.ThrowIfDisposed();
            if (group == null)
            {
                throw new ArgumentNullException("group");
            }
            this._groupStore.Delete(group);
            await this.Context.SaveChangesAsync();
        }

        public virtual void Delete(ApplicationGroup group)
        {
            this.ThrowIfDisposed();
            if (group == null)
            {
                throw new ArgumentNullException("group");
            }
            this._groupStore.Delete(group);
            this.Context.SaveChanges();
        }

        public Task<ApplicationGroup> FindByIdAsync(string roleId)
        {
            this.ThrowIfDisposed();
            return this._groupStore.GetByIdAsync(roleId);
        }

        public ApplicationGroup FindById(string groupId)
        {
            this.ThrowIfDisposed();
            return this._groupStore.GetById(groupId);
        }

        public Task<ApplicationGroup> FindByNameAsync(string groupName)
        {
            this.ThrowIfDisposed();
            return QueryableExtensions
                .FirstOrDefaultAsync<ApplicationGroup>(this._groupStore.EntitySet,
                    (ApplicationGroup u) => u.Name.ToUpper() == groupName.ToUpper());
        }

        public virtual async Task UpdateAsync(ApplicationGroup group)
        {
            this.ThrowIfDisposed();
            if (group == null)
            {
                throw new ArgumentNullException("group");
            }
            this._groupStore.Update(group);
            await this.Context.SaveChangesAsync();
        }

        public virtual void Update(ApplicationGroup group)
        {
            this.ThrowIfDisposed();
            if (group == null)
            {
                throw new ArgumentNullException("group");
            }
            this._groupStore.Update(group);
            this.Context.SaveChanges();
        }
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion Public Methods
    }
}