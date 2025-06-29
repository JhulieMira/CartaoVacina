using CartaoVacina.Contracts.Data;
using CartaoVacina.Core.Exceptions;
using MediatR;

namespace CartaoVacina.Core.Handlers.Commands.Vaccines;

public class DeleteVaccineCommand(int vaccineId): IRequest
{
    public int VaccineId { get; } = vaccineId;
}

public class DeleteVaccineCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<DeleteVaccineCommand>
{
    public async Task Handle(DeleteVaccineCommand request, CancellationToken cancellationToken)
    {
        if (request.VaccineId == 0)
            throw new NotFoundException($"Vaccine with id {request.VaccineId} not found.");
        
        await unitOfWork.Vaccines.Delete(request.VaccineId, cancellationToken);
        await unitOfWork.CommitAsync(cancellationToken);
    }
}