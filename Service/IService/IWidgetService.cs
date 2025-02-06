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
    public interface IWidgetService
    {
        Task<ResultModel> GetAllAsync();
        Task<Widgets> GetByIdAsync(int id);
        Task<WidgetCreateDto> CreateAsync(WidgetCreateDto widget);
        Task<bool> UpdateAsync(int id, WidgetDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
