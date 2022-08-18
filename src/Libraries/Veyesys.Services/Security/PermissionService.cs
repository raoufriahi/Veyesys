using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Veyesys.Core;
using Veyesys.Core.Caching;
using Veyesys.Core.Domain.Customers;
using Veyesys.Core.Domain.Security;
using Veyesys.Data;
//RRH using Veyesys.Services.Customers;
using Veyesys.Services.Localization;

namespace Veyesys.Services.Security
{
    /// <summary>
    /// Permission service
    /// </summary>
    public partial class PermissionService : IPermissionService
    {
        #region Fields

       //RRH  private readonly ICustomerService _customerService;
        private readonly ILocalizationService _localizationService;
        private readonly IRepository<PermissionRecord> _permissionRecordRepository;
        private readonly IRepository<PermissionRecordCustomerRoleMapping> _permissionRecordCustomerRoleMappingRepository;
        private readonly IStaticCacheManager _staticCacheManager;
        private readonly IWorkContext _workContext;

        #endregion

        #region Ctor

        public PermissionService( ILocalizationService localizationService,
            IRepository<PermissionRecord> permissionRecordRepository,
            IRepository<PermissionRecordCustomerRoleMapping> permissionRecordCustomerRoleMappingRepository,
            IStaticCacheManager staticCacheManager,
            IWorkContext workContext)
        {
            _localizationService = localizationService;
            _permissionRecordRepository = permissionRecordRepository;
            _permissionRecordCustomerRoleMappingRepository = permissionRecordCustomerRoleMappingRepository;
            _staticCacheManager = staticCacheManager;
            _workContext = workContext;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Get permission records by customer role identifier
        /// </summary>
        /// <param name="customerRoleId">Customer role identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the permissions
        /// </returns>
        protected virtual async Task<IList<PermissionRecord>> GetPermissionRecordsByCustomerRoleIdAsync(int customerRoleId)
        {
            var key = _staticCacheManager.PrepareKeyForDefaultCache(VeSecurityDefaults.PermissionRecordsAllCacheKey, customerRoleId);

            var query = from pr in _permissionRecordRepository.Table
                join prcrm in _permissionRecordCustomerRoleMappingRepository.Table on pr.Id equals prcrm
                    .PermissionRecordId
                where prcrm.CustomerRoleId == customerRoleId
                orderby pr.Id
                select pr;

            return await _staticCacheManager.GetAsync(key, async ()=> await query.ToListAsync());
        }

        /// <summary>
        /// Gets a permission
        /// </summary>
        /// <param name="systemName">Permission system name</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the permission
        /// </returns>
        protected virtual async Task<PermissionRecord> GetPermissionRecordBySystemNameAsync(string systemName)
        {
            if (string.IsNullOrWhiteSpace(systemName))
                return null;

            var query = from pr in _permissionRecordRepository.Table
                where pr.SystemName == systemName
                orderby pr.Id
                select pr;

            var permissionRecord = await query.FirstOrDefaultAsync();
            return permissionRecord;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets all permissions
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the permissions
        /// </returns>
        public virtual async Task<IList<PermissionRecord>> GetAllPermissionRecordsAsync()
        {
            var permissions = await _permissionRecordRepository.GetAllAsync(query =>
            {
                return from pr in query
                    orderby pr.Name
                    select pr;
            });

            return permissions;
        }

        /// <summary>
        /// Inserts a permission
        /// </summary>
        /// <param name="permission">Permission</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task InsertPermissionRecordAsync(PermissionRecord permission)
        {
            await _permissionRecordRepository.InsertAsync(permission);
        }

        /// <summary>
        /// Gets a permission record by identifier
        /// </summary>
        /// <param name="permission">Permission</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains a permission record
        /// </returns>
        public virtual async Task<PermissionRecord> GetPermissionRecordByIdAsync(int permissionId)
        {
            return await _permissionRecordRepository.GetByIdAsync(permissionId);
        }

        /// <summary>
        /// Updates the permission
        /// </summary>
        /// <param name="permission">Permission</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task UpdatePermissionRecordAsync(PermissionRecord permission)
        {
            await _permissionRecordRepository.UpdateAsync(permission);
        }

        /// <summary>
        /// Delete a permission
        /// </summary>
        /// <param name="permission">Permission</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task DeletePermissionRecordAsync(PermissionRecord permission)
        {
            await _permissionRecordRepository.DeleteAsync(permission);
        }

        /// <summary>
        /// Install permissions
        /// </summary>
        /// <param name="permissionProvider">Permission provider</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task InstallPermissionsAsync(IPermissionProvider permissionProvider)
        {
            return;
        }

        /// <summary>
        /// Install permissions
        /// </summary>
        /// <param name="permissionProvider">Permission provider</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task UninstallPermissionsAsync(IPermissionProvider permissionProvider)
        {
            return;
        }

        /// <summary>
        /// Authorize permission
        /// </summary>
        /// <param name="permission">Permission record</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the rue - authorized; otherwise, false
        /// </returns>
        public virtual async Task<bool> AuthorizeAsync(PermissionRecord permission)
        {
            return await AuthorizeAsync(permission, await _workContext.GetCurrentCustomerAsync());
        }

        /// <summary>
        /// Authorize permission
        /// </summary>
        /// <param name="permission">Permission record</param>
        /// <param name="customer">Customer</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the rue - authorized; otherwise, false
        /// </returns>
        public virtual async Task<bool> AuthorizeAsync(PermissionRecord permission, Customer customer)
        {
            if (permission == null)
                return false;

            if (customer == null)
                return false;

            return await AuthorizeAsync(permission.SystemName, customer);
        }

        /// <summary>
        /// Authorize permission
        /// </summary>
        /// <param name="permissionRecordSystemName">Permission record system name</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the rue - authorized; otherwise, false
        /// </returns>
        public virtual async Task<bool> AuthorizeAsync(string permissionRecordSystemName)
        {
            return await AuthorizeAsync(permissionRecordSystemName, await _workContext.GetCurrentCustomerAsync());
        }

        /// <summary>
        /// Authorize permission
        /// </summary>
        /// <param name="permissionRecordSystemName">Permission record system name</param>
        /// <param name="customer">Customer</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the rue - authorized; otherwise, false
        /// </returns>
        public virtual async Task<bool> AuthorizeAsync(string permissionRecordSystemName, Customer customer)
        {
          

            //no permission found
            return false;
        }

        /// <summary>
        /// Authorize permission
        /// </summary>
        /// <param name="permissionRecordSystemName">Permission record system name</param>
        /// <param name="customerRoleId">Customer role identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the rue - authorized; otherwise, false
        /// </returns>
        public virtual async Task<bool> AuthorizeAsync(string permissionRecordSystemName, int customerRoleId)
        {
            if (string.IsNullOrEmpty(permissionRecordSystemName))
                return false;

            var key = _staticCacheManager.PrepareKeyForDefaultCache(VeSecurityDefaults.PermissionAllowedCacheKey, permissionRecordSystemName, customerRoleId);

            return await _staticCacheManager.GetAsync(key, async () =>
            {
                var permissions = await GetPermissionRecordsByCustomerRoleIdAsync(customerRoleId);
                foreach (var permission in permissions)
                    if (permission.SystemName.Equals(permissionRecordSystemName, StringComparison.InvariantCultureIgnoreCase))
                        return true;

                return false;
            });
        }

        /// <summary>
        /// Gets a permission record-customer role mapping
        /// </summary>
        /// <param name="permissionId">Permission identifier</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task<IList<PermissionRecordCustomerRoleMapping>> GetMappingByPermissionRecordIdAsync(int permissionId)
        {
            var query = _permissionRecordCustomerRoleMappingRepository.Table;

            query = query.Where(x => x.PermissionRecordId == permissionId);

            return await query.ToListAsync();
        }

        /// <summary>
        /// Delete a permission record-customer role mapping
        /// </summary>
        /// <param name="permissionId">Permission identifier</param>
        /// <param name="customerRoleId">Customer role identifier</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task DeletePermissionRecordCustomerRoleMappingAsync(int permissionId, int customerRoleId)
        {
            var mapping = _permissionRecordCustomerRoleMappingRepository.Table
                .FirstOrDefault(prcm => prcm.CustomerRoleId == customerRoleId && prcm.PermissionRecordId == permissionId);
            if (mapping is null)
                return;

            await _permissionRecordCustomerRoleMappingRepository.DeleteAsync(mapping);
        }

        /// <summary>
        /// Inserts a permission record-customer role mapping
        /// </summary>
        /// <param name="permissionRecordCustomerRoleMapping">Permission record-customer role mapping</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task InsertPermissionRecordCustomerRoleMappingAsync(PermissionRecordCustomerRoleMapping permissionRecordCustomerRoleMapping)
        {
            await _permissionRecordCustomerRoleMappingRepository.InsertAsync(permissionRecordCustomerRoleMapping);
        }

        #endregion
    }
}