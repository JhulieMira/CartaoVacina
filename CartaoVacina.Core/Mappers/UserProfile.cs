using AutoMapper;
using CartaoVacina.Contracts.Data.Entities;
using CartaoVacina.Contracts.DTOS.Users;

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