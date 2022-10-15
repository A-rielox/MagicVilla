using MagicVilla_WebAPI.Data;
using MagicVilla_WebAPI.Models;
using MagicVilla_WebAPI.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace MagicVilla_WebAPI.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly ApplicationDbContext _db;
        internal DbSet<T> dbSet;

        public Repository(ApplicationDbContext db)
        {
            this._db = db;
            this.dbSet = _db.Set<T>();
        }

        ////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////
        public async Task CreateAsync(T entity)
        {
            await dbSet.AddAsync(entity);
            await SaveAsync();
        }

        ////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////
        public async Task<T> GetAsync(Expression<Func<T, bool>> filter = null,
            bool tracked = true,
            string? includeProperties = null)
        {
            IQueryable<T> query = dbSet;

            if (!tracked)
                query = query.AsNoTracking();

            if (filter != null)
                query = query.Where(filter);

            // al necesitar incluir props vendrian como "Villa, VillaSpacial"
            if(includeProperties != null)
            {
                foreach (var includeProp in includeProperties.Split(new char[] {','}, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProp);
                }
            }

            return await query.FirstOrDefaultAsync();
        }

        ////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////
        public async Task<List<T>> GetAllAsync(
            Expression<Func<T,bool>>? filter = null,
            string? includeProperties = null )
        {
            // el IQueriable NO se ejecuta altiro => despues se le pone el filtro
            IQueryable<T> query = dbSet;

            if (filter != null)
                query = query.Where(filter);

            // al necesitar incluir varias props vendrian como "Villa, VillaSpacial"
            if (includeProperties != null)
            {
                foreach (var includeProp in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProp);
                }
            }

            return await query.ToListAsync();
        }

        ////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////
        public async Task RemoveAsync(T entity)
        {
            dbSet.Remove(entity);
            await SaveAsync();
        }

        ////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////
        public async Task SaveAsync()
        {
            await _db.SaveChangesAsync();
        }
    }
}
