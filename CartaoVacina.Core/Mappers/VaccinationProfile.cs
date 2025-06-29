using AutoMapper;
using CartaoVacina.Contracts.Data.DTOS.Vaccinations;
using CartaoVacina.Contracts.Data.Entities;

namespace CartaoVacina.Core.Mappers;

public class VaccinationProfile: Profile
{
    public VaccinationProfile()
    {
        CreateMap<Vaccination, VaccinationDTO>()
            .ForMember(dest => dest.Vaccine, opt => opt.MapFrom(src => src.Vaccine.Name));
    }
}