using AutoMapper;
using CartaoVacina.Contracts.Data.Entities;
using CartaoVacina.Contracts.DTOS.Vaccinations;

namespace CartaoVacina.Core.Mappers;

public class VaccinationProfile: Profile
{
    public VaccinationProfile()
    {
        CreateMap<Vaccination, VaccinationDTO>()
            .ForMember(dest => dest.Vaccine, opt => opt.MapFrom(src => src.Vaccine.Name));
    }
}