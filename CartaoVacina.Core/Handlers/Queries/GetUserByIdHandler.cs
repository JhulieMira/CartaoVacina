using AutoMapper;
using CartaoVacina.Contracts.Data;
using CartaoVacina.Contracts.DTOS.Users;
using CartaoVacina.Core.Exceptions;
using MediatR;

namespace CartaoVacina.Core.Handlers.Queries;

public class GetUserByIdQuery(int userId) : IRequest<UserDTO>
{
    public int UserId { get; } = userId;
}

public class GetUserByIdHandler(IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<GetUserByIdQuery, UserDTO>
{
    public async Task<UserDTO> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var user = await unitOfWork.Users.GetById(request.UserId);

        if (user is null)
            throw new NotFoundException($"User with id {request.UserId} not found.");
        
        return mapper.Map<UserDTO>(user);
    }
}