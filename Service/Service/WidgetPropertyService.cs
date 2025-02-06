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
    public class WidgetPropertyService : IWidgetPropertyService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWidgetSettingsService widgetSettingsService;
        private readonly IWidgetService widgetService;
        public WidgetPropertyService(IUnitOfWork unitOfWork, IWidgetSettingsService widgetSettingsService, IWidgetService widgetService)
        {
            _unitOfWork = unitOfWork;
            this.widgetSettingsService = widgetSettingsService;
            this.widgetService = widgetService;
        }

        public async Task<WidgetPropertyCreateDto> CreateAsync(WidgetPropertyCreateDto widgetPropertyDto)
        {
            // Fetch the WidgetSettings entity by ID
            var widgetSettingsEntity = await widgetSettingsService.GetByIdAsync(widgetPropertyDto.WsId);

            _unitOfWork.WidgetSettingsRepository.Attach(widgetSettingsEntity);
            // Fetch the Widget entity by ID
            var widgetEntity = await widgetService.GetByIdAsync(widgetPropertyDto.WidgetId);
            _unitOfWork.WidgetRepository.Attach(widgetEntity);
            if (widgetEntity == null)
            {
                throw new Exception($"Widget with ID {widgetPropertyDto.WidgetId} not found.");
            }

            if (widgetSettingsEntity == null)
            {
                throw new Exception($"WidgetSettings with ID {widgetPropertyDto.WsId} not found.");
            }

            // Create a new WidgetProperty entity and associate it with the Widget and WidgetSettings
            var widgetPropertyEntity = new WidgetProperty
            {
                pName = widgetPropertyDto.pName,
                pType = widgetPropertyDto.pType,
                pValue = widgetPropertyDto.pValue,
                pLabel = widgetPropertyDto.pLabel,
                Datasource = widgetPropertyDto.Datasource,
                // Associate with the parent settings
                WidgetSettings = widgetSettingsEntity,
                // Associate with the parent widget
                Widget = widgetEntity // Set the widget entity
            };

            // Add the new WidgetProperty entity to the repository
            var createdProperty = await _unitOfWork.WidgetPropertyRepository.AddAsync(widgetPropertyEntity);
            await _unitOfWork.CompleteAsync();

            // Return a DTO containing the created property information
            return new WidgetPropertyCreateDto
            {
                Id = createdProperty.Id,
                pName = createdProperty.pName,
                pType = createdProperty.pType,
                pValue = createdProperty.pValue,
                pLabel = createdProperty.pLabel,
                Datasource = createdProperty.Datasource,
                WsId = createdProperty.WsId,
                WidgetId = createdProperty.WidgetId // Return the Widget ID as well
            };
        }

        public Task<bool> DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<WidgetProperty>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<WidgetProperty> GetByIdAsync(int id)
        {
            var widgetProperty = await _unitOfWork.WidgetPropertyRepository.FindFirstAsync(wp=>wp.Id== id);
            if(widgetProperty == null)
            {
                return null;
            }
            return widgetProperty;
        }

        public Task<bool> UpdateAsync(int id, WidgetPropertyDto dto)
        {
            throw new NotImplementedException();
        }
    }
}
