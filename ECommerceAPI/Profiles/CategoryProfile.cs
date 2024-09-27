using AutoMapper;
using ECommerceAPI.Data.DTOs;
using ECommerceAPI.Models;
namespace ECommerceAPI.Profiles;

public class CategoryProfile : Profile
{
    public CategoryProfile()
    {
        CreateMap<CategoryDtoCreate, Category>();
        CreateMap<Category, CategoryDtoRead>();
        CreateMap<CategoryDtoRead, Category>();
        CreateMap<CategoryDtoRead, Category>();
        CreateMap<Category, CategoryDtoRead>();
        CreateMap<CategoryDtoUpdate, Category>();
        CreateMap<Category, CategoryDtoUpdate>();
    }
}