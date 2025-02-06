using Core.Data.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.IService
{
    public interface IWidgetSaveDataService
    {
        Task<string> SavePropertyData(WidgetSaveDataDTO widgetSaveDataDTO);

        Task<string> GetPropertyDataAsJSON(int reportID);
    }
}
