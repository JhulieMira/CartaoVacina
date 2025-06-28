using CartaoVacina.Contracts.DTOS.Users;
using FluentValidation;

namespace CartaoVacina.Core.Validators;

public class UpdateUserDTOValidator : AbstractValidator<UpdateUserDTO>
{
    public UpdateUserDTOValidator()
    {
        RuleFor(x => x)
            .Must(x => !string.IsNullOrEmpty(x.Name) || x.BirthDate.HasValue)
            .WithMessage("At least one field (Name or BirthDate) must be provided for update.");
        
        When(x => !string.IsNullOrEmpty(x.Name), () =>
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .MaximumLength(150)
                .MinimumLength(3);
        });
        
        When(x => x.BirthDate.HasValue, () =>
        {
            RuleFor(x => x.BirthDate)
                .LessThan(DateTime.Now)
                .GreaterThan(DateTime.MinValue)
                .WithMessage("BirthDate must be a valid date.");
        });
    }
}