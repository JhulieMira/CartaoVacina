using CartaoVacina.Contracts.Data;
using CartaoVacina.Core.Exceptions;
using MediatR;

namespace CartaoVacina.Core.Handlers.Commands.Users;

public class DeleteUserCommand(int userId) : IRequest
{
    public int UserId { get; } = userId;
}

public class DeleteUserCommandHandler(IUnitOfWork unitOfWork): IRequestHandler<DeleteUserCommand>
{
    public async Task Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        if (request.UserId == 0)
            throw new NotFoundException($"User with id {request.UserId} not found.");
        
        await unitOfWork.Users.Delete(request.UserId, cancellationToken);
        await unitOfWork.CommitAsync(cancellationToken);
    }
}