using Infrastructure.DbContext;
using Infrastructure.Identity;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Interfaces.Persistence;

namespace Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        private readonly Dictionary<Type, object> _repositories = new();

        public UnitOfWork(AppDbContext context)
        {
            _context = context;
        }

        public IGenericRepository<T> GetRepository<T>() where T : class
        {
            var type = typeof(T);

            if (_repositories.TryGetValue(type, out var existingRepository))
            {
                return (IGenericRepository<T>)existingRepository;
            }

            var repository = new GenericRepository<T>(_context);
            _repositories[type] = repository;
            return repository;
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public int SaveChanges()
        {
            return _context.SaveChanges();
        }

        public void Dispose()
        {
            _context.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
