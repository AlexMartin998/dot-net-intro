using ProductApi.Models;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ProductApi.Data;

namespace ProductApi.Services;


public interface IUserRepository
{
    Task<User> Register(UserRegisterDto userDto);
    Task<AuthResponse> Login(UserLoginDto userDto);
    Task<bool> UserExists(string username);
}

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _config;

    public UserRepository(AppDbContext context, IConfiguration config)
    {
        _context = context;
        _config = config;
    }

    public async Task<User> Register(UserRegisterDto userDto)
    {
        if (await UserExists(userDto.Username))
            throw new ApplicationException("Username already exists");

        if (string.IsNullOrWhiteSpace(userDto.Password))
            throw new ArgumentException("Password cannot be null or empty.");

        CreatePasswordHash(userDto.Password, out byte[] hash, out byte[] salt);

        var user = new User
        {
            Username = userDto.Username,
            PasswordHash = hash,
            PasswordSalt = salt,
            Role = "User"
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return user;
    }

    public async Task<AuthResponse> Login(UserLoginDto userDto)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Username == userDto.Username);

        if (user == null || !VerifyPasswordHash(userDto.Password, user.PasswordHash, user.PasswordSalt))
            throw new ApplicationException("Invalid credentials");

        var token = GenerateJwtToken(user);

        return new AuthResponse
        {
            Token = token,
            Expiration = DateTime.UtcNow.AddMinutes(_config.GetValue<int>("Jwt:ExpireMinutes")),
            Role = user.Role
        };
    }

    public async Task<bool> UserExists(string username)
    {
        return await _context.Users.AnyAsync(u => u.Username == username);
    }

    private void CreatePasswordHash(string password, out byte[] hash, out byte[] salt)
    {
        using var hmac = new HMACSHA512();
        salt = hmac.Key;
        hash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
    }

    private bool VerifyPasswordHash(string password, byte[] hash, byte[] salt)
    {
        using var hmac = new HMACSHA512(salt);
        var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
        return computedHash.SequenceEqual(hash);
    }

    private string GenerateJwtToken(User user)
    {
        var jwtKey = _config.GetSection("Jwt:Key").Value;
        if (string.IsNullOrEmpty(jwtKey))
            throw new ApplicationException("JWT key is not configured.");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Role, user.Role)
        };

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_config.GetValue<int>("Jwt:ExpireMinutes")),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}