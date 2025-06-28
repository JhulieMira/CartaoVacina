using AutoMapper;
using CartaoVacina.Contracts.Data.Entities;
using CartaoVacina.Contracts.DTOS.Users;

namespace CartaoVacina.Core.Mappers;

public class UserProfile: Profile
{
    public UserProfile()
    {
        CreateMap<User, UserDTO>();
    }
}