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
    public interface IWidgetPropertyService
    {
        Task<IEnumerable<WidgetProperty>> GetAllAsync();
        Task<ResultModel> GetByIdAsync(int id);
        Task<ResultModel> CreateAsync(WidgetPropertyCreateDto widgetPropertyDto);
        Task<bool> UpdateAsync(int id, WidgetPropertyDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
