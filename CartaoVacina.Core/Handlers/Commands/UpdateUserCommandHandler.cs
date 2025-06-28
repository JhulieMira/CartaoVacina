using AutoMapper;
using CartaoVacina.Contracts.Data;
using CartaoVacina.Contracts.DTOS.Users;
using CartaoVacina.Core.Exceptions;
using FluentValidation;
using MediatR;

namespace CartaoVacina.Core.Handlers.Commands;

public class UpdateUserCommand(int userId, UpdateUserDTO updateUserDTO): IRequest<UserDTO>
{
    public int UserId { get; } = userId;
    public UpdateUserDTO UpdateUserDTO { get; } = updateUserDTO;
}

public class UpdateUserCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, IValidator<UpdateUserDTO> validator) : IRequestHandler<UpdateUserCommand, UserDTO>
{
    public async Task<UserDTO> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        if (request.UserId == 0)
            throw new NotFoundException($"User with id {request.UserId} not found.");
        
        var validationResult = await validator.ValidateAsync(request.UpdateUserDTO, cancellationToken);
        
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);
        
        var user = await unitOfWork.Users.GetById(request.UserId, cancellationToken);
        
        if (user is null)
            throw new NotFoundException($"User with id {request.UserId} not found.");
        
        user.Name = string.IsNullOrEmpty(request.UpdateUserDTO.Name) ? user.Name : request.UpdateUserDTO.Name;
        user.BirthDate = request.UpdateUserDTO.BirthDate ?? user.BirthDate;
        
        unitOfWork.Users.Update(user);
        await unitOfWork.CommitAsync(cancellationToken);
        
        return mapper.Map<UserDTO>(user);
    }
}