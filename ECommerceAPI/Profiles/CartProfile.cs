using AutoMapper;
using ECommerceAPI.Data.DTOs;
using ECommerceAPI.Models;
namespace ECommerceAPI.Profiles;

public class CartProfile : Profile
{
    public CartProfile()
    {
        CreateMap<CartDto, CartDto>().ReverseMap();
        CreateMap<CartHeaderDto, CartHeader>().ReverseMap();
        CreateMap<CartItemDto, CartItem>().ReverseMap();
    }
}