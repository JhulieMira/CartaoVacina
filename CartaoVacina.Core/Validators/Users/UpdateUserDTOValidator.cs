using CartaoVacina.Contracts.Data.DTOS.Users;
using FluentValidation;

namespace CartaoVacina.Core.Validators.Users;

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
                .NotEmpty().WithMessage("Name field cannot be empty.")
                .MaximumLength(150).WithMessage("Name field must have a maximum of 150 characters.")
                .MinimumLength(3).WithMessage("Name field must have a minimum of 3 characters.");
        });
        
        When(x => x.BirthDate.HasValue, () =>
        {
            RuleFor(x => x.BirthDate)
                .LessThan(DateTime.Now).WithMessage("BirthDate field cannot be a future date.")
                .GreaterThan(DateTime.MinValue).WithMessage("BirthDate field must be a valid date.");
        });
    }
}