using Core.Data.DTOs;
using Core.Data.Models;
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
        Task<WidgetSettings> GetByIdAsync(int id);
        Task<WidgetSettingsCreateDto> CreateAsync(WidgetSettingsCreateDto widgetSettingsDto, int widgetId);
        Task<bool> UpdateAsync(int id, WidgetSettingsDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
