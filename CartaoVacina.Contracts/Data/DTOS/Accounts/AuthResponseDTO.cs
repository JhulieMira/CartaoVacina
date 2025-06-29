namespace CartaoVacina.Contracts.Data.DTOS.Accounts;

public class AuthResponseDTO
{
    public string Token { get; set; }
    public string RefreshToken { get; set; }
    public DateTime ExpiresAt { get; set; }
    public AccountInfoDTO Account { get; set; }
}

public class AccountInfoDTO
{
    public int Id { get; set; }
    public string Email { get; set; }
} 