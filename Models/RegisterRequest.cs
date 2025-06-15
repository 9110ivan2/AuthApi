namespace AuthApi.Models;

public class RegisterRequest
{
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    // Additional properties can be added as needed
    // For example, you might want to include a ConfirmPassword property for validation
    // public string ConfirmPassword { get; set; } = string.Empty;
}