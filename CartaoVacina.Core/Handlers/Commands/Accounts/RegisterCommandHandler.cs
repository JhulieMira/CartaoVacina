using CartaoVacina.Contracts.Data;
using CartaoVacina.Contracts.Data.Entities;
using CartaoVacina.Contracts.DTOS.Accounts;
using CartaoVacina.Core.Services;
using FluentValidation;
using MediatR;

namespace CartaoVacina.Core.Handlers.Commands.Accounts;

public class RegisterCommand(RegisterDTO payload) : IRequest<AuthResponseDTO>
{
    public RegisterDTO Payload { get; } = payload;
}


public class RegisterCommandHandler(
    IUnitOfWork unitOfWork,
    IPasswordService passwordService,
    IJwtService jwtService,
    IValidator<RegisterDTO> validator)
    : IRequestHandler<RegisterCommand, AuthResponseDTO>
{
    public async Task<AuthResponseDTO> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        await ValidateRequest(request, cancellationToken);

        if (!EmailIsUnique(request.Payload.Email))
            throw new InvalidOperationException("Email already registered");
        
        var (passwordHash, salt) = passwordService.HashPassword(request.Payload.Password);
        
        var account = new Account
        {
            Email = request.Payload.Email,
            PasswordHash = passwordHash,
            Salt = salt,
            IsActive = true
        };

        await SaveChanges(account, cancellationToken);

        var token = jwtService.GenerateToken(account);
        var refreshToken = jwtService.GenerateRefreshToken();

        return new AuthResponseDTO
        {
            Token = token,
            RefreshToken = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddHours(24),
            Account = new AccountInfoDTO()
            {
                Id = account.Id,
                Email = account.Email
            }
        };
    }

    private async Task ValidateRequest(RegisterCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request.Payload, cancellationToken);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);
    }

    private async Task SaveChanges(Account account, CancellationToken cancellationToken)
    {
        await unitOfWork.Accounts.Add(account, cancellationToken);
        await unitOfWork.CommitAsync(cancellationToken);
    }

    private bool EmailIsUnique(string email)
    {
        return unitOfWork.Accounts.Get(x => x.Email == email).ToList().Count == 0;
    }
} 