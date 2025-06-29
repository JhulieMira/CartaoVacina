using AutoMapper;
using CartaoVacina.Contracts.Data.DTOS.Users;
using CartaoVacina.Contracts.Data.Entities;

namespace CartaoVacina.Core.Mappers;

public class UserProfile: Profile
{
    public UserProfile()
    {
        CreateMap<User, UserDTO>();
        CreateMap<CreateUserDTO, User>()
            .ForMember(
                dest => dest.Gender, 
                opt => opt.MapFrom(src => string.IsNullOrEmpty(src.Gender) ? Gender.Other : Enum.Parse<Gender>(src.Gender, true))
            );
    }
}