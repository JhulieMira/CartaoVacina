using AutoMapper;
using CartaoVacina.Contracts.Data;
using CartaoVacina.Contracts.Data.Entities;
using CartaoVacina.Contracts.DTOS.Vaccinations;
using CartaoVacina.Core.Exceptions;
using FluentValidation;
using MediatR;

namespace CartaoVacina.Core.Handlers.Commands.Vaccinations;

public class CreateVaccinationCommand(int userId, CreateVaccinationDTO payload) : IRequest<VaccinationDTO>
{
    public int UserId { get; } = userId;
    public CreateVaccinationDTO Payload { get; } = payload;
}

public class CreateVaccinationCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, IValidator<CreateVaccinationDTO> validator): IRequestHandler<CreateVaccinationCommand, VaccinationDTO>
{
    public async Task<VaccinationDTO> Handle(CreateVaccinationCommand request, CancellationToken cancellationToken)
    {
        if (request.UserId == 0)
            throw new NotFoundException($"User with id {request.UserId} not found.");
        
        if (request.Payload.VaccineId == 0)
            throw new NotFoundException($"Vaccine with id {request.Payload.VaccineId} not found.");
        
        var validationResult = await validator.ValidateAsync(request.Payload, cancellationToken);
        
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);
        
        var user = await unitOfWork.Users.GetById(request.UserId, cancellationToken);
        
        if (user == null)
            throw new NotFoundException($"User with id {request.UserId} not found.");
        
        var vaccine = await unitOfWork.Vaccines.GetById(request.Payload.VaccineId, cancellationToken);
        
        if (vaccine == null)
            throw new NotFoundException($"Vaccine with id {request.Payload.VaccineId} not found.");
        
        var userAge = Math.Floor((DateTime.UtcNow - user.BirthDate).TotalDays / 365.25);

        if (vaccine.MinimumAge.HasValue && userAge < vaccine.MinimumAge.Value)
            throw new ValidationException($"User is too young for this vaccine. Minimum age is {vaccine.MinimumAge} years.");
        
        if (vaccine.MaximumAge.HasValue && userAge > vaccine.MaximumAge.Value)
            throw new ValidationException($"User is too old for this vaccine. Maximum age is {vaccine.MaximumAge} years.");
        
        var existingVaccination = await Task.FromResult(unitOfWork.Vaccinations
            .Get(x => x.UserId == request.UserId && x.VaccineId == request.Payload.VaccineId)
            .Where(x => x.Dose == request.Payload.Dose));
        
        if (existingVaccination.FirstOrDefault() is not null)
            throw new ValidationException($"User already has taken the dose {request.Payload.Dose} of the vaccine {vaccine.Name}.");
        
        var vaccination = new Vaccination {
            UserId = request.UserId,
            VaccineId = request.Payload.VaccineId,
            VaccinationDate = request.Payload.VaccinationDate,
            Dose = request.Payload.Dose
        };
        
        await unitOfWork.Vaccinations.Add(vaccination, cancellationToken);
        await unitOfWork.CommitAsync(cancellationToken);
        
        vaccination.Vaccine = vaccine;
        
        return mapper.Map<VaccinationDTO>(vaccination);
    }
}