using Core.Data.DTOs;
using Core.Data.Models;
using Core.Utils;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Service.IService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using UnitOfWork;

namespace Service.Service
{
    public class WidgetSaveDataService:IWidgetSaveDataService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ResultModel _resultModel;
        public WidgetSaveDataService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ResultModel> SavePropertyData(WidgetSaveDataDTO widgetSaveDataDTO)
        {
            try
            {
                var property = _unitOfWork.WidgetPropertyRepository.Get(x => x.Id == widgetSaveDataDTO.pId).FirstOrDefault();
                var propertyWidget = _unitOfWork.WidgetRepository.Get(x => x.Id == property.WidgetId).Select(x=>x.Name).FirstOrDefault();

                //var propertyName = _unitOfWork.WidgetPropertyRepository.Get(x=>x.Id==widgetSaveDataDTO.pId).Select(x => x.pName).FirstOrDefault();
                //var propertyLabel = _unitOfWork.WidgetPropertyRepository.Get(x=>x.Id==widgetSaveDataDTO.pId).Select(x=>x.pLabel).FirstOrDefault();
                //var propertyType = _unitOfWork.WidgetPropertyRepository.Get(x=>x.Id==widgetSaveDataDTO.pId).Select(x=>x.pType).FirstOrDefault();
                //var widgetId = _unitOfWork.WidgetPropertyRepository.Get(x=>x.Id==widgetSaveDataDTO.pId).Select(x=>x.WidgetId).FirstOrDefault();
                //var widgetName = _unitOfWork.WidgetRepository.Get(x=>x.Id==widgetId).Select(x=>x.Name).FirstOrDefault();
                var saveData = new WidgetSaveData
                {
                    pName = property.pName,
                    pLabel = property.pLabel,
                    pType = property.pType,
                    pWId = property.WidgetId,
                    pValue = widgetSaveDataDTO.pValue,
                    pId = widgetSaveDataDTO.pId,
                    RId = widgetSaveDataDTO.RId,
                    pWidgetName = propertyWidget,
                };
                await _unitOfWork.WidgetSaveDataRepository.AddAsync(saveData);
                await _unitOfWork.CompleteAsync();
                _resultModel.Message = "Report Data Saved Successfully"; ;
                _resultModel.Success = true;
            }
            catch (Exception ex)
            {

                _resultModel.Message = "Error Inserting:" + ex.Message;
                _resultModel.Success = true;
            }
            return _resultModel;
        }


        public async Task<ResultModel> GetPropertyDataAsJSON(int reportID)
        {
            try
            {
                // Ensure you are calling Getq which returns IQueryable
                var reportData = await _unitOfWork.WidgetSaveDataRepository
                    .Getq(x => x.RId == reportID)  // Get IQueryable here
                    .Select(x => new
                    {
                        x.pId,
                        x.pValue,
                        x.pName,
                        x.pLabel,
                        x.pType,
                        x.pWidgetName
                    })
                    .ToListAsync();  // Execute the query asynchronously and convert it to List

                _resultModel.Data = JsonConvert.SerializeObject(reportData, Newtonsoft.Json.Formatting.Indented);
                _resultModel.Success = true;
            }
            catch (Exception ex)
            {
                _resultModel.Message = $"Error retrieving report data for Report ID {reportID}: {ex.Message}";

            }
            return _resultModel;

        }

    }
}
