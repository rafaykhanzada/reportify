using AutoMapper;
using Core.Data.DTOs;
using Core.Data.Models;

namespace Core.Utils
{
    public class AutoMapperProfile:Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Widgets, WidgetDto>().ReverseMap();
            CreateMap<WidgetProperty, WidgetPropertyDto>().ReverseMap();
            CreateMap<WidgetPropertyData, WidgetPropertyDataDto>().ReverseMap();
            CreateMap<WidgetSettings, WidgetSettingsDto>().ReverseMap();
            CreateMap<Project, ProjectDto>().ReverseMap();
            CreateMap<Report, ReportDto>().ReverseMap();
        }
    }
}
