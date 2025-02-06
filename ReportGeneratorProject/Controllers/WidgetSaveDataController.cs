using Core.Data.DTOs;
using Core.Data.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.IService;
using Service.Service;
using UnitOfWork;

namespace ReportGeneratorProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WidgetSaveDataController : ControllerBase
    {
        private readonly IWidgetSaveDataService widgetSaveDataService;
        private readonly IUnitOfWork _unitOfWork;
        public WidgetSaveDataController(IWidgetSaveDataService widgetSaveDataService,IUnitOfWork unitOfWork)
        {
            this.widgetSaveDataService = widgetSaveDataService;
            _unitOfWork = unitOfWork;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromQuery] WidgetSaveDataDTO widgetSaveDataDTO)
        {
            try
            {
                var result = await widgetSaveDataService.SavePropertyData(widgetSaveDataDTO);
                return Ok(result);
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }


        [HttpPost("save")]
        public async Task<IActionResult> SaveReportData([FromBody] SaveReportDTO saveReportDTO)
        {
            try
            {
                // Loop through each property data and save
                var saveDataList = new List<WidgetSaveData>();
                foreach (var widgetSaveDataDTO in saveReportDTO.widgetSaveData)
                {
                    var property = _unitOfWork.WidgetPropertyRepository.Get(x => x.Id == widgetSaveDataDTO.pId).FirstOrDefault();
                    var propertyWidget = _unitOfWork.WidgetRepository.Get(x => x.Id == property.WidgetId).Select(x => x.Name).FirstOrDefault();

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

                    saveDataList.Add(saveData);
                }

                // Bulk save all properties for the report
                await _unitOfWork.WidgetSaveDataRepository.AddRangeAsync(saveDataList);
                await _unitOfWork.CompleteAsync();

                return Ok("Report Data Saved Successfully");
            }
            catch (Exception ex)
            {
                return BadRequest("Error saving report data: " + ex.Message);
            }
        }

        [HttpGet("DataAsJson/:recID")]
        public async Task<IActionResult> GetDataAsJSON(int recID)
        {
            try
            {
                if(recID == 0)
                {
                    return BadRequest("report ID cannot be null");
                }
                var result = await widgetSaveDataService.GetPropertyDataAsJSON(recID);
                return Ok(result);
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
