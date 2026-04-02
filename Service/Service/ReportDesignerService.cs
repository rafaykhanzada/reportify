using AutoMapper;
using Core.Constant;
using Core.Data.DTOs.ReportDesigner;
using Core.Data.Models;
using Core.Utils;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Service.IService;
using UnitOfWork;

namespace Service.Service
{
    public class ReportDesignerService : IReportDesignerService
    {
        public string UserId { get; set; }
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ReportDesignerService> _logger;
        private readonly IMapper _mapper;

        public ReportDesignerService(
            IUnitOfWork unitOfWork,
            ILogger<ReportDesignerService> logger,
            IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<ResultModel> CreateOrUpdate(ReportDesignDto model)
        {
            var result = new ResultModel();
            
            try
            {
                var report = _mapper.Map<Report>(model);

                if (model.Id.HasValue && model.Id > 0)
                {
                    var existing = _unitOfWork.ReportRepository.Get(x => x.Id == model.Id).FirstOrDefault();
                    if (existing == null)
                    {
                        result.Message = MessageString.NotFound;
                        return result;
                    }

                    report.UpdatedOn = DateTime.Now;
                    report.UpdatedBy = UserId;
                    report.CreatedBy = existing.CreatedBy;
                    report.CreatedOn = existing.CreatedOn;
                    _unitOfWork.ReportRepository.Update(report);
                    result.Message = MessageString.Updated;
                }
                else
                {
                    report.CreatedOn = DateTime.Now;
                    report.CreatedBy = UserId;
                    _unitOfWork.ReportRepository.Insert(report);
                    result.Message = MessageString.Created;
                }

                await _unitOfWork.CompleteAsync();
                result.Success = true;
                result.Data = _mapper.Map<ReportDesignDto>(report);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating/updating report design");
                result.Success = false;
                result.Message = MessageString.ServerError;
            }

            return result;
        }

        public async Task<ResultModel> Get(int id)
        {
            var result = new ResultModel();
            
            try
            {
                var report = _unitOfWork.ReportRepository
                    .Get(x => x.Id == id && x.DeletedOn == null)
                    .FirstOrDefault();

                if (report == null)
                {
                    result.Success = true;
                    result.Message = MessageString.NotFound;
                    return result;
                }

                // Deserialize the report design from JSON stored in database
                var design = new ReportDesignDto
                {
                    Id = report.Id,
                    Name = report.Name,
                    Description = report.Desc,
                    ProjectId = report.ProjectId,
                    CreatedOn = report.CreatedOn,
                    UpdatedOn = report.UpdatedOn
                };

                // Parse widgets (elements) if exists
                if (!string.IsNullOrEmpty(report.Widgets))
                {
                    try
                    {
                        design.Elements = JsonConvert.DeserializeObject<List<ReportElementDto>>(report.Widgets) 
                            ?? new List<ReportElementDto>();
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Error deserializing report elements");
                    }
                }

                // Parse context (additional settings) if exists
                if (!string.IsNullOrEmpty(report.Context))
                {
                    try
                    {
                        var context = JsonConvert.DeserializeObject<Dictionary<string, object>>(report.Context);
                        
                        if (context != null)
                        {
                            if (context.ContainsKey("PageSettings"))
                                design.PageSettings = JsonConvert.DeserializeObject<PageSettingsDto>(context["PageSettings"].ToString()!) 
                                    ?? new PageSettingsDto();
                            
                            if (context.ContainsKey("Parameters"))
                                design.Parameters = JsonConvert.DeserializeObject<List<ReportParameterDto>>(context["Parameters"].ToString()!) 
                                    ?? new List<ReportParameterDto>();
                            
                            if (context.ContainsKey("DataSources"))
                                design.DataSources = JsonConvert.DeserializeObject<List<ReportDataSourceDto>>(context["DataSources"].ToString()!) 
                                    ?? new List<ReportDataSourceDto>();
                            
                            if (context.ContainsKey("Groups"))
                                design.Groups = JsonConvert.DeserializeObject<List<GroupingDefinitionDto>>(context["Groups"].ToString()!) 
                                    ?? new List<GroupingDefinitionDto>();
                            
                            if (context.ContainsKey("Sorting"))
                                design.Sorting = JsonConvert.DeserializeObject<List<SortingDto>>(context["Sorting"].ToString()!) 
                                    ?? new List<SortingDto>();

                            if (context.ContainsKey("FormulaFields"))
                                design.FormulaFields = JsonConvert.DeserializeObject<List<FormulaFieldDto>>(context["FormulaFields"].ToString()!) 
                                    ?? new List<FormulaFieldDto>();

                            if (context.ContainsKey("RunningTotals"))
                                design.RunningTotals = JsonConvert.DeserializeObject<List<RunningTotalDto>>(context["RunningTotals"].ToString()!) 
                                    ?? new List<RunningTotalDto>();

                            if (context.ContainsKey("ConditionalFormats"))
                                design.ConditionalFormats = JsonConvert.DeserializeObject<List<ConditionalFormatDto>>(context["ConditionalFormats"].ToString()!) 
                                    ?? new List<ConditionalFormatDto>();
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Error deserializing report context");
                    }
                }

                result.Success = true;
                result.Data = design;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving report design {ReportId}", id);
                result.Success = false;
                result.Message = MessageString.ServerError;
            }

            return result;
        }

        public async Task<ResultModel> GetAll(int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var result = new ResultModel();
            
            try
            {
                var reports = _unitOfWork.ReportRepository
                    .Get(x => x.DeletedOn == null, pageIndex, pageSize);

                var designs = reports.Select(r => new ReportDesignDto
                {
                    Id = r.Id,
                    Name = r.Name,
                    Description = r.Desc,
                    ProjectId = r.ProjectId,
                    CreatedOn = r.CreatedOn,
                    UpdatedOn = r.UpdatedOn
                }).ToList();

                result.Success = true;
                result.Data = designs;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving report designs");
                result.Success = false;
                result.Message = MessageString.ServerError;
            }

            return result;
        }

        public async Task<ResultModel> Delete(int id)
        {
            var result = new ResultModel();
            
            try
            {
                var report = _unitOfWork.ReportRepository
                    .Get(x => x.Id == id && x.DeletedOn == null)
                    .FirstOrDefault();

                if (report != null)
                {
                    _unitOfWork.ReportRepository.SoftDelete(report, UserId);
                    await _unitOfWork.CompleteAsync();
                    result.Success = true;
                    result.Message = MessageString.Deleted;
                }
                else
                {
                    result.Message = MessageString.NotFound;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting report design {ReportId}", id);
                result.Success = false;
                result.Message = MessageString.ServerError;
            }

            return result;
        }

        public async Task<ResultModel> Duplicate(int id, string newName)
        {
            var result = new ResultModel();
            
            try
            {
                var existingResult = await Get(id);
                if (!existingResult.Success || existingResult.Data == null)
                {
                    result.Message = MessageString.NotFound;
                    return result;
                }

                var design = (ReportDesignDto)existingResult.Data;
                design.Id = null;
                design.Name = newName;
                design.CreatedOn = null;
                design.UpdatedOn = null;

                return await CreateOrUpdate(design);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error duplicating report design {ReportId}", id);
                result.Success = false;
                result.Message = MessageString.ServerError;
            }

            return result;
        }

        public async Task<ResultModel> GetElements(int reportId)
        {
            var result = new ResultModel();
            
            try
            {
                var reportResult = await Get(reportId);
                if (!reportResult.Success || reportResult.Data == null)
                {
                    result.Message = MessageString.NotFound;
                    return result;
                }

                var design = (ReportDesignDto)reportResult.Data;
                result.Success = true;
                result.Data = design.Elements;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving elements for report {ReportId}", reportId);
                result.Success = false;
                result.Message = MessageString.ServerError;
            }

            return result;
        }

        public async Task<ResultModel> SaveElement(int reportId, ReportElementDto element)
        {
            var result = new ResultModel();
            
            try
            {
                var reportResult = await Get(reportId);
                if (!reportResult.Success || reportResult.Data == null)
                {
                    result.Message = MessageString.NotFound;
                    return result;
                }

                var design = (ReportDesignDto)reportResult.Data;
                
                // Update or add element
                if (element.Id.HasValue && element.Id > 0)
                {
                    var existingElement = design.Elements.FirstOrDefault(e => e.Id == element.Id);
                    if (existingElement != null)
                    {
                        design.Elements.Remove(existingElement);
                    }
                }
                else
                {
                    element.Id = design.Elements.Any() 
                        ? design.Elements.Max(e => e.Id ?? 0) + 1 
                        : 1;
                }

                design.Elements.Add(element);

                // Save back to database
                return await CreateOrUpdate(design);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving element for report {ReportId}", reportId);
                result.Success = false;
                result.Message = MessageString.ServerError;
            }

            return result;
        }

        public async Task<ResultModel> DeleteElement(int elementId)
        {
            var result = new ResultModel();
            
            try
            {
                // This would need to be enhanced based on how you want to track elements
                // For now, this is a placeholder
                result.Success = true;
                result.Message = MessageString.Deleted;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting element {ElementId}", elementId);
                result.Success = false;
                result.Message = MessageString.ServerError;
            }

            return result;
        }
    }
}
