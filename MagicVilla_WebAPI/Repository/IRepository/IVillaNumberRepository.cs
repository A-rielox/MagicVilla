using MagicVilla_WebAPI.Models;
using System.Linq.Expressions;

namespace MagicVilla_WebAPI.Repository.IRepository
{
    public interface IVillaNumberRepository : IRepository<VillaNumber>
    {
        Task<VillaNumber> UpdateAsync(VillaNumber entity);
    }
}
