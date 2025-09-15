using Core.Data.DTOs;
using Core.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.IService
{
    public interface IWidgetSaveDataService
    {
        Task<ResultModel> SavePropertyData(WidgetSaveDataDTO widgetSaveDataDTO);

        Task<ResultModel> GetPropertyDataAsJSON(int reportID);
    }
}
