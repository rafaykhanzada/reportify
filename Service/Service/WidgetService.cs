using Core.Data.DTOs;
using Core.Data.Models;
using Core.Utils;
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
    public class WidgetService : IWidgetService
    {
        private readonly IUnitOfWork _unitOfWork;
        public WidgetService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        //public async Task<Widgets> CreateAsync(WidgetDto widgetDto)
        //{
        //    try
        //    {
        //        // Map the WidgetDto to Widgets entity
        //        var widgetEntity = new Widgets
        //        {
        //            Name = widgetDto.Name,
        //            DefaultImage = widgetDto.DefaultImage,
        //            DragImage = widgetDto.DragImage,
        //            WidgetSettings = widgetDto.WidgetSettings.Select(wsDto => new WidgetSettings
        //            {
        //                Name = wsDto.Name,
        //                WidgetProperty = wsDto.WidgetProperty.Select(wpDto => new WidgetProperty
        //                {
        //                    pName = wpDto.pName,
        //                    pType = wpDto.pType,
        //                    pValue = wpDto.pValue,
        //                    pLabel = wpDto.pLabel,
        //                    Datasource = wpDto.Datasource,
        //                    WidgetPropertyData = wpDto.WidgetPropertyData.Select(wpdDto => new WidgetPropertyData
        //                    {
        //                        Name = wpdDto.Name,
        //                        DefaultValue = wpdDto.DefaultValue
        //                    }).ToList()
        //                }).ToList()
        //            }).ToList()
        //        };

        //        // Add the new widget entity to the database
        //        var createdWidget = await _unitOfWork.WidgetRepository.AddAsync(widgetEntity);

        //        // Optionally, save changes if the unit of work pattern does not include auto-commit
        //        await _unitOfWork.CompleteAsync();

        //        return createdWidget;
        //    }
        //    catch (Exception ex)
        //    {
        //        // Log or handle exception as needed
        //        throw new Exception($"Error creating widget: {ex.Message}", ex);
        //    }
        //}

        public async Task<WidgetCreateDto> CreateAsync(WidgetCreateDto widgetDto)
        {
            var widgetEntity = new Widgets
            {
                Name = widgetDto.Name,
                DefaultImage = widgetDto.DefaultImage,
                DragImage = widgetDto.DragImage,
                WidgetSettings = null
            };

            var createdWidget = await _unitOfWork.WidgetRepository.AddAsync(widgetEntity);
             await _unitOfWork.CompleteAsync();

            return new WidgetCreateDto
            {
                Id = createdWidget.Id,
                Name = widgetDto.Name,
                DefaultImage = widgetDto.DefaultImage,
                DragImage = widgetDto.DragImage,
            };
        }
        public Task<bool> DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        //public async Task<IEnumerable<WidgetDto>> GetAllAsync()
        //{
        //    try
        //    {
        //        var widgets = await _unitOfWork.WidgetRepository
        //            .GetAllQuery()
        //            .Include(w => w.WidgetSettings)
        //                .ThenInclude(ws => ws.WidgetProperty)
        //                    .ThenInclude(wp => wp.WidgetPropertyData)
        //            .AsSplitQuery() // Enables split queries to improve performance
        //            .ToListAsync();

        //        var result = widgets.Select(widget => new WidgetDto
        //        {
        //            Id = widget.Id,
        //            Name = widget.Name,
        //            DefaultImage = widget.DefaultImage,
        //            DragImage = widget.DragImage,
        //            WidgetSettings = widget.WidgetSettings.Select(ws => new WidgetSettingsDto
        //            {
        //                Id = ws.Id,
        //                Name = ws.Name,
        //                WidgetProperty = ws.WidgetProperty.Select(wp => new WidgetPropertyDto
        //                {
        //                    Id = wp.Id,
        //                    pName = wp.pName,
        //                    pType = wp.pType,
        //                    pValue = wp.pValue,
        //                    pLabel = wp.pLabel,
        //                    Datasource = wp.Datasource,

        //                    WidgetPropertyData = wp.WidgetPropertyData.Select(wpd => new WidgetPropertyDataDto
        //                    {
        //                        Id = wpd.Id,
        //                        Name = wpd.Name,
        //                        DefaultValue = wpd.DefaultValue
        //                    }).ToList()
        //                }).ToList()
        //            }).ToList()
        //        }).ToList();

        //        return result;
        //    }
        //    catch (Exception ex)
        //    {
        //        // Log the exception for troubleshooting purposes
        //        Console.WriteLine(ex);
        //        throw;
        //    }
        //}

        public async Task<ResultModel> GetAllAsync()
        {
            var widgets = await _unitOfWork.WidgetRepository
          .GetAllQuery()
          .Include(w => w.WidgetSettings)
              .ThenInclude(ws => ws.WidgetProperty)
                  .ThenInclude(wp => wp.WidgetPropertyData)
                  .AsSplitQuery()
          .ToListAsync();
            var result = widgets.Select(w => new WidgetDto
            {
                Id = w.Id,
                Name = w.Name,
                DefaultImage = w.DefaultImage,
                DragImage = w.DragImage,

                // Only include WidgetSettings associated with this widget
                WidgetSettings = w.WidgetSettings
        .Where(ws => ws.Widgets.Any(widget => widget.Id == w.Id)) // Check if WidgetSettings references this widget
        .Select(ws => new WidgetSettingsDto
        {
            Id = ws.Id,
            Name = ws.Name,
            WId = w.Id,

            // Only include WidgetProperties that reference both the current Widget and WidgetSettings
            WidgetProperty = ws.WidgetProperty
                .Where(wp => wp.WsId == ws.Id && wp.WidgetId == w.Id) // Filter by both WidgetSettings ID and Widget ID
                .Select(wp => new WidgetPropertyDto
                {
                    Id = wp.Id,
                    pName = wp.pName,
                    pType = wp.pType,
                    pValue = wp.pValue,
                    pLabel = wp.pLabel,
                    Datasource = wp.Datasource,
                    WsId = wp.WsId,
                    WidgetId = wp.WidgetId, // Include both foreign keys for reference

                    // Only include WidgetPropertyData if it's linked to this WidgetProperty
                    WidgetPropertyData = wp.WidgetPropertyData
                        .Where(wpd => wpd.propId == wp.Id) // Filter by propId to match WidgetProperty
                        .Select(wpd => new WidgetPropertyDataDto
                        {
                            Id = wpd.Id,
                            Name = wpd.Name,
                            DefaultValue = wpd.DefaultValue,
                            propId = wpd.propId
                        }).ToList()
                }).ToList()
        }).ToList()
            }).ToList();


            //var result = widgets.Select(

            //    w => new WidgetDto
            //    {
            //        Id = w.Id,
            //        Name = w.Name,
            //        DefaultImage = w.DefaultImage,
            //        DragImage = w.DragImage,
            //        WidgetSettings = w.WidgetSettings.Select(ws => new WidgetSettingsDto
            //        {
            //            Id = ws.Id,
            //            Name = ws.Name,
            //            WId = w.Id,
            //            WidgetProperty = ws.WidgetProperty.Select(wp => new WidgetPropertyDto
            //            {
            //                Id = wp.Id,
            //                pName = wp.pName,
            //                pType = wp.pType,
            //                pValue = wp.pValue,
            //                pLabel = wp.pLabel,
            //                Datasource = wp.Datasource,
            //                WsId = wp.WsId,
            //                WidgetPropertyData = wp.WidgetPropertyData.Select(wpd => new WidgetPropertyDataDto
            //                {
            //                    Id = wpd.Id,
            //                    Name = wpd.Name,
            //                    DefaultValue = wpd.DefaultValue,
            //                    propId = wpd.propId
            //                }).ToList()
            //            }).ToList()
            //        }).ToList()
            //    });

            return new ResultModel
            {
                Success = true,
                Data = result
            };
        }

        //public async Task<IEnumerable<WidgetDto>> GetAllAsync()
        //{
        //    //var Widgets = _unitOfWork.WidgetRepository.GetAll();
        //    //var WidgetSettings = _unitOfWork.WidgetSettingsRepository.GetAll();
        //    //var WidgetProperty = _unitOfWork.WidgetPropertyRepository.GetAll();
        //    //var WidgetPropertyData = _unitOfWork.WidgetPropertyDataRepository.GetAll();
        //    var widgets = await _unitOfWork.WidgetRepository
        //            .GetAllQuery()
        //            .Include(w => w.WidgetSettings)
        //                .ThenInclude(ws => ws.WidgetProperty)
        //                    .ThenInclude(wp => wp.WidgetPropertyData)
        //            .AsSplitQuery() // Enables split queries to improve performance
        //            .ToListAsync();
        //    var result = widgets.Select(widget => new WidgetDto
        //    {
        //        Id = widget.Id,
        //        Name = widget.Name,
        //        DefaultImage = widget.DefaultImage,
        //        DragImage = widget.DragImage,
        //        WidgetSettings = widget.WidgetSettings.Select(ws => new WidgetSettingsDto
        //        {
        //            Id = ws.Id,
        //            Name = ws.Name,
        //            WidgetProperty = ws.WidgetProperty.Select(wp => new WidgetPropertyDto
        //            {
        //                Id = wp.Id,
        //                pName = wp.pName,
        //                pType = wp.pType,
        //                pValue = wp.pValue,
        //                pLabel = wp.pLabel,
        //                Datasource = wp.Datasource,
        //                WidgetPropertyData = wp.WidgetPropertyData.Select(wpd => new WidgetPropertyDataDto
        //                {
        //                    Id = wpd.Id,
        //                    Name = wpd.Name,
        //                    DefaultValue = wpd.DefaultValue
        //                }).ToList()
        //            }).ToList()
        //        }).ToList()
        //    }).ToList();

        //    return result;
        //}

        public async Task<Widgets> GetByIdAsync(int id)
        {
            var widget = await _unitOfWork.WidgetRepository.GetSingleIncludeAsync(w=>w.Id==id);
            return widget;
        }

        public Task<bool> UpdateAsync(int id, WidgetDto dto)
        {
            throw new NotImplementedException();
        }
    }
}
