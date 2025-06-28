namespace CartaoVacina.Contracts.Data.Entities;

public class User: BaseEntity
{
    public string Name { get; set; }
    public Gender Gender { get; set; }
    public DateTime BirthDate { get; set; }
    
    public virtual ICollection<Vaccination> Vaccinations { get; set; }
}

public enum Gender
{
    Male,
    Female,
    Other
}