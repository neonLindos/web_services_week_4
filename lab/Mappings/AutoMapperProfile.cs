using AutoMapper;
using ProductLab.DTO;
using ProductLab.Models;

namespace ProductLab.Mappings;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<Product, ProductDto>().ReverseMap();
    }
}
