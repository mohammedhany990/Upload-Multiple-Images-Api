using Microsoft.EntityFrameworkCore;
using OgTech.Core.Entities;
using OgTech.Core.Repo;
using OgTech.Repository.Identity;
using System.Security.Principal;
using OgTech.Repository.Data;

namespace OgTech.Repository
{
    public class GenericRepository<T> : IGenericRepository<T> where T : IEntity
    
    {
        private readonly ImageDbContext _dbContext;

        public GenericRepository(ImageDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task AddAsync(T item)
        {

            await _dbContext.Set<T>().AddAsync(item);
            await _dbContext.SaveChangesAsync();
        }


        public void Delete(T item)
        {
            _dbContext.Set<T>().Remove(item);
            _dbContext.SaveChangesAsync();
        }

        public async Task<IReadOnlyList<T>> GetAllAsync()
        {
            return await _dbContext.Set<T>().ToListAsync();
        }

        public async Task<T?> GetByNameAsync(string name)
        {
            return await _dbContext.Images.FirstOrDefaultAsync(n => n.Name == name) as T;
        }

    }
}
