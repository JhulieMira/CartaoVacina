using CartaoVacina.Contracts.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CartaoVacina.Migrations.Configurations;

public class VaccineConfiguration: IEntityTypeConfiguration<Vaccine>
{
    public void Configure(EntityTypeBuilder<Vaccine> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .ValueGeneratedOnAdd();
        
        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(50);
        
        builder.Property(x => x.Code)
            .IsRequired()
            .HasMaxLength(10);
        
        builder.Property(x => x.Doses)
            .IsRequired();
        
        builder.HasIndex(x => x.Code).IsUnique();
    }
}