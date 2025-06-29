using CartaoVacina.Contracts.Data;
using CartaoVacina.Core.Exceptions;
using MediatR;

namespace CartaoVacina.Core.Handlers.Commands.Vaccinations;

public class DeleteVaccinationCommand(int userId, int vaccinationId): IRequest
{
    public int UserId { get; } = userId;
    public int VaccinationId { get; } = vaccinationId;
}

public class DeleteVaccinationCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<DeleteVaccinationCommand>
{
    public async Task Handle(DeleteVaccinationCommand request, CancellationToken cancellationToken)
    {
        if (request.UserId == 0)
            throw new NotFoundException($"User with id {request.UserId} not found.");
        
        if (request.VaccinationId == 0)
            throw new NotFoundException($"Vaccination with id {request.VaccinationId} not found.");
        
        var user = await unitOfWork.Users.GetById(request.UserId, cancellationToken);
        
        if (user == null)
            throw new NotFoundException($"User with id {request.UserId} not found.");
        
        await unitOfWork.Vaccinations.Delete(request.VaccinationId, cancellationToken);
        await unitOfWork.CommitAsync(cancellationToken);
    }
}