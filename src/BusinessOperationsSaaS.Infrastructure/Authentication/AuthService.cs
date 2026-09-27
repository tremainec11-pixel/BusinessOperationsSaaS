using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BusinessOperationsSaaS.Application.Authentication.DTOs;
using BusinessOperationsSaaS.Application.Authentication.Interfaces;
using BusinessOperationsSaaS.Application.Authentication.Settings;
using BusinessOperationsSaaS.Domain.Entities;
using BusinessOperationsSaaS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace BusinessOperationsSaaS.Infrastructure.Authentication;

public class AuthService : IAuthService
{
    private readonly AppDbContext _context;
    private readonly PasswordHasherService _passwordHasher;
    private readonly JwtSettings _jwtSettings;

    public AuthService(
        AppDbContext context,
        PasswordHasherService passwordHasher,
        IOptions<JwtSettings> jwtOptions)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _jwtSettings = jwtOptions.Value;
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
    {
        var normalizedEmail = request.Email.Trim().ToLowerInvariant();

        var emailExists = await _context.Users
            .AnyAsync(x => x.Email == normalizedEmail);

        if (emailExists)
        {
            throw new InvalidOperationException(
                "A user with this email already exists.");
        }

        var company = new Company
        {
            Id = Guid.NewGuid(),
            Name = request.CompanyName.Trim(),
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        var user = new User
        {
            Id = Guid.NewGuid(),
            CompanyId = company.Id,
            FirstName = request.FirstName.Trim(),
            LastName = request.LastName.Trim(),
            Email = normalizedEmail,
            PasswordHash = _passwordHasher.HashPassword(request.Password),
            Role = "Admin",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _context.Companies.Add(company);
        _context.Users.Add(user);

        await _context.SaveChangesAsync();

        return CreateAuthResponse(user);
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        var normalizedEmail = request.Email.Trim().ToLowerInvariant();

        var user = await _context.Users
            .FirstOrDefaultAsync(x => x.Email == normalizedEmail);

        if (user is null || !user.IsActive)
        {
            throw new UnauthorizedAccessException(
                "Invalid email or password.");
        }

        var passwordValid = _passwordHasher.VerifyPassword(
            request.Password,
            user.PasswordHash);

        if (!passwordValid)
        {
            throw new UnauthorizedAccessException(
                "Invalid email or password.");
        }

        return CreateAuthResponse(user);
    }

    private AuthResponse CreateAuthResponse(User user)
    {
        var token = GenerateToken(user);

        return new AuthResponse
        {
            Token = token,
            UserId = user.Id,
            CompanyId = user.CompanyId,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            Role = user.Role
        };
    }

    private string GenerateToken(User user)
    {
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email),
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
            new(ClaimTypes.Role, user.Role),
            new("companyId", user.CompanyId.ToString())
        };

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_jwtSettings.Key));

        var credentials = new SigningCredentials(
            key,
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(
                _jwtSettings.ExpirationMinutes),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
