using AutoMapper;
using ECommerceAPI.Data.DTOs;
using ECommerceAPI.Models;
namespace ECommerceAPI.Profiles;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<User, UserDtoRegister>();
        CreateMap<UserDtoRegister, User>();
        CreateMap<UserDtoRead, User>();
        CreateMap<User, UserDtoRead>().ForMember(dest => dest.RoleName, opt => opt.MapFrom(src => src.Role.Name));;
        CreateMap<User, UserDtoLogin>();
    }
}