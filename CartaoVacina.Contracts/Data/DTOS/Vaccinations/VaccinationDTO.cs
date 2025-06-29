namespace CartaoVacina.Contracts.Data.DTOS.Vaccinations;

public class VaccinationDTO
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int VaccineId { get; set; }
    public string Vaccine { get; set; }
    public DateTime VaccinationDate { get; set; }
    public ushort Dose { get; set; }
}