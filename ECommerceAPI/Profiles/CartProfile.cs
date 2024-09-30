using AutoMapper;
using ECommerceAPI.Data.DTOs;
using ECommerceAPI.Models;
namespace ECommerceAPI.Profiles;

public class CartProfile : Profile
{
    public CartProfile()
    {
        CreateMap<CartItemDtoCreate, CartItem>().ReverseMap();
        CreateMap<CartItemDto, CartItem>().ReverseMap();
        CreateMap<CartItemDtoUpdate, CartItem>().ReverseMap();
        CreateMap<CartItemDtoRead, CartItem>().ReverseMap();
    }
}