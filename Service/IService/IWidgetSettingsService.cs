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
    public interface IWidgetSettingsService
    {
        Task<IEnumerable<WidgetSettings>> GetAllAsync();
        Task<ResultModel> GetByIdAsync(int id);
        Task<ResultModel> CreateAsync(WidgetSettingsCreateDto widgetSettingsDto, int widgetId);
        Task<bool> UpdateAsync(int id, WidgetSettingsDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
