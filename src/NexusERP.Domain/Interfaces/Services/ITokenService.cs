namespace NexusERP.Domain.Interfaces.Services;

public interface ITokenService
{
    string GenerateAccessToken(int userId, string username, IEnumerable<string> roles);
    string GenerateRefreshToken();
    int? GetUserIdFromAccessToken(string token);
}
