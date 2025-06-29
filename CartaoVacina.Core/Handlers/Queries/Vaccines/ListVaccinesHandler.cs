using AutoMapper;
using CartaoVacina.Contracts.Data.DTOS.Vaccines;
using CartaoVacina.Contracts.Data.Interfaces;
using MediatR;

namespace CartaoVacina.Core.Handlers.Queries.Vaccines;

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