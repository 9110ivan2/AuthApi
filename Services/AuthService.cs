using AuthApi.Models;
using AuthApi.Interfaces;
using AuthApi.Data;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using BCrypt.Net;


namespace AuthApi.Services
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _db;
        private readonly IConfiguration _config;
        private readonly HashSet<string> _tokenBlacklist = new();

        public AuthService(AppDbContext db, IConfiguration config)
        {
            _db = db;
            _config = config;
        }

        public async Task<bool> RegisterAsync(RegisterRequest request)
        {
            if (await _db.Users.AnyAsync(u => u.Name == request.Username)) return false;

            var user = new User
            {
                Name = request.Username,
                HashPassword = BCrypt.Net.BCrypt.HashPassword(request.Password)
            }; 
            _db.Users.Add(user);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<string?> LoginAsync(LoginRequest request)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Name == request.Username);
            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.HashPassword))
                return null;

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, user.Name)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: creds
            
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        public void Logout(string token)
        {
            _tokenBlacklist.Add(token);
        }
        public bool IsTokenBlackListed(string token) => _tokenBlacklist.Contains(token);
    }
}
