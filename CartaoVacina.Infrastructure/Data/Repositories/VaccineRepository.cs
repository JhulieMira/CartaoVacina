using CartaoVacina.Contracts.Data.Entities;
using CartaoVacina.Contracts.Data.Repositories;
using CartaoVacina.Migrations;

namespace CartaoVacina.Infrastructure.Data.Repositories;

public class VaccineRepository(DatabaseContext context) : Repository<Vaccine>(context), IVaccineRepository { }
