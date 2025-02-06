using Core.Data.DTOs;
using Core.Data.Models;
using Microsoft.EntityFrameworkCore;
using Service.IService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnitOfWork;

namespace Service.Service
{
    public class WidgetSettingsService : IWidgetSettingsService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWidgetService _widgetService;
        public WidgetSettingsService(IUnitOfWork unitOfWork, IWidgetService widgetService)
        {
            _unitOfWork = unitOfWork;
            _widgetService = widgetService;
        }

        public async Task<WidgetSettingsCreateDto> CreateAsync(WidgetSettingsCreateDto widgetSettingsDto, int widgetId)
        {
            //var widgetEntity = await _widgetService.GetByIdAsync(widgetId);
            //_unitOfWork.WidgetRepository.Attach(widgetEntity);

            //var widgetSettingsEntity = new WidgetSettings
            //{
            //    Name = widgetSettingsDto.Name,
            //    // Associate with the parent widget
            //    Widgets = new List<Widgets> { widgetEntity }
            //};

            //var createdSettings = await _unitOfWork.WidgetSettingsRepository.AddAsync(widgetSettingsEntity);
            //await _unitOfWork.CompleteAsync();

            //return new WidgetSettingsCreateDto
            //{
            //    Id = createdSettings.Id,
            //    Name = createdSettings.Name,
            //};
            // Step 1: Check if WidgetSettings with the same name already exists
            // Fetch the existing widget entity
            var widgetEntity = await _widgetService.GetByIdAsync(widgetId);

            // Attach the widget entity to the context to prevent EF from trying to insert it
            _unitOfWork.WidgetRepository.Attach(widgetEntity);
            var existingSettings = await _unitOfWork.WidgetSettingsRepository
                .FindAsync(ws => ws.Name == widgetSettingsDto.Name);

            WidgetSettings widgetSettingsEntity;
          
            if (existingSettings != null)
            {
                // Step 2: If it exists, reuse it
                widgetSettingsEntity = existingSettings;
            }
            else
            {
                // Step 3: If it doesn't exist, create a new WidgetSettings
               
                widgetSettingsEntity = new WidgetSettings
                {
                    Name = widgetSettingsDto.Name
                };

                // Add the new WidgetSettings to the repository
                await _unitOfWork.WidgetSettingsRepository.AddAsync(widgetSettingsEntity);
            }

            // Step 4: Get the existing widget entity
            if (widgetEntity == null)
            {
                throw new Exception($"Widget with ID {widgetId} not found.");
            }

            // Step 5: Associate the widget with the WidgetSettings
            // Check if the widget is already associated with the WidgetSettings
            if (!widgetSettingsEntity.Widgets.Contains(widgetEntity))
            {
                widgetSettingsEntity.Widgets.Add(widgetEntity);
            }

            // Step 6: Save changes to the context
            await _unitOfWork.CompleteAsync();

            // Return the created or existing WidgetSettings
            return new WidgetSettingsCreateDto
            {
                Id = widgetSettingsEntity.Id,
                Name = widgetSettingsEntity.Name,
            };
        }

        //public async Task<WidgetSettingsCreateDto> CreateAsync(WidgetSettingsCreateDto widgetSettingsDto, int widgetId)
        //{
        //    // Fetch the existing widget entity
        //    var widgetEntity = await _widgetService.GetByIdAsync(widgetId);

        //    // Attach the widget entity to the context to prevent EF from trying to insert it
        //    _unitOfWork.WidgetRepository.Attach(widgetEntity);

        //    // Check if the WidgetSettings already exists
        //    var existingWidgetSettings = await _unitOfWork.WidgetSettingsRepository
        //        .FindAsync(ws => ws.Name == widgetSettingsDto.Name);

        //    // Create or reuse WidgetSettings
        //    WidgetSettings widgetSettingsEntity;

        //    if (existingWidgetSettings != null)
        //    {
        //        // If it exists, use the existing WidgetSettings
        //        widgetSettingsEntity = existingWidgetSettings;
        //    }
        //    else
        //    {
        //        // If it does not exist, create a new WidgetSettings
        //        widgetSettingsEntity = new WidgetSettings
        //        {
        //            Name = widgetSettingsDto.Name
        //        };
        //    }

        //    // Associate the widget with the WidgetSettings if not already done
        //    if (!widgetSettingsEntity.Widgets.Any(w => w.Id == widgetId))
        //    {
        //        widgetSettingsEntity.Widgets.Add(widgetEntity);
        //    }

        //    // Add the WidgetSettings entity to the context
        //    await _unitOfWork.WidgetSettingsRepository.AddAsync(widgetSettingsEntity);
        //    await _unitOfWork.CompleteAsync();

        //    return new WidgetSettingsCreateDto
        //    {
        //        Id = widgetSettingsEntity.Id,
        //        Name = widgetSettingsEntity.Name,
        //    };
        //}
        public Task<bool> DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<WidgetSettings>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<WidgetSettings> GetByIdAsync(int id)
        {
            var widgetSetting = await _unitOfWork.WidgetSettingsRepository.FindFirstAsync(ws=>ws.Id==id);
            if(widgetSetting == null)
            {
                return null;
            }
            return widgetSetting;
        }

        public Task<bool> UpdateAsync(int id, WidgetSettingsDto dto)
        {
            throw new NotImplementedException();
        }
    }
}
