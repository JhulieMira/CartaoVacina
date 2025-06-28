using AutoMapper;
using CartaoVacina.Contracts.Data;
using CartaoVacina.Contracts.DTOS.Vaccines;
using CartaoVacina.Core.Exceptions;
using FluentValidation;
using MediatR;

namespace CartaoVacina.Core.Handlers.Commands;

public class UpdateVaccineCommand(int vaccineId, UpdateVaccineDTO payload) : IRequest<VaccineDTO>
{
    public int VaccineId { get; } = vaccineId;
    public UpdateVaccineDTO Payload { get; } = payload;
}

public class UpdateVaccineCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, IValidator<UpdateVaccineDTO> validator): IRequestHandler<UpdateVaccineCommand, VaccineDTO>
{
    public async Task<VaccineDTO> Handle(UpdateVaccineCommand request, CancellationToken cancellationToken)
    {
        if (request.VaccineId == 0)
            throw new NotFoundException($"Vaccine with id {request.VaccineId} not found.");
        
        var validationResult = await validator.ValidateAsync(request.Payload, cancellationToken);
        
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);
        
        var vaccine = await unitOfWork.Vaccines.GetById(request.VaccineId, cancellationToken);
        
        if (vaccine is null)
            throw new NotFoundException($"Vaccine with id {request.VaccineId} not found.");

        if (!string.IsNullOrEmpty(request.Payload.Code) && vaccine.Code != request.Payload.Code)
        {
            var existingVaccine = await Task.FromResult(unitOfWork.Vaccines.Get(x => x.Code == request.Payload.Code));
        
            if (existingVaccine.FirstOrDefault() is not null)
                throw new ValidationException($"A vaccine with code {request.Payload.Code} already exists.");
        }
        
        vaccine.Name = string.IsNullOrEmpty(request.Payload.Name) ? vaccine.Name : request.Payload.Name;
        vaccine.Code = string.IsNullOrEmpty(request.Payload.Code) ? vaccine.Code : request.Payload.Code;
        vaccine.Doses = request.Payload.Doses ?? vaccine.Doses;
        vaccine.MinimumAge = request.Payload.MinimumAge ?? vaccine.MinimumAge;
        vaccine.MaximumAge = request.Payload.MaximumAge ?? vaccine.MaximumAge;
        
        unitOfWork.Vaccines.Update(vaccine);
        await unitOfWork.CommitAsync(cancellationToken);
        
        return mapper.Map<VaccineDTO>(vaccine);
    }
}