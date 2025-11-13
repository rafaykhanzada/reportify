using AutoMapper;
using Core.Constant;
using Core.Data.DTOs;
using Core.Data.Models;
using Core.Utils;
using Microsoft.Extensions.Logging;
using Service.IService;
using System.Linq.Expressions;
using UnitOfWork;

namespace Service.Service
{
    public class ProjectService(IUnitOfWork unitOfWork, ILogger<ProjectService> logger, IMapper mapper) : IProjectService
    {
        public string UserId { get; set; }
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly ILogger<ProjectService> _logger = logger;
        private readonly IMapper _mapper = mapper;
        private readonly ResultModel _resultModel = new();

        public async Task<ResultModel> CreateOrUpdate(ProjectDto model)
        {
            try
            {
                var data = _mapper.Map<Project>(model);
                var exist = _unitOfWork.ProjectRepository.Get(x => x.Id == model.Id).FirstOrDefault();

                if (data.Id > 0)
                {
                    if (exist == null)
                    {
                        _resultModel.Message = MessageString.NotFound;
                        return _resultModel;
                    }
                    data.UpdatedOn = DateTime.Now;
                    data.UpdatedBy = UserId;
                    data.CreatedBy = exist.CreatedBy;
                    data.CreatedOn = exist.CreatedOn;
                    _unitOfWork.ProjectRepository.Update(data);
                    _resultModel.Message = MessageString.Updated;

                }
                else
                {
                    data.CreatedOn = DateTime.Now;
                    //data.UpdatedOn = DateTime.Now;
                    data.CreatedBy = UserId;
                    _unitOfWork.ProjectRepository.Insert(data);
                    _resultModel.Message = MessageString.Created;

                }
                await _unitOfWork.CompleteAsync();
                _resultModel.Success = true;
                _resultModel.Data = _mapper.Map<ProjectDto>(data);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error:", ex);
                _resultModel.Success = false;
                _resultModel.Message = MessageString.ServerError;
            }
            return _resultModel;
        }

        public async Task<ResultModel> Delete(int id)
        {
            try
            {
                var data = _unitOfWork.ProjectRepository.Get(x => x.Id == id && x.DeletedOn == null).FirstOrDefault();
                if (data != null)
                {
                    _unitOfWork.ProjectRepository.SoftDelete(data, UserId);
                    await _unitOfWork.CompleteAsync();
                    _resultModel.Message = MessageString.Deleted;
                    _resultModel.Success = true;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Error:", ex);
                _resultModel.Success = false;
                _resultModel.Message = MessageString.ServerError;
            }
            return _resultModel;
        }

        public ResultModel Get()
        {
            try
            {
                var data = _unitOfWork.ProjectRepository.Get(x => x.DeletedOn == null);
                if (data.Any())
                {
                    var result = _mapper.Map<List<ProjectDto>>(data);
                    _resultModel.Success = true;
                    _resultModel.Data = result;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Error:", ex);
                _resultModel.Success = false;
                _resultModel.Message = MessageString.ServerError;
            }
            return _resultModel;
        }

        public async Task<ResultModel> Get(int pageIndex, int pageSize, FilterDto? model)
        {
            try
            {
                #region Filter
                // Set Predicate
                Expression<Func<Project, bool>> predicate = x => x.DeletedOn == null;
                if (!string.IsNullOrEmpty(model.SearchBy))
                    predicate = BaseUtil.AddSearchCriteria<Project>(model, predicate);
                #endregion
                var data = _unitOfWork.ProjectRepository.Get(predicate, pageIndex, pageSize);
                _resultModel.Data = new ListModel<ProjectDto>(_mapper.Map<IEnumerable<ProjectDto>>(data), await _unitOfWork.ProjectRepository.CountAsync(predicate));
                _resultModel.Success = true;
                _resultModel.Message = MessageString.Success;
                _logger.LogInformation(_resultModel.Message);

            }
            catch (Exception ex)
            {
                _logger.LogError("Error:", ex);
                _resultModel.Success = false;
                _resultModel.Message = MessageString.ServerError;
            }
            return _resultModel;
        }

        public ResultModel Get(int id)
        {
            try
            {
                var data = _unitOfWork.ProjectRepository.Get(x => x.DeletedOn == null && x.Id == id).FirstOrDefault();
                if (data != null)
                {
                    var result = _mapper.Map<ProjectDto>(data);
                    var reports = _unitOfWork.ReportRepository.Get(x => x.ProjectId == result.Id && x.DeletedOn == null).ToList();
                    if (reports != null && reports.Count > 0)
                        result.Reports = _mapper.Map<List<ReportDto>>(reports);
                    _resultModel.Success = true;
                    _resultModel.Data = result;
                }
                else
                {
                    _logger.LogInformation(MessageString.NotFound);
                    _resultModel.Success = true;
                    _resultModel.Message = MessageString.NotFound;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Error:", ex);
                _resultModel.Success = false;
                _resultModel.Message = MessageString.ServerError;
            }
            return _resultModel;
        }
        public async Task<int> GetCount()
        {
            try
            {
                return await _unitOfWork.ProjectRepository.GetCountAsync();

            }
            catch (Exception ex)
            {
                _logger.LogError("Error:", ex);
            }
            return 0;
        }
        public async Task<ResultModel> Export(string? Type, FilterDto? model)
        {
            try
            {
                Expression<Func<Project, bool>> predicate = x => x.DeletedOn == null;
                if (model != null && !string.IsNullOrEmpty(model.SearchBy))
                    predicate = BaseUtil.AddSearchCriteria<Project>(model, predicate);
                var data = _unitOfWork.ProjectRepository.Get(predicate);

                if (!data.Any())
                {
                    _logger.LogInformation("No Record Found!");
                    _resultModel.Data = new ListModel<string>(list: new List<string>(), count: 0);
                    _resultModel.Success = true;
                    _resultModel.Message = "No Record Found!";
                    return _resultModel;
                }
                var EmailVm = _mapper.Map<List<ProjectDto>>(data);
                //await PopulateDealerNames(EmailVm);
                _resultModel.Success = true;
                _resultModel.Data = new ListModel<ProjectDto>(EmailVm, data.Count());
                byte[] content = ExportUtility.ExportToExcel(EmailVm);
                _resultModel.Success = true;
                _resultModel.Data = content;
                _resultModel.Message = $"Total Items Exported {EmailVm.Count}";
                return _resultModel;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error:", ex);
                _resultModel.Success = false;
                _resultModel.Message = "Error While Get Record";
            }
            return _resultModel;
        }
    }
}
