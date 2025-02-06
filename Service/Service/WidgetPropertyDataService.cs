using Core.Data.DTOs;
using Core.Data.Models;
using Service.IService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnitOfWork;

namespace Service.Service
{
    public class WidgetPropertyDataService : IWidgetPropertyDataService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWidgetPropertyService _widgetPropertyService;
        public WidgetPropertyDataService(IUnitOfWork unitOfWork, IWidgetPropertyService widgetPropertyService)
        {
            _unitOfWork = unitOfWork;
            _widgetPropertyService = widgetPropertyService;
        }

        public async Task<WidgetPropertyDataDto> CreateAsync(WidgetPropertyDataDto widgetPropertyDataDto)
        {
            var widgetPropertyEntity = await _widgetPropertyService.GetByIdAsync(widgetPropertyDataDto.propId);
            var widgetPropertyDataEntity = new WidgetPropertyData
            {
                Name = widgetPropertyDataDto.Name,
                DefaultValue = widgetPropertyDataDto.DefaultValue,
                // Associate with the parent property
                WidgetProperty = widgetPropertyEntity
            };

            var createdPropertyData = await _unitOfWork.WidgetPropertyDataRepository.AddAsync(widgetPropertyDataEntity);
            await _unitOfWork.CompleteAsync();

            return new WidgetPropertyDataDto
            {
                Id = createdPropertyData.Id,
                Name = createdPropertyData.Name,
                DefaultValue = widgetPropertyDataDto.DefaultValue,
                propId = widgetPropertyDataDto.propId,
            };
        }

        public Task<bool> DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<WidgetPropertyData>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<WidgetPropertyData> GetByIdAsync(int id)
        {
            var widgetPropertyData = await _unitOfWork.WidgetPropertyDataRepository.FindFirstAsync(wpd=>wpd.Id==id);
            return widgetPropertyData;
        }

        public Task<bool> UpdateAsync(int id, WidgetPropertyDataDto dto)
        {
            throw new NotImplementedException();
        }
    }
}
