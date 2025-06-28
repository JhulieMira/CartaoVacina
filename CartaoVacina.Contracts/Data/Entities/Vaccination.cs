namespace CartaoVacina.Contracts.Data.Entities;

public class Vaccination: BaseEntity
{
    public int UserId { get; set; }
    public int VaccineId { get; set; }
    public DateTime VaccinationDate { get; set; }
    public ushort Dose { get; set; }
    
    public virtual User User { get; set; }
    public virtual Vaccine Vaccine { get; set; }
}