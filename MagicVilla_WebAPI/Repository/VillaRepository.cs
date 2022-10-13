using MagicVilla_WebAPI.Data;
using MagicVilla_WebAPI.Models;
using MagicVilla_WebAPI.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace MagicVilla_WebAPI.Repository
{
    public class VillaRepository : IVillaRepository
    {
        private readonly ApplicationDbContext _db;

        public VillaRepository(ApplicationDbContext db)
        {
            this._db = db;
        }

        ////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////
        public async Task CreateAsync(Villa entity)
        {
            await _db.Villas.AddAsync(entity);
            await SaveAsync();
        }

        ////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////
        public async Task<Villa> GetAsync(Expression<Func<Villa, bool>> filter = null, bool tracked = true)
        {
            IQueryable<Villa> query = _db.Villas;

            if (!tracked)
                query = query.AsNoTracking();

            if(filter != null)
                query = query.Where(filter);

            return await query.FirstOrDefaultAsync();
        }

        ////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////
        public async Task<List<Villa>> GetAllAsync(Expression<Func<Villa, bool>> filter = null)
        {
            // el IQueriable NO se ejecuta altiro => despues se le pone el filtro
            IQueryable<Villa> query = _db.Villas;

            if(filter != null)
                query = query.Where(filter);

            return await query.ToListAsync();
        }

        ////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////
        public async Task RemoveAsync(Villa entity)
        {
            _db.Villas.Remove(entity);
            await SaveAsync();
        }

        ////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////
        public async Task SaveAsync()
        {
            await _db.SaveChangesAsync();
        }

        ////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////
        public async Task UpdateAsync(Villa entity)
        {
            _db.Villas.Update(entity);
            await SaveAsync();
        }
    }
}
