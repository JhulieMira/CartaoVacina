using AutoMapper;
using CartaoVacina.Contracts.Data;
using CartaoVacina.Contracts.DTOS.Vaccines;
using CartaoVacina.Core.Exceptions;
using MediatR;

namespace CartaoVacina.Core.Handlers.Queries;

public class GetVaccineByIdQuery(int vaccineId) : IRequest<VaccineDTO>
{
    public int VaccineId { get; } = vaccineId;
}

public class GetVaccineByIdHandler(IUnitOfWork unitOfWork, IMapper mapper): IRequestHandler<GetVaccineByIdQuery, VaccineDTO>
{
    public async Task<VaccineDTO> Handle(GetVaccineByIdQuery request, CancellationToken cancellationToken)
    {
        if (request.VaccineId == 0)
            throw new NotFoundException($"Vaccine with id {request.VaccineId} not found.");
        
        var vaccine = await unitOfWork.Vaccines.GetById(request.VaccineId, cancellationToken);
        
        if (vaccine is null)
            throw new NotFoundException($"Vaccine with id {request.VaccineId} not found.");
        
        return mapper.Map<VaccineDTO>(vaccine);
    }
}