using CartaoVacina.Contracts.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CartaoVacina.Migrations.Configurations;

public class UserConfiguration: IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .ValueGeneratedOnAdd();
        
        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(x => x.Gender)
            .IsRequired()
            .HasConversion(
                v => v.ToString(),
                v => Enum.Parse<Gender>(v)
            );
        
        builder.Property(x => x.BirthDate)
            .IsRequired();
    }
}