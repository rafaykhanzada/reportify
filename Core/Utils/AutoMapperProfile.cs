using AutoMapper;
using Core.Data.DTOs;
using Core.Data.DTOs.ReportDesigner;
using Core.Data.Models;
using Newtonsoft.Json;

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

            // Report Designer mappings
            CreateMap<Report, ReportDesignDto>()
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Desc))
                .ForMember(dest => dest.Elements, opt => opt.MapFrom(src => 
                    string.IsNullOrEmpty(src.Widgets) 
                        ? new List<ReportElementDto>() 
                        : JsonConvert.DeserializeObject<List<ReportElementDto>>(src.Widgets)))
                .ForMember(dest => dest.PageSettings, opt => opt.Ignore())
                .ForMember(dest => dest.Parameters, opt => opt.Ignore())
                .ForMember(dest => dest.DataSources, opt => opt.Ignore())
                .ForMember(dest => dest.Groups, opt => opt.Ignore())
                .ForMember(dest => dest.Sorting, opt => opt.Ignore())
                .ForMember(dest => dest.FormulaFields, opt => opt.Ignore())
                .ForMember(dest => dest.RunningTotals, opt => opt.Ignore())
                .ForMember(dest => dest.ConditionalFormats, opt => opt.Ignore());

            CreateMap<ReportDesignDto, Report>()
                .ForMember(dest => dest.Desc, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.Widgets, opt => opt.MapFrom(src => 
                    src.Elements != null && src.Elements.Any() 
                        ? JsonConvert.SerializeObject(src.Elements) 
                        : null))
                .ForMember(dest => dest.Context, opt => opt.MapFrom(src => 
                    JsonConvert.SerializeObject(new {
                        PageSettings = src.PageSettings,
                        Parameters = src.Parameters,
                        DataSources = src.DataSources,
                        Groups = src.Groups,
                        Sorting = src.Sorting,
                        FormulaFields = src.FormulaFields,
                        RunningTotals = src.RunningTotals,
                        ConditionalFormats = src.ConditionalFormats
                    })))
                .ForMember(dest => dest.Project, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedBy, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedOn, opt => opt.Ignore());

            CreateMap<ReportElement, ReportElementDto>().ReverseMap();
            CreateMap<ReportParameter, ReportParameterDto>().ReverseMap();
            CreateMap<ReportDataSource, ReportDataSourceDto>().ReverseMap();
        }
    }
}
