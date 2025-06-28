using AutoMapper;
using CartaoVacina.Contracts.Data;
using CartaoVacina.Contracts.DTOS.Users;
using MediatR;

namespace CartaoVacina.Core.Handlers.Queries;


public class ListUsersQuery() : IRequest<List<UserDTO>>
{
    // TODO: Add parameters for pagination, filtering, etc.
}

public class ListUsersHandler(IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<ListUsersQuery, List<UserDTO>>
{
    public async Task<List<UserDTO>> Handle(ListUsersQuery request, CancellationToken cancellationToken)
    {
        var entities = await Task.FromResult(unitOfWork.Users.Get());
        
        return mapper.Map<List<UserDTO>>(entities);
    }
}