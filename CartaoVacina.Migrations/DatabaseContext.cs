using System.Reflection;
using CartaoVacina.Contracts.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace CartaoVacina.Migrations;

public class DatabaseContext: DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Vaccine> Vaccines { get; set; }
    public DbSet<Vaccination> Vaccinations { get; set; }
    
    public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
    {
        ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
    }
    
    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        foreach (var item in ChangeTracker.Entries<BaseEntity>().AsEnumerable())
        {
            if (item.Entity.Id == 0)
                item.Entity.CreatedOn = DateTime.UtcNow;
            
            item.Entity.UpdatedOn = DateTime.UtcNow;
        }

        return base.SaveChangesAsync(cancellationToken);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<DatabaseContext>
{
    public DatabaseContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", optional: true, reloadOnChange: true)
            .Build();
        
        var optionsBuilder = new DbContextOptionsBuilder<DatabaseContext>();
        
        optionsBuilder.UseSqlite(configuration.GetConnectionString("DefaultConnection"));
        
        return new DatabaseContext(optionsBuilder.Options);
    }
}