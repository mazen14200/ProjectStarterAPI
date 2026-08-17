using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Interfaces.Persistence
{
    public interface IGenericRepository<T> where T : class
    {
        // =========================================================
        // QUERY
        // =========================================================

        IQueryable<T> Table { get; }

        IQueryable<T> Query(
            bool isTracking = false);

        // =========================================================
        // READ ACTIONS
        // =========================================================

        Task<T?> GetByIdAsync(
            object id,
            bool isTracking = false,
            CancellationToken cancellationToken = default);

        Task<T?> GetByIdAsync(
            int id,
            bool isTracking = false,
            CancellationToken cancellationToken = default);

        Task<T?> GetAsync(
            Expression<Func<T, bool>> filter,
            string? includeProperties = null,
            bool isTracking = false,
            CancellationToken cancellationToken = default);

        Task<T?> GetByIdAsync(
            Expression<Func<T, bool>> predicate,
            string? includeProperties = null,
            bool isTracking = false,
            CancellationToken cancellationToken = default);

        Task<T?> GetFirstOrDefaultAsync(
            Expression<Func<T, bool>> predicate,
            string? includeProperties = null,
            bool isTracking = false,
            CancellationToken cancellationToken = default);

        Task<T?> GetByColumnAsync(
            Expression<Func<T, bool>> predicate,
            string? includeProperties = null,
            bool isTracking = false,
            CancellationToken cancellationToken = default);

        Task<IEnumerable<T>> GetAllAsync(
            Expression<Func<T, bool>>? predicate = null,
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
            string? includeProperties = null,
            bool isTracking = false,
            int? take = null,
            CancellationToken cancellationToken = default);

        Task<IEnumerable<T>> GetPagedAsync(
            int pageNumber,
            int pageSize,
            Expression<Func<T, bool>>? predicate = null,
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
            string? includeProperties = null,
            bool isTracking = false,
            CancellationToken cancellationToken = default);

        // =========================================================
        // CHECK / COUNT ACTIONS
        // =========================================================

        bool Any(
            Expression<Func<T, bool>> predicate);

        Task<bool> AnyAsync(
            Expression<Func<T, bool>> predicate,
            CancellationToken cancellationToken = default);

        Task<int> CountAsync(
            Expression<Func<T, bool>>? predicate = null,
            CancellationToken cancellationToken = default);

        // =========================================================
        // CREATE / ADD ACTIONS
        // =========================================================

        Task AddAsync(
            T entity,
            CancellationToken cancellationToken = default);

        Task CreateAsync(
            T entity,
            CancellationToken cancellationToken = default);

        Task AddRangeAsync(
            IEnumerable<T> entities,
            CancellationToken cancellationToken = default);

        // =========================================================
        // UPDATE ACTIONS
        // =========================================================

        void Update(T entity);

        Task UpdateAsync(
            T entity,
            CancellationToken cancellationToken = default);

        void UpdateRange(
            IEnumerable<T> entities);

        Task UpdateRangeAsync(
            IEnumerable<T> entities,
            CancellationToken cancellationToken = default);

        // =========================================================
        // DELETE / REMOVE ACTIONS
        // =========================================================

        void Remove(T entity);

        Task DeleteAsync(
            T entity,
            CancellationToken cancellationToken = default);

        void RemoveRange(
            IEnumerable<T> entities);

        Task DeleteRangeAsync(
            IEnumerable<T> entities,
            CancellationToken cancellationToken = default);

        // =========================================================
        // SAVE
        // =========================================================

        Task SaveAsync(
            CancellationToken cancellationToken = default);
    }
}
