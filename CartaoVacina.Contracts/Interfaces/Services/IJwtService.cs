using CartaoVacina.Contracts.Data.Entities;

namespace CartaoVacina.Core.Services;

public interface IJwtService
{
    string GenerateToken(Account account);
    string GenerateRefreshToken();
    bool ValidateToken(string token);
    string GetEmailFromToken(string token);
} 