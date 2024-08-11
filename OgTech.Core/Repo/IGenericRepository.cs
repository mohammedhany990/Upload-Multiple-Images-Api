using OgTech.Core.Entities;

namespace OgTech.Core.Repo
{
    public interface IGenericRepository<T> where T:IEntity
    {
        Task AddAsync(T item);
        void Delete(T item);
        Task<IReadOnlyList<T>> GetAllAsync();
        Task<T?> GetByNameAsync(string name);
    }
}
