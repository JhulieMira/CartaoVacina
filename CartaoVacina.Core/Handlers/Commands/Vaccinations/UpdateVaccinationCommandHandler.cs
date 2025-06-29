using AutoMapper;
using CartaoVacina.Contracts.Data;
using CartaoVacina.Contracts.DTOS.Vaccinations;
using CartaoVacina.Core.Exceptions;
using FluentValidation;
using MediatR;

namespace CartaoVacina.Core.Handlers.Commands.Vaccinations;

public class UpdateVaccinationCommand(int userId, int vaccinationId, UpdateVaccinationDTO payload) : IRequest<VaccinationDTO>
{
    public int UserId { get; } = userId;
    public int VaccinationId { get; } = vaccinationId;
    public UpdateVaccinationDTO Payload { get; } = payload;
}

public class UpdateVaccinationCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, IValidator<UpdateVaccinationDTO> validator): IRequestHandler<UpdateVaccinationCommand, VaccinationDTO>
{
    public async Task<VaccinationDTO> Handle(UpdateVaccinationCommand request, CancellationToken cancellationToken)
    {
        if (request.UserId == 0)
            throw new NotFoundException($"User with id {request.UserId} not found.");
        
        if (request.VaccinationId == 0)
            throw new NotFoundException($"Vaccination with id {request.VaccinationId} not found.");
        
        var validationResult = await validator.ValidateAsync(request.Payload, cancellationToken);
        
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);
        
        var user = await unitOfWork.Users.GetById(request.UserId, cancellationToken);
        
        if (user == null)
            throw new NotFoundException($"User with id {request.UserId} not found.");
        
        var vaccination = await unitOfWork.Vaccinations.GetById(request.VaccinationId, cancellationToken);
        
        if (vaccination == null)
            throw new NotFoundException($"Vaccine with id {request.VaccinationId} not found.");

        vaccination.VaccinationDate = request.Payload.VaccinationDate.Value;

        unitOfWork.Vaccinations.Update(vaccination);
        await unitOfWork.CommitAsync(cancellationToken);
        
        vaccination.Vaccine = await unitOfWork.Vaccines.GetById(vaccination.VaccineId, cancellationToken);
        return mapper.Map<VaccinationDTO>(vaccination);
    }
}