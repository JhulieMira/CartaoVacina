using AutoMapper;
using CartaoVacina.Contracts.Data.DTOS.Vaccines;
using CartaoVacina.Contracts.Data.Entities;

namespace CartaoVacina.Core.Mappers;

public class VaccineProfile: Profile
{
    public VaccineProfile()
    {
        CreateMap<Vaccine, VaccineDTO>();
        CreateMap<CreateVaccineDTO, Vaccine>();
    }
}