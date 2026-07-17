using AutoMapper;
using Vladify.Application.Commands.ModerationTasks.CreateTask;
using Vladify.Domain.Entities;

namespace Vladify.Application.MapperProfiles;

public class ModerationTaskProfile : Profile
{
    public ModerationTaskProfile()
    {
        CreateMap<CreateTaskCommand, ModerationTask>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.AssignedModeratorId, opt => opt.Ignore())
            .ForMember(dest => dest.Status, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());

        CreateMap<CreatedTaskResponse, ModerationTask>()
            .ReverseMap();
    }
}
