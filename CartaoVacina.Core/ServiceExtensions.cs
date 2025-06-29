using System.Reflection;
using CartaoVacina.Core.Services;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace CartaoVacina.Core;

public static class ServiceExtensions
{
    public static IServiceCollection AddCoreServices(this IServiceCollection services)
    {
        return services
            .AddValidatorsFromAssembly(Assembly.GetExecutingAssembly())
            .AddAutoMapper(Assembly.GetExecutingAssembly())
            .AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()))
            .AddScoped<IJwtService, JwtService>()
            .AddScoped<IPasswordService, PasswordService>();
    }
}