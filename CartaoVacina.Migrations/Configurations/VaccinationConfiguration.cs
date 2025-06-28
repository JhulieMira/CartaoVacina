using CartaoVacina.Contracts.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CartaoVacina.Migrations.Configurations;

public class VaccinationConfiguration: IEntityTypeConfiguration<Vaccination>
{
    public void Configure(EntityTypeBuilder<Vaccination> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.VaccinationDate)
            .IsRequired();

        builder.HasOne(x => x.Vaccine)
            .WithMany()
            .HasForeignKey(x => x.VaccineId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}