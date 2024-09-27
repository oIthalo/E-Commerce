using AutoMapper;
using ECommerceAPI.Data.DTOs;
using ECommerceAPI.Models;
namespace ECommerceAPI.Profiles;

public class RoleProfile : Profile
{
    public RoleProfile()
    {
        CreateMap<Role, RoleDtoRead>();
        CreateMap<RoleDtoCreate, Role>();
        CreateMap<Role, RoleDtoUpdate>();
        CreateMap<RoleDtoUpdate, Role>();
    }
}