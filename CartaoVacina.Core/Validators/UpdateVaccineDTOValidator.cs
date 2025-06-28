using CartaoVacina.Contracts.DTOS.Vaccines;
using FluentValidation;

namespace CartaoVacina.Core.Validators;

public class UpdateVaccineDTOValidator: AbstractValidator<UpdateVaccineDTO>
{
    public UpdateVaccineDTOValidator()
    {
        RuleFor(x => x)
            .Must(x => 
                !string.IsNullOrEmpty(x.Name) || 
                !string.IsNullOrEmpty(x.Code) ||
                x.Doses.HasValue ||
                x.MaximumAge.HasValue ||
                x.MinimumAge.HasValue)
            .WithMessage("At least one field must be provided for update.");
        
        When(x => !string.IsNullOrEmpty(x.Name), () =>
        {
            RuleFor(x => x.Name)
                .MaximumLength(100)
                .MinimumLength(3);
        });
        
        When(x => !string.IsNullOrEmpty(x.Code), () =>
        {
            RuleFor(x => x.Code)
                .MaximumLength(10)
                .MinimumLength(3);
        });

        When(x => x.Doses.HasValue, () =>
        {
            RuleFor(x => x.Doses)
                .GreaterThanOrEqualTo((ushort)1);
        });
    }
}