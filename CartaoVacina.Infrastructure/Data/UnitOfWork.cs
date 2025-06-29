using CartaoVacina.Contracts.Data;
using CartaoVacina.Contracts.Data.Interfaces.Repositories;
using CartaoVacina.Infrastructure.Data.Repositories;
using CartaoVacina.Migrations;

namespace CartaoVacina.Infrastructure.Data;

public class UnitOfWork(DatabaseContext context) : IUnitOfWork
{
    public IUserRepository Users => new UserRepository(context);
    public IVaccineRepository Vaccines => new VaccineRepository(context);
    public IVaccinationRepository Vaccinations => new VaccinationRepository(context);
    public IAccountRepository Accounts => new AccountRepository(context);

    public async Task CommitAsync(CancellationToken cancellationToken = default)
    {
        await context.SaveChangesAsync(cancellationToken);
    }
}