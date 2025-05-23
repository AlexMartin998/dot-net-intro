using System.ComponentModel.DataAnnotations;

namespace ProductApi.Models;

public class User
{
    public int Id { get; set; }

    [Required]
    public required string Username { get; set; }

    [Required]
    public required byte[] PasswordHash { get; set; }

    [Required]
    public required byte[] PasswordSalt { get; set; }

    public string Role { get; set; } = "User";
}

public class UserRegisterDto
{
    [Required]
    public required string Username { get; set; }

    [Required]
    [StringLength(100, MinimumLength = 6)]
    public string? Password { get; set; }
}

public class UserLoginDto
{
    [Required]
    public required string Username { get; set; }

    [Required]
    public required string Password { get; set; }
}

public class AuthResponse
{
    public required string Token { get; set; }
    public DateTime Expiration { get; set; }
    public required string Role { get; set; }
}