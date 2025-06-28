using AutoMapper;
using CartaoVacina.Contracts.Data;
using CartaoVacina.Contracts.DTOS.Vaccines;
using MediatR;

namespace CartaoVacina.Core.Handlers.Queries;

public class ListVaccinesQuery : IRequest<List<VaccineDTO>>
{
    // TODO: Add parameters for pagination, filtering, etc.
}

public class ListVaccinesHandler(IUnitOfWork unitOfWork, IMapper mapper): IRequestHandler<ListVaccinesQuery, List<VaccineDTO>>
{
    public async Task<List<VaccineDTO>> Handle(ListVaccinesQuery request, CancellationToken cancellationToken)
    {
        var entities = await Task.FromResult(unitOfWork.Vaccines.Get());
        
        return mapper.Map<List<VaccineDTO>>(entities);
    }
}