using BusinessOperationsSaaS.Application.Authentication.DTOs;

namespace BusinessOperationsSaaS.Application.Authentication.Interfaces;

public interface IAuthService
{
    Task<AuthResponse> RegisterAsync(RegisterRequest request);

    Task<AuthResponse> LoginAsync(LoginRequest request);
}
