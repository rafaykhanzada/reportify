using Core.Data.DTOs;
using Core.Data.Models;
using Core.Utils;
using Service.IService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnitOfWork;

namespace Service.Service
{
    public class WidgetReportService: IWidgetReportService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly ResultModel _resultModel = new();
        public WidgetReportService(IUnitOfWork _unitOfWork)
        {
            unitOfWork = _unitOfWork;

        }
        //Generate new report
        public async Task<ResultModel> CreateReport(string RName)
        {
            try
            {
                int lastCount = unitOfWork.WidgetReportRepository
                    .GetAll()
                    .OrderByDescending(x => x.Id)
                    .Select(x => x.Count)
                    .FirstOrDefault();

                var model = new WidgetReportData
                {
                    ReportName = RName,
                    Count = lastCount + 1,
                };

                await unitOfWork.WidgetReportRepository.AddAsync(model);
                await unitOfWork.CompleteAsync();
                _resultModel.Message = $"Successfully Generated Report, Report_ID: {model.Id}";
            _resultModel.Success = true ;
                return _resultModel;
            }
            catch (Exception ex)
            {
                _resultModel.Message = $"There was some error, Error Message: {ex.Message}";
                _resultModel.Success = true;
                return _resultModel;
            }
        }
    }
}
