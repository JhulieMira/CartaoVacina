using AutoMapper;
using CartaoVacina.Contracts.Data;
using CartaoVacina.Contracts.Data.Entities;
using CartaoVacina.Contracts.DTOS.Users;
using FluentValidation;
using MediatR;

namespace CartaoVacina.Core.Handlers.Commands.Users;


public class CreateUserCommand(CreateUserDTO payload) : IRequest<UserDTO>
{
    public CreateUserDTO Payload { get; } = payload;
}

public class CreateUserCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, IValidator<CreateUserDTO> validator) : IRequestHandler<CreateUserCommand, UserDTO>
{
    public async Task<UserDTO> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request.Payload, cancellationToken);
        
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);
        
        var user = mapper.Map<User>(request.Payload);
        
        await unitOfWork.Users.Add(user, cancellationToken);
        await unitOfWork.CommitAsync(cancellationToken);
        
        return mapper.Map<UserDTO>(user);
    }
}