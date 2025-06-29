using AutoMapper;
using CartaoVacina.Contracts.Data.DTOS.Users;
using CartaoVacina.Contracts.Data.Interfaces;
using CartaoVacina.Core.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CartaoVacina.Core.Handlers.Queries.Users;

public class GetUserByIdQuery(int userId) : IRequest<UserDTO>
{
    public int UserId { get; } = userId;
}

public class GetUserByIdHandler(IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<GetUserByIdQuery, UserDTO>
{
    public async Task<UserDTO> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var user = await unitOfWork.Users.Get()
            .Include(x => x.Vaccinations)
            .ThenInclude(x => x.Vaccine)
            .FirstOrDefaultAsync(x => x.Id == request.UserId);

        if (user is null)
            throw new NotFoundException($"User with id {request.UserId} not found.");
        
        return mapper.Map<UserDTO>(user);
    }
}