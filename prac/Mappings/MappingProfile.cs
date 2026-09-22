using AutoMapper;
using TeamPractice.DTO;
using TeamPractice.Models;

namespace TeamPractice.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Team, TeamDto>().ReverseMap();
    }
}
