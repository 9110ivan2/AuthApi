using AuthApi.Models;

namespace AuthApi.Interfaces;

public interface  IAuthService
{
    Task<bool> RegisterAsync(RegisterRequest request);
    Task<string?>LoginAsync(LoginRequest request);
    void Logout(string Token); // test  adasd a 
}