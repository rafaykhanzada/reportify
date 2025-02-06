using AutoMapper;
using Core.Data.DTOs;
using Core.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        }
    }
}
