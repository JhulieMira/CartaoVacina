using CartaoVacina.Contracts.Data.DTOS.Vaccines;
using FluentValidation;

namespace CartaoVacina.Core.Validators.Vaccines;

public class CreateVaccineDTOValidator: AbstractValidator<CreateVaccineDTO>
{
    public CreateVaccineDTOValidator()
    {
        RuleFor(x => x.Name)
            .MaximumLength(50)
            .MinimumLength(3);
        
        RuleFor(x => x.Code)
            .MaximumLength(10)
            .MinimumLength(3);

        RuleFor(x => x.Doses)
            .GreaterThanOrEqualTo((ushort)1);
    }
}