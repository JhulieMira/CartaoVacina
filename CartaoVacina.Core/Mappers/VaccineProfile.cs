using AutoMapper;
using CartaoVacina.Contracts.Data.Entities;
using CartaoVacina.Contracts.DTOS.Vaccines;

namespace CartaoVacina.Core.Mappers;

public class VaccineProfile: Profile
{
    public VaccineProfile()
    {
        CreateMap<Vaccine, VaccineDTO>();
        CreateMap<CreateVaccineDTO, Vaccine>();
    }
}