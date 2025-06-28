using AutoMapper;
using CartaoVacina.Contracts.Data;
using CartaoVacina.Contracts.Data.Entities;
using CartaoVacina.Contracts.DTOS.Vaccines;
using FluentValidation;
using MediatR;

namespace CartaoVacina.Core.Handlers.Commands;


public class CreateVaccineCommand(CreateVaccineDTO payload) : IRequest<VaccineDTO>
{
    public CreateVaccineDTO Payload { get; } = payload;
}

public class CreateVaccineCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, IValidator<CreateVaccineDTO> validator): IRequestHandler<CreateVaccineCommand, VaccineDTO>
{
    public async Task<VaccineDTO> Handle(CreateVaccineCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request.Payload, cancellationToken);
        
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);
        
        var existingVaccine = await Task.FromResult(unitOfWork.Vaccines.Get(x => x.Code == request.Payload.Code));
        
        if (existingVaccine.FirstOrDefault() is not null)
            throw new ValidationException($"A vaccine with code {request.Payload.Code} already exists.");
        
        var vaccine = mapper.Map<Vaccine>(request.Payload);
        
        await unitOfWork.Vaccines.Add(vaccine, cancellationToken);
        await unitOfWork.CommitAsync(cancellationToken);
        
        return mapper.Map<VaccineDTO>(vaccine);
    }
}