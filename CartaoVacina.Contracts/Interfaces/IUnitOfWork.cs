using CartaoVacina.Contracts.Data.Interfaces.Repositories;

namespace CartaoVacina.Contracts.Data.Interfaces;

public interface IUnitOfWork
{
    IUserRepository Users { get; }
    IVaccineRepository Vaccines { get; }
    IVaccinationRepository Vaccinations { get; }
    IAccountRepository Accounts { get; }
    
    Task CommitAsync(CancellationToken cancellationToken = default);
}