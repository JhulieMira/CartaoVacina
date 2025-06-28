using CartaoVacina.Contracts.Data.Repositories;

namespace CartaoVacina.Contracts.Data;

public interface IUnitOfWork
{
    IUserRepository Users { get; }
    IVaccineRepository Vaccines { get; }
    IVaccinationRepository Vaccinations { get; }
    
    Task CommitAsync();
}