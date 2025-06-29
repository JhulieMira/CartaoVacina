using CartaoVacina.Contracts.Data.Entities;
using CartaoVacina.Contracts.Data.Interfaces.Repositories;
using CartaoVacina.Migrations;

namespace CartaoVacina.Infrastructure.Data.Repositories;

public class VaccinationRepository(DatabaseContext context) : Repository<Vaccination>(context), IVaccinationRepository { }