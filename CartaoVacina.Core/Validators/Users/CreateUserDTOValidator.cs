using CartaoVacina.Contracts.Data.DTOS.Users;
using CartaoVacina.Contracts.Data.Entities;
using FluentValidation;

namespace CartaoVacina.Core.Validators.Users;

public class CreateUserDTOValidator : AbstractValidator<CreateUserDTO>
{
    public CreateUserDTOValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name field is required.")
            .MaximumLength(150).WithMessage("Name field must have a maximum of 150 characters.")
            .MinimumLength(3).WithMessage("Name field must have a minimum of 3 characters.");
        
        RuleFor(x => x.BirthDate)
            .GreaterThan(DateTime.MinValue).WithMessage("BirthDate field must be a valid date.")
            .LessThan(DateTime.Now).WithMessage("BirthDate field cannot be a future date.");
        
        RuleFor(x => x.Gender)
            .NotEmpty().WithMessage("Gender field is required.")
            .Must(gender => Enum.TryParse<Gender>(gender, true, out _))
            .WithMessage($"Gender field must be one of the following values: {string.Join(", ", Enum.GetNames<Gender>())}");

    }
}