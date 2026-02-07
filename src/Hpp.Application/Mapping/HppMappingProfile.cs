using AutoMapper;
using Hpp.Application.DTOs;
using Hpp.Domain.Entities;

namespace Hpp.Application.Mapping;

/// <summary>
/// AutoMapper profile.
/// </summary>
public sealed class HppMappingProfile : Profile
{
    public HppMappingProfile()
    {
        CreateMap<Item, ItemDto>()
            .ForMember(dest => dest.StandardCost, opt => opt.MapFrom(src => src.StandardCost.Amount))
            .ForMember(dest => dest.Currency, opt => opt.MapFrom(src => src.StandardCost.Currency));

        CreateMap<Purchase, PurchaseDto>()
            .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity.Value))
            .ForMember(dest => dest.UnitCost, opt => opt.MapFrom(src => src.UnitCost.Amount))
            .ForMember(dest => dest.Currency, opt => opt.MapFrom(src => src.UnitCost.Currency));
    }
}
