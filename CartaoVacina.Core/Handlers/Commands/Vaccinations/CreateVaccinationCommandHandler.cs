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

public class CreateVaccinationCommandHandler(
    IUnitOfWork unitOfWork, 
    IMapper mapper, 
    IValidator<CreateVaccinationDTO> validator
): IRequestHandler<CreateVaccinationCommand, VaccinationDTO>
{
    public async Task<VaccinationDTO> Handle(CreateVaccinationCommand request, CancellationToken cancellationToken)
    {
        await ValidateRequest(request, cancellationToken);
        
        var user = await GetUserOrThrow(request, cancellationToken);
        var vaccine = await GetVaccineOrThrow(request, cancellationToken);
        
        if (!UserCanTakeVaccine(user, vaccine))
            throw new ValidationException($"User is not eligible to take the vaccine {vaccine.Name}.");
        
        if (UserHasTakenVaccine(unitOfWork, request))
            throw new ValidationException($"User already has taken the dose {request.Payload.Dose} of the vaccine {vaccine.Name}.");

        if (DoseVacinationIsValid(vaccine, request))
            throw new ValidationException($"User has already taken all doses of the vaccine {vaccine.Name}. Total doses: {vaccine.Doses}");


        var vaccination = new Vaccination {
            UserId = request.UserId,
            VaccineId = request.Payload.VaccineId,
            VaccinationDate = request.Payload.VaccinationDate,
            Dose = request.Payload.Dose
        };
        
        await SaveChanges(unitOfWork, vaccination, cancellationToken);
        
        vaccination.Vaccine = vaccine;
        
        return mapper.Map<VaccinationDTO>(vaccination);
    }

    private async Task ValidateRequest(CreateVaccinationCommand request, CancellationToken cancellationToken)
    {
        if (request.UserId == 0)
            throw new NotFoundException($"User with id {request.UserId} not found.");
        
        if (request.Payload.VaccineId == 0)
            throw new NotFoundException($"Vaccine with id {request.Payload.VaccineId} not found.");
        
        var validationResult = await validator.ValidateAsync(request.Payload, cancellationToken);
        
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);
    }
    private async Task<User> GetUserOrThrow(CreateVaccinationCommand request, CancellationToken cancellationToken)
    {
        var user = await unitOfWork.Users.GetById(request.UserId, cancellationToken);
        
        if (user == null)
            throw new NotFoundException($"User with id {request.UserId} not found.");

        return user;
    }
    private async Task<Vaccine> GetVaccineOrThrow(CreateVaccinationCommand request, CancellationToken cancellationToken)
    {
        var vaccine = await unitOfWork.Vaccines.GetById(request.Payload.VaccineId, cancellationToken);
        
        if (vaccine == null)
            throw new NotFoundException($"Vaccine with id {request.Payload.VaccineId} not found.");
        
        return vaccine;
    }
    private static bool UserCanTakeVaccine(User user, Vaccine vaccine)
    {
        var userAge = Math.Floor((DateTime.UtcNow - user.BirthDate).TotalDays / 365.25);
        
        if (vaccine.MinimumAge.HasValue && userAge < vaccine.MinimumAge.Value)
            return false;
        
        if (vaccine.MaximumAge.HasValue && userAge > vaccine.MaximumAge.Value)
            return false;

        return true;
    }
    private static bool DoseVacinationIsValid(Vaccine vaccine, CreateVaccinationCommand request)
    {
         return request.Payload.Dose > vaccine.Doses;
    }
    private static bool UserHasTakenVaccine(IUnitOfWork unitOfWork, CreateVaccinationCommand request)
    {
        var existingVaccination = unitOfWork.Vaccinations
            .Get(x => x.UserId == request.UserId && x.VaccineId == request.Payload.VaccineId)
            .Where(x => x.Dose == request.Payload.Dose)
            .ToList();
        
        return existingVaccination.Count != 0;
    }
    private static async Task SaveChanges(IUnitOfWork unitOfWork, Vaccination vaccination, CancellationToken cancellationToken)
    {
        await unitOfWork.Vaccinations.Add(vaccination, cancellationToken);
        await unitOfWork.CommitAsync(cancellationToken);
    }
}