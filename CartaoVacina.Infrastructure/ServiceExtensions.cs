using CartaoVacina.Contracts.Data.Interfaces;
using CartaoVacina.Infrastructure.Data;
using CartaoVacina.Migrations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CartaoVacina.Infrastructure;

public static class ServiceExtensions
{
    private static IServiceCollection AddUnitOfWork(this IServiceCollection services)
    {
        return services.AddScoped<IUnitOfWork, UnitOfWork>();
    }

    private static IServiceCollection AddDatabaseContext(this IServiceCollection services, IConfiguration configuration)
    {
        return services.AddSqlite<DatabaseContext>(configuration.GetConnectionString("DefaultConnection"), (options) =>
        {
            options.MigrationsAssembly("CartaoVacina.Migrations");
        });
    }

    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        return services.AddDatabaseContext(configuration).AddUnitOfWork();
    }
}