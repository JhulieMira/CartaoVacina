namespace CartaoVacina.Contracts.Data.DTOS.Vaccinations;

public class CreateVaccinationDTO
{
    public int VaccineId { get; set; }
    public DateTime VaccinationDate { get; set; }
    public ushort Dose { get; set; }
}