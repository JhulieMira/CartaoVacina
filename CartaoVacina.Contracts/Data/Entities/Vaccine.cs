namespace CartaoVacina.Contracts.Data.Entities;

public class Vaccine: BaseEntity
{
    public string Name { get; set; }
    public string Code { get; set; }
    public ushort Doses { get; set; }
    public ushort? MinimumAge { get; set; }
    public ushort? MaximumAge { get; set; }
    
    public virtual ICollection<Vaccination> Vaccinations { get; set; }
}