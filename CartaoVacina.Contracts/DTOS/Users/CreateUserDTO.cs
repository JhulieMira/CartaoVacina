namespace CartaoVacina.Contracts.DTOS.Users;

public class CreateUserDTO
{
    public string Name { get; set; }
    public DateTime BirthDate { get; set; }
    public string Gender { get; set; }
}