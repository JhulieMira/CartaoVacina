namespace CartaoVacina.Contracts.Data.Entities;

public class Account : BaseEntity
{
    public string Email { get; set; }
    public string PasswordHash { get; set; }
    public string Salt { get; set; }
    public bool IsActive { get; set; }
    public DateTime? LastLogin { get; set; }
} 