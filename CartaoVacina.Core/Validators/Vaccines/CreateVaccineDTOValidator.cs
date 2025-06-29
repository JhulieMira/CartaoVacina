using CartaoVacina.Contracts.Data.DTOS.Vaccines;
using FluentValidation;

namespace CartaoVacina.Core.Validators.Vaccines;

public class CreateVaccineDTOValidator: AbstractValidator<CreateVaccineDTO>
{
    public CreateVaccineDTOValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name field is required.")
            .MaximumLength(50).WithMessage("Name field must have a maximum of 50 characters.")
            .MinimumLength(3).WithMessage("Name field must have a minimum of 3 characters.");
        
        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("Code field is required.")
            .MaximumLength(10).WithMessage("Code field must have a maximum of 10 characters.")
            .MinimumLength(3).WithMessage("Code field must have a minimum of 3 characters.");

        RuleFor(x => x.Doses)
            .GreaterThanOrEqualTo((ushort)1).WithMessage("Doses field must be greater than or equal to 1.");
    }
}