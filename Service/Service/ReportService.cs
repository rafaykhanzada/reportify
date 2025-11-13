using AutoMapper;
using Core.Constant;
using Core.Data.DTOs;
using Core.Data.Models;
using Core.Utils;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Service.IService;
using System.Linq.Expressions;
using System.Reflection.Metadata.Ecma335;
using UnitOfWork;

namespace Service.Service
{
    public class ReportService(IUnitOfWork unitOfWork, ILogger<ReportService> logger, IMapper mapper) : IReportService
    {
        public string UserId { get; set; }
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly ILogger<ReportService> _logger = logger;
        private readonly IMapper _mapper = mapper;
        private readonly ResultModel _resultModel = new();

        public async Task<ResultModel> CreateOrUpdate(ReportDto model)
        {
            try
            {
                var data = _mapper.Map<Report>(model);
                var exist = _unitOfWork.ReportRepository.Get(x => x.Id == model.Id).FirstOrDefault();

                if (data.Id > 0)
                {
                    if (exist == null)
                    {
                        _resultModel.Message = MessageString.NotFound;
                        return _resultModel;
                    }
                    data.UpdatedOn = DateTime.Now;
                    data.UpdatedBy = UserId;
                    data.Project = null;
                    data.CreatedBy = exist.CreatedBy;
                    data.CreatedOn = exist.CreatedOn;
                    _unitOfWork.ReportRepository.Update(data);
                    _resultModel.Message = MessageString.Updated;

                }
                else
                {
                    data.CreatedOn = DateTime.Now;
                    //data.UpdatedOn = DateTime.Now;
                    data.Project = null;
                    data.CreatedBy = UserId;
                    _unitOfWork.ReportRepository.Insert(data);
                    _resultModel.Message = MessageString.Created;

                }
                await _unitOfWork.CompleteAsync();
                _resultModel.Success = true;
                _resultModel.Data = _mapper.Map<ReportDto>(data);
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
                var data = _unitOfWork.ReportRepository.Get(x => x.Id == id && x.DeletedOn == null).FirstOrDefault();
                if (data != null)
                {
                    _unitOfWork.ReportRepository.SoftDelete(data, UserId);
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
                var data = _unitOfWork.ReportRepository.Get(x => x.DeletedOn == null);
                if (data.Any())
                {
                    var result = _mapper.Map<List<ReportDto>>(data);
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
                Expression<Func<Report, bool>> predicate = x => x.DeletedOn == null;
                if (!string.IsNullOrEmpty(model.SearchBy))
                    predicate = BaseUtil.AddSearchCriteria<Report>(model, predicate);
                #endregion
                var data = _unitOfWork.ReportRepository.Get(predicate, pageIndex, pageSize);
                _resultModel.Data = new ListModel<ReportDto>(_mapper.Map<IEnumerable<ReportDto>>(data), await _unitOfWork.ReportRepository.CountAsync(predicate));
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
                var data = _unitOfWork.ReportRepository.Get(x => x.DeletedOn == null && x.Id == id).FirstOrDefault();
                if (data != null)
                {
                    var result = _mapper.Map<ReportDto>(data);
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
        public ResultModel GetPreview(int id)
        {
            try
            {
                List<DynamicWidgetMeta> dbwidgets = new();
                List<DynamicWidgetMeta> meta = new();
                var data = _unitOfWork.ReportRepository.Get(x => x.DeletedOn == null && x.Id == id).FirstOrDefault();
                if (data.Widgets !=null)
                    meta = JsonConvert.DeserializeObject<List<DynamicWidgetMeta>>(data.Widgets)!;
                string connectionString = data.Connection! + ";TrustServerCertificate=True";
                SqlConnection cn = new SqlConnection(connectionString);

                // (Optional) Open the connection
                cn.Open();

                // Use it in your code
                Console.WriteLine("Connection is open!");

               
                if (meta.Count > 0 )
                {

                    foreach (var item in meta)
                    {
                        if (item.dbMeta == null || !item.dbMeta.Any()) continue;
                        var prodecure = item.dbMeta!.FirstOrDefault();
                        string query = "";
                        if (item.type == "textbox")
                        {
                            if (prodecure.tabletype == "table")
                            {
                                query = $"select Top 1 {prodecure.column} from {prodecure.table}";
                                item.text =  DbUtil.RawSqlQuery<string>(query, x => x[0].ToString(), cn).FirstOrDefault();

                            }
                        }
                        else if (item.type == "table")
                        {
                            if (prodecure.tabletype == "table")
                            {
                                query = $"select top 10 {prodecure.column} from {prodecure.table}";
                                var data2= DbUtil.GetAllDBRows(query, cn);
                                string table = "";
                                foreach (var row in data2.Rows)
                                    table += ((System.Data.DataRow)(row)).ItemArray[0];
                                //item.tableData.rows.Append();
                            }
                        }
                    }
                    data.Widgets = JsonConvert.SerializeObject(meta);
                }
                if (data != null)
                {
                    var result = _mapper.Map<ReportDto>(data);
                    _resultModel.Success = true;
                    _resultModel.Data = result;
                }
                else
                {
                    _logger.LogInformation(MessageString.NotFound);
                    _resultModel.Success = true;
                    _resultModel.Message = MessageString.NotFound;
                }
                // Always close or dispose when done
                cn.Close();
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
                return await _unitOfWork.ReportRepository.GetCountAsync();

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
                Expression<Func<Report, bool>> predicate = x => x.DeletedOn == null;
                if (model != null && !string.IsNullOrEmpty(model.SearchBy))
                    predicate = BaseUtil.AddSearchCriteria<Report>(model, predicate);
                var data = _unitOfWork.ReportRepository.Get(predicate);

                if (!data.Any())
                {
                    _logger.LogInformation("No Record Found!");
                    _resultModel.Data = new ListModel<string>(list: new List<string>(), count: 0);
                    _resultModel.Success = true;
                    _resultModel.Message = "No Record Found!";
                    return _resultModel;
                }
                var EmailVm = _mapper.Map<List<ReportDto>>(data);
                //await PopulateDealerNames(EmailVm);
                _resultModel.Success = true;
                _resultModel.Data = new ListModel<ReportDto>(EmailVm, data.Count());
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
