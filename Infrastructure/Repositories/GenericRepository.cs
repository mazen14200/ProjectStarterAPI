using Application.Interfaces.Persistence;
using Infrastructure.DbContext;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected readonly AppDbContext _context;
        internal DbSet<T> _dbSet;

        public GenericRepository(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _dbSet = _context.Set<T>();
        }

        // =========================================================
        // QUERY
        // =========================================================

        public IQueryable<T> Table => _dbSet.AsNoTracking();

        public IQueryable<T> Query(bool isTracking = false)
        {
            return isTracking
                ? _dbSet
                : _dbSet.AsNoTracking();
        }

        // =========================================================
        // GET BY ID
        // =========================================================

        public async Task<T?> GetByIdAsync(
            object id,
            bool isTracking = false,
            CancellationToken cancellationToken = default)
        {
            if (id == null)
                throw new ArgumentNullException(nameof(id));

            if (isTracking)
            {
                return await _dbSet.FindAsync(
                    new object[] { id },
                    cancellationToken);
            }

            var entity = await _dbSet.FindAsync(
                new object[] { id },
                cancellationToken);

            if (entity != null)
            {
                _context.Entry(entity).State = EntityState.Detached;
            }

            return entity;
        }

        // =========================================================
        // GET BY INT ID
        // =========================================================

        public async Task<T?> GetByIdAsync(
            int id,
            bool isTracking = false,
            CancellationToken cancellationToken = default)
        {
            return await GetByIdAsync(
                (object)id,
                isTracking,
                cancellationToken);
        }

        // =========================================================
        // GET ASYNC
        // =========================================================

        public async Task<T?> GetAsync(
            Expression<Func<T, bool>> filter,
            string? includeProperties = null,
            bool isTracking = false,
            CancellationToken cancellationToken = default)
        {
            if (filter == null)
                throw new ArgumentNullException(nameof(filter));

            return await GetFirstOrDefaultAsync(
                filter,
                includeProperties,
                isTracking,
                cancellationToken);
        }

        // =========================================================
        // GET BY PREDICATE
        // =========================================================

        public async Task<T?> GetByIdAsync(
            Expression<Func<T, bool>> predicate,
            string? includeProperties = null,
            bool isTracking = false,
            CancellationToken cancellationToken = default)
        {
            return await GetFirstOrDefaultAsync(
                predicate,
                includeProperties,
                isTracking,
                cancellationToken);
        }

        // =========================================================
        // GET FIRST OR DEFAULT
        // =========================================================

        public async Task<T?> GetFirstOrDefaultAsync(
            Expression<Func<T, bool>> predicate,
            string? includeProperties = null,
            bool isTracking = false,
            CancellationToken cancellationToken = default)
        {
            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));

            IQueryable<T> query = isTracking
                ? _dbSet
                : _dbSet.AsNoTracking();

            query = query.Where(predicate);

            query = ApplyIncludes(query, includeProperties);

            return await query.FirstOrDefaultAsync(cancellationToken);
        }

        // =========================================================
        // GET BY COLUMN
        // =========================================================

        public async Task<T?> GetByColumnAsync(
            Expression<Func<T, bool>> predicate,
            string? includeProperties = null,
            bool isTracking = false,
            CancellationToken cancellationToken = default)
        {
            return await GetFirstOrDefaultAsync(
                predicate,
                includeProperties,
                isTracking,
                cancellationToken);
        }

        // =========================================================
        // GET ALL
        // =========================================================

        public async Task<IEnumerable<T>> GetAllAsync(
            Expression<Func<T, bool>>? predicate = null,
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
            string? includeProperties = null,
            bool isTracking = false,
            int? take = null,
            CancellationToken cancellationToken = default)
        {
            IQueryable<T> query = isTracking
                ? _dbSet
                : _dbSet.AsNoTracking();

            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            query = ApplyIncludes(query, includeProperties);

            if (orderBy != null)
            {
                query = orderBy(query);
            }

            if (take.HasValue && take.Value > 0)
            {
                query = query.Take(take.Value);
            }

            return await query.ToListAsync(cancellationToken);
        }

        // =========================================================
        // GET PAGED
        // =========================================================

        public async Task<IEnumerable<T>> GetPagedAsync(
            int pageNumber,
            int pageSize,
            Expression<Func<T, bool>>? predicate = null,
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
            string? includeProperties = null,
            bool isTracking = false,
            CancellationToken cancellationToken = default)
        {
            if (pageNumber <= 0)
                throw new ArgumentOutOfRangeException(nameof(pageNumber));

            if (pageSize <= 0)
                throw new ArgumentOutOfRangeException(nameof(pageSize));

            IQueryable<T> query = isTracking
                ? _dbSet
                : _dbSet.AsNoTracking();

            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            query = ApplyIncludes(query, includeProperties);

            if (orderBy != null)
            {
                query = orderBy(query);
            }

            return await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);
        }

        // =========================================================
        // ANY
        // =========================================================

        public bool Any(Expression<Func<T, bool>> predicate)
        {
            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));

            return _dbSet
                .AsNoTracking()
                .Any(predicate);
        }

        public async Task<bool> AnyAsync(
            Expression<Func<T, bool>> predicate,
            CancellationToken cancellationToken = default)
        {
            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));

            return await _dbSet
                .AsNoTracking()
                .AnyAsync(predicate, cancellationToken);
        }

        // =========================================================
        // COUNT
        // =========================================================

        public async Task<int> CountAsync(
            Expression<Func<T, bool>>? predicate = null,
            CancellationToken cancellationToken = default)
        {
            if (predicate != null)
            {
                return await _dbSet
                    .AsNoTracking()
                    .CountAsync(predicate, cancellationToken);
            }

            return await _dbSet
                .AsNoTracking()
                .CountAsync(cancellationToken);
        }

        // =========================================================
        // ADD
        // =========================================================

        public async Task AddAsync(
            T entity,
            CancellationToken cancellationToken = default)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            await _dbSet.AddAsync(
                entity,
                cancellationToken);
        }

        // =========================================================
        // CREATE + SAVE
        // =========================================================

        public async Task CreateAsync(
            T entity,
            CancellationToken cancellationToken = default)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            await _dbSet.AddAsync(
                entity,
                cancellationToken);

            await SaveAsync(cancellationToken);
        }

        // =========================================================
        // ADD RANGE
        // =========================================================

        public async Task AddRangeAsync(
            IEnumerable<T> entities,
            CancellationToken cancellationToken = default)
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            var entitiesList = entities.ToList();

            if (!entitiesList.Any())
                return;

            await _dbSet.AddRangeAsync(
                entitiesList,
                cancellationToken);
        }

        // =========================================================
        // UPDATE
        // =========================================================

        public void Update(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            _dbSet.Update(entity);
        }

        // =========================================================
        // UPDATE + SAVE
        // =========================================================

        public async Task UpdateAsync(
            T entity,
            CancellationToken cancellationToken = default)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            _dbSet.Update(entity);

            await SaveAsync(cancellationToken);
        }

        // =========================================================
        // UPDATE RANGE
        // =========================================================

        public void UpdateRange(IEnumerable<T> entities)
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            _dbSet.UpdateRange(entities);
        }

        // =========================================================
        // UPDATE RANGE + SAVE
        // =========================================================

        public async Task UpdateRangeAsync(
            IEnumerable<T> entities,
            CancellationToken cancellationToken = default)
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            var entitiesList = entities.ToList();

            if (!entitiesList.Any())
                return;

            _dbSet.UpdateRange(entitiesList);

            await SaveAsync(cancellationToken);
        }

        // =========================================================
        // REMOVE
        // =========================================================

        public void Remove(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            _dbSet.Remove(entity);
        }

        // =========================================================
        // DELETE + SAVE
        // =========================================================

        public async Task DeleteAsync(
            T entity,
            CancellationToken cancellationToken = default)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            _dbSet.Remove(entity);

            await SaveAsync(cancellationToken);
        }

        // =========================================================
        // REMOVE RANGE
        // =========================================================

        public void RemoveRange(IEnumerable<T> entities)
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            _dbSet.RemoveRange(entities);
        }

        // =========================================================
        // DELETE RANGE + SAVE
        // =========================================================

        public async Task DeleteRangeAsync(
            IEnumerable<T> entities,
            CancellationToken cancellationToken = default)
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            var entitiesList = entities.ToList();

            if (!entitiesList.Any())
                return;

            _dbSet.RemoveRange(entitiesList);

            await SaveAsync(cancellationToken);
        }

        // =========================================================
        // SAVE
        // =========================================================

        public async Task SaveAsync(
            CancellationToken cancellationToken = default)
        {
            await _context.SaveChangesAsync(
                cancellationToken);
        }

        // =========================================================
        // INCLUDE HELPER
        // =========================================================

        private static IQueryable<T> ApplyIncludes(
            IQueryable<T> query,
            string? includeProperties)
        {
            if (string.IsNullOrWhiteSpace(includeProperties))
                return query;

            foreach (var includeProperty in includeProperties
                .Split(
                    ',',
                    StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(
                    includeProperty.Trim());
            }

            return query;
        }
    }
}

