using MagicVilla_WebAPI.Models;
using MagicVilla_WebAPI.Models.Dto;

namespace MagicVilla_WebAPI.Repository.IRepository
{
    public interface IUserRepository
    {
        bool IsUniqueUser(string username);
        Task<LoginResponseDTO> Login(LoginRequestDTO loginRequestDTO);
        Task<LocalUser> Register(RegisterationRequestDTO registerationRequestDTO);
    }
}
