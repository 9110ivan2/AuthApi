namespace AuthApi.Models;

public class  LoginRequest
{
    public required string Username { get; set; }
    public string Password { get; set; } = string.Empty;
}