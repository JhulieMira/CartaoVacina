using CartaoVacina.Contracts.Data.DTOS.Vaccinations;
using FluentValidation;

namespace CartaoVacina.Core.Validators.Vaccinations;

public class CreateVaccinationDTOValidator: AbstractValidator<CreateVaccinationDTO>
{
    public CreateVaccinationDTOValidator()
    {
        RuleFor(x => x.VaccineId)
            .GreaterThanOrEqualTo(1);
        
        RuleFor(x => x.Dose)
            .GreaterThanOrEqualTo((ushort)1);
        
        RuleFor(x => x.VaccinationDate)
            .GreaterThan(DateTime.MinValue)
            .LessThan(DateTime.Now)
            .WithMessage("VaccinationDate must be a valid date");
    }
}