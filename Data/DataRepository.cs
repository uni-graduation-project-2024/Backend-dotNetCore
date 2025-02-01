using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Learntendo_backend.Data
{
    public class DataRepository<T> : IDataRepository<T> where T : class
    {
        private readonly DataContext _db;
        private readonly DbSet<T> table;

        public DataRepository(DataContext db)
        {
           _db = db;
            table =_db.Set<T>();
        }

        public async Task<IEnumerable<T>> GetAll()
        {
            return await table.ToListAsync();
        }

        public async Task<T> GetById(int id)
        {
            var result = await table.FindAsync(id);
            if (result == null)
            {
                throw new KeyNotFoundException($" id {id} not found");
            }
            return result;
            
        }

        public async Task Add(T entity)
        {
            await table.AddAsync(entity);
            await _db.SaveChangesAsync();
        }

        public async Task Update(T entity)
        {
            table.Update(entity);
            await _db.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            var entity = await GetById(id);
            if (entity != null)
            {
                table.Remove(entity);
                await _db.SaveChangesAsync();
            }
        }
    }

}
