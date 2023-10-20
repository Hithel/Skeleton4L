

using API.Dtos;
using AutoMapper;
using Domain.Entities;

namespace API.Profiles;

public class MappingProfiles : Profile
{
    public MappingProfiles()
    {

        CreateMap<User,UserDto>().ReverseMap();
        CreateMap<Rol,RolDto>().ReverseMap();
        

    }
}