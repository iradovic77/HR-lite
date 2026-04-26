namespace IdentityService.DTOs;

public record LoginRequest(string Email, string Password);

public record LoginResponse(
    string AccessToken,
    DateTime ExpiresAt,
    string Email,
    IEnumerable<string> Roles
);
