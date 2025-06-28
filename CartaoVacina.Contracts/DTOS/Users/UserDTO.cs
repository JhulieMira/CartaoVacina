using CartaoVacina.Contracts.DTOS.Vaccinations;

namespace CartaoVacina.Contracts.DTOS.Users;

public class UserDTO
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Gender { get; set; }
    public DateTime BirthDate { get; set; }
    
    public List<VaccinationDTO> Vaccinations { get; set; }
}