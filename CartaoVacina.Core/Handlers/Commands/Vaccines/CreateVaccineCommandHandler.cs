using AutoMapper;
using CartaoVacina.Contracts.Data;
using CartaoVacina.Contracts.Data.Entities;
using CartaoVacina.Contracts.DTOS.Vaccines;
using FluentValidation;
using MediatR;

namespace CartaoVacina.Core.Handlers.Commands.Vaccines;


public class CreateVaccineCommand(CreateVaccineDTO payload) : IRequest<VaccineDTO>
{
    public CreateVaccineDTO Payload { get; } = payload;
}

public class CreateVaccineCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, IValidator<CreateVaccineDTO> validator): IRequestHandler<CreateVaccineCommand, VaccineDTO>
{
    public async Task<VaccineDTO> Handle(CreateVaccineCommand request, CancellationToken cancellationToken)
    {
        await ValidateRequest(request, cancellationToken);

        if (!VaccineCodeIsUnique(request.Payload.Code))
            throw new ValidationException($"A vaccine with code {request.Payload.Code} already exists.");
        
        var vaccine = mapper.Map<Vaccine>(request.Payload);
        
        await SaveChanges(vaccine, cancellationToken);
        
        return mapper.Map<VaccineDTO>(vaccine);
    }

    private async Task ValidateRequest(CreateVaccineCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request.Payload, cancellationToken);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);
    }

    private bool VaccineCodeIsUnique(string code)
    {
        return unitOfWork.Vaccines.Get(x => x.Code == code).ToList().Count == 0;
    }

    private async Task SaveChanges(Vaccine vaccine, CancellationToken cancellationToken)
    {
        await unitOfWork.Vaccines.Add(vaccine, cancellationToken);
        await unitOfWork.CommitAsync(cancellationToken);
    }
}