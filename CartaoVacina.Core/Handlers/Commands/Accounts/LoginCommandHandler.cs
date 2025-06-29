using CartaoVacina.Contracts.Data.DTOS.Accounts;
using CartaoVacina.Contracts.Data.Entities;
using CartaoVacina.Contracts.Data.Interfaces;
using CartaoVacina.Core.Exceptions;
using CartaoVacina.Core.Services;
using FluentValidation;
using MediatR;

namespace CartaoVacina.Core.Handlers.Commands.Accounts;

public class LoginCommand(LoginDTO payload) : IRequest<AuthResponseDTO>
{
    public LoginDTO Payload { get; } = payload;
}

public class LoginCommandHandler(
    IUnitOfWork unitOfWork,
    IPasswordService passwordService,
    IJwtService jwtService,
    IValidator<LoginDTO> validator)
    : IRequestHandler<LoginCommand, AuthResponseDTO>
{
    public async Task<AuthResponseDTO> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        await ValidateRequest(request, cancellationToken);
        
        var account = GetActiveAccountOrThrow(request);

        if (!passwordService.VerifyPassword(request.Payload.Password, account.PasswordHash, account.Salt))
            throw new InvalidOperationException("Invalid email or password");
        
        await UpdateLastLogin(account, cancellationToken);

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
    
    private async Task ValidateRequest(LoginCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request.Payload, cancellationToken);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);
    }

    private async Task UpdateLastLogin(Account account, CancellationToken cancellationToken)
    {
        account.LastLogin = DateTime.UtcNow;
        
        unitOfWork.Accounts.Update(account);
        await unitOfWork.CommitAsync(cancellationToken);
    }

    private Account GetActiveAccountOrThrow(LoginCommand request)
    {
        var account = unitOfWork.Accounts.Get(x => x.Email == request.Payload.Email).FirstOrDefault();
        
        if (account == null)
            throw new NotFoundException("Invalid email or password");

        if (!account.IsActive)
            throw new InvalidOperationException("Account is deactivated");
        
        return account;
    }
} 