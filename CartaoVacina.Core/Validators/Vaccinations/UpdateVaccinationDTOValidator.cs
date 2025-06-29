using CartaoVacina.Contracts.Data.DTOS.Vaccinations;
using FluentValidation;

namespace CartaoVacina.Core.Validators.Vaccinations;

public class UpdateVaccinationDTOValidator: AbstractValidator<UpdateVaccinationDTO>
{
    public UpdateVaccinationDTOValidator()
    {
        RuleFor(x => x)
            .Must(x => 
                x.VaccinationDate.HasValue)
            .WithMessage("At least one field must be provided for update.");
        
        When(x => x.VaccinationDate.HasValue, () =>
        {
            RuleFor(x => x.VaccinationDate)
                .GreaterThan(DateTime.MinValue).WithMessage("VaccinationDate field must be a valid date.")
                .LessThan(DateTime.Now).WithMessage("VaccinationDate field cannot be a future date.");
        });
    }
}