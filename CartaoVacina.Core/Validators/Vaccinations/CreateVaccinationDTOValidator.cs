using CartaoVacina.Contracts.Data.DTOS.Vaccinations;
using FluentValidation;

namespace CartaoVacina.Core.Validators.Vaccinations;

public class CreateVaccinationDTOValidator: AbstractValidator<CreateVaccinationDTO>
{
    public CreateVaccinationDTOValidator()
    {
        RuleFor(x => x.VaccineId)
            .GreaterThanOrEqualTo(1)
            .WithMessage("VaccineId field must be greater than or equal to 1.");
        
        RuleFor(x => x.Dose)
            .GreaterThanOrEqualTo((ushort)1)
            .WithMessage("Dose field must be greater than or equal to 1.");
        
        RuleFor(x => x.VaccinationDate)
            .GreaterThan(DateTime.MinValue)
            .WithMessage("VaccinationDate field must be a valid date.")
            .LessThan(DateTime.Now)
            .WithMessage("VaccinationDate field cannot be a future date.");
    }
}