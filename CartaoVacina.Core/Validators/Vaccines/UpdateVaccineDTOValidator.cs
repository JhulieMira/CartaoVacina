using CartaoVacina.Contracts.Data.DTOS.Vaccines;
using FluentValidation;

namespace CartaoVacina.Core.Validators.Vaccines;

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
                .NotEmpty().WithMessage("Name field cannot be empty.")
                .MaximumLength(100).WithMessage("Name field must have a maximum of 100 characters.")
                .MinimumLength(3).WithMessage("Name field must have a minimum of 3 characters.");
        });
        
        When(x => !string.IsNullOrEmpty(x.Code), () =>
        {
            RuleFor(x => x.Code)
                .NotEmpty().WithMessage("Code field cannot be empty.")
                .MaximumLength(10).WithMessage("Code field must have a maximum of 10 characters.")
                .MinimumLength(3).WithMessage("Code field must have a minimum of 3 characters.");
        });

        When(x => x.Doses.HasValue, () =>
        {
            RuleFor(x => x.Doses)
                .GreaterThanOrEqualTo((ushort)1).WithMessage("Doses field must be greater than or equal to 1.");
        });
    }
}