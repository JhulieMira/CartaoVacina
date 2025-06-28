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

public class UpdateUserCommandHandler(IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<UpdateUserCommand, UserDTO>
{
    public async Task<UserDTO> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        if (request.UserId == 0)
            throw new NotFoundException($"User with id {request.UserId} not found.");
        
        if (request.UpdateUserDTO == null || (string.IsNullOrEmpty(request.UpdateUserDTO.Name) && request.UpdateUserDTO.Name == null))
            throw new ValidationException("At least one field must be provided for update.");
        
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