using System;
using System.Threading.Tasks;

namespace Application.Interfaces.Persistence
{
    public interface IUnitOfWork : IDisposable
    {

        IGenericRepository<T> GetRepository<T>() where T : class;

        Task<int> SaveChangesAsync();
        int SaveChanges();
    }
}
