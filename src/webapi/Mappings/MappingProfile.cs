using AutoMapper;

namespace webapi.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        //Configure the Mappings
        CreateMap<Shared.Entities.MongoDB.PlannedScheduleEntity, Shared.Models.PlannedScheduleObject>();
        CreateMap<Shared.Models.PlannedScheduleObject, Shared.Entities.MongoDB.PlannedScheduleEntity>();
    }
}