namespace CartaoVacina.Contracts.Data.DTOS.Vaccines;

public class CreateVaccineDTO
{
    public string Name { get; set; }
    public string Code { get; set; }
    public ushort Doses { get; set; }
    public ushort? MinimumAge { get; set; }
    public ushort? MaximumAge { get; set; }
}