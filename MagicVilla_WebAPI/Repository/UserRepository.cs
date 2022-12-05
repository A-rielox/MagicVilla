﻿using MagicVilla_WebAPI.Data;
using MagicVilla_WebAPI.Models;
using MagicVilla_WebAPI.Models.Dto;
using MagicVilla_WebAPI.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MagicVilla_WebAPI.Repository;

public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _db;
    private string secretKey;

    public UserRepository(ApplicationDbContext db, IConfiguration configuration)
    {
        this._db = db;
        this.secretKey = configuration.GetValue<string>("ApiSettings:Secret");
    }

    ///////////////////////////////////////////////////////////
    //////////////////////////////////////////////////////////////
    public bool IsUniqueUser(string username)
    {
        var user = _db.LocalUsers.FirstOrDefault(u => u.UserName == username);


        // si NO lo encuentra => si va a ser unico
        if (user == null)
            return true;

        return false;
    }

    ///////////////////////////////////////////////////////////
    //////////////////////////////////////////////////////////////
    public async Task<LoginResponseDTO> Login(LoginRequestDTO loginRequestDTO)
    {

        var user = await _db.LocalUsers.FirstOrDefaultAsync(
            u => u.UserName.ToLower() == loginRequestDTO.UserName.ToLower() &&
            u.Password == loginRequestDTO.Password );

        if (user == null)
        {
            return new LoginResponseDTO()
            {
                Token = "",
                User = null
            };
        }

        // if user found generar token
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(secretKey);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, user.Id.ToString()),
                new Claim(ClaimTypes.Role, user.Role)
            }),
            Expires = DateTime.UtcNow.AddDays(7),
            SigningCredentials = new(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);

        LoginResponseDTO loginResponseDTO = new LoginResponseDTO()
        {
            Token = tokenHandler.WriteToken(token),
            User = user
        };

        return loginResponseDTO;
    }

    ///////////////////////////////////////////////////////////
    //////////////////////////////////////////////////////////////

    public async Task<LocalUser> Register(RegisterationRequestDTO registerationRequestDTO)
    {
        LocalUser user = new ()
        {
            UserName = registerationRequestDTO.UserName,
            Name = registerationRequestDTO.Name,
            Password = registerationRequestDTO.Password,
            Role = registerationRequestDTO.Role,
        };

        await _db.LocalUsers.AddAsync(user);
        await _db.SaveChangesAsync();

        user.Password = "";

        return user;
    }
}
