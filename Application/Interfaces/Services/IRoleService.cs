using Application.DTOs.Role;

namespace Application.Interfaces.Services
{
    /// <summary>
    /// Interface for role management operations
    /// Defines the contract for role-related business logic
    /// </summary>
    public interface IRoleService
    {
        #region CRUD Operations

        #region Create
        /// <summary>
        /// Creates a new role in the system
        /// </summary>
        /// <param name="roleDto">Data transfer object containing role information</param>
        /// <returns>True if role creation successful, false otherwise</returns>
        Task<bool> CreateRoleAsync(CreateRoleDTO roleDto);
        #endregion

        #region Update
        /// <summary>
        /// Updates an existing role's information
        /// </summary>
        /// <param name="roleDto">Data transfer object with updated role information</param>
        /// <returns>True if role update successful, false otherwise</returns>
        Task<bool> UpdateRoleAsync(UpdateRoleDTO roleDto);
        #endregion

        #region Delete Operations
        /// <summary>
        /// Soft deletes a role (marks as deleted without permanent removal)
        /// </summary>
        /// <param name="id">ID of the role to soft delete</param>
        /// <returns>True if soft delete successful, false otherwise</returns>
        Task<bool> SoftDeleteRoleAsync(string id);

        /// <summary>
        /// Permanently deletes a role from the system
        /// </summary>
        /// <param name="id">ID of the role to hard delete</param>
        /// <returns>True if hard delete successful, false otherwise</returns>
        Task<bool> HardDeleteRoleAsync(string id);

        /// <summary>
        /// Restores a previously soft-deleted role
        /// </summary>
        /// <param name="id">ID of the role to restore</param>
        /// <returns>True if restoration successful, false otherwise</returns>
        Task<bool> RestoreRoleAsync(string id);
        #endregion

        #region Read Operations

        #region Get All Roles
        /// <summary>
        /// Retrieves all active (non-deleted) roles from the system
        /// </summary>
        /// <returns>List of RoleDTO objects representing active roles</returns>
        Task<List<RoleDTO>> GetAllRolesAsync();

        /// <summary>
        /// Retrieves all roles including deleted ones
        /// </summary>
        /// <returns>List of RoleDTO objects with deletion status</returns>
        Task<List<RoleDTO>> GetAllRolesWithDeletedAsync();
        #endregion

        #region Get Role By ID
        /// <summary>
        /// Retrieves a specific active role by its ID
        /// </summary>
        /// <param name="id">ID of the role to retrieve</param>
        /// <returns>RoleDTO object if found, null otherwise</returns>
        Task<RoleDTO> GetRoleByIdAsync(string id);

        /// <summary>
        /// Retrieves a specific deleted role by its ID
        /// </summary>
        /// <param name="id">ID of the deleted role to retrieve</param>
        /// <returns>RoleDTO object if found, null otherwise</returns>
        Task<RoleDTO> GetDeletedRoleByIdAsync(string id);
        #endregion

        #endregion

        #region Validation Methods
        /// <summary>
        /// Checks if a role name already exists in the system
        /// </summary>
        /// <param name="name">Role name to check for existence</param>
        /// <param name="excludeId">Optional role ID to exclude from the check (useful for update operations)</param>
        /// <returns>True if the role name exists, false otherwise</returns>
        Task<bool> RoleNameExistsAsync(string name, string excludeId = null);
        #endregion
        Task<byte[]> ExportRolesToExcelAsync();
        #endregion
    }
}
