using Core.Data.DTOs;
using Core.Data.Models;
using Core.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.IService
{
    public interface IWidgetPropertyDataService
    {
        Task<IEnumerable<WidgetPropertyData>> GetAllAsync();
        Task<ResultModel> GetByIdAsync(int id);
        Task<ResultModel> CreateAsync(WidgetPropertyDataDto widgetPropertyDataDto);
        Task<bool> UpdateAsync(int id, WidgetPropertyDataDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
