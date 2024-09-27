using AutoMapper;
using ECommerceAPI.Data.DTOs;
using ECommerceAPI.Models;
namespace ECommerceAPI.Profiles;

public class ProductProfile : Profile
{
    public ProductProfile()
    {
        CreateMap<Product, ProductDtoRead>();
        CreateMap<Product, ProductDtoCreate>();
        CreateMap<Product, ProductDtoUpdate>();
        CreateMap<ProductDtoUpdate, Product>();
        CreateMap<ProductDtoCreate, Product>().ForMember(dest => dest.CategoryId, opt => opt.MapFrom(src => src.CategoryId));
    }
}