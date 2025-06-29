using CartaoVacina.Contracts.Data.DTOS.Users;
using CartaoVacina.Contracts.Data.Entities;
using FluentValidation;

namespace CartaoVacina.Core.Validators.Users;

public class CreateUserDTOValidator : AbstractValidator<CreateUserDTO>
{
    public CreateUserDTOValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(150)
            .MinimumLength(3);
        
        RuleFor(x => x.BirthDate)
            .GreaterThan(DateTime.MinValue)
            .LessThan(DateTime.Now)
            .WithMessage("BirthDate must be a valid date");
        
        RuleFor(x => x.Gender)
            .NotEmpty()
            .Must(gender => Enum.TryParse<Gender>(gender, true, out _))
            .WithMessage($"Gender must be one of: {string.Join(", ", Enum.GetNames<Gender>())}");

    }
}