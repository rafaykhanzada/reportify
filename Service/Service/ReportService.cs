using AutoMapper;
using Core.Constant;
using Core.Data.DTOs;
using Core.Data.Models;
using Core.Utils;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Service.IService;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection.Metadata.Ecma335;
using System.Text.Json;
using UnitOfWork;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Service.Service
{
    public class ReportService(IUnitOfWork unitOfWork, ILogger<ReportService> logger, IMapper mapper, IDynamicDbContextService dynamicDbContextService) : IReportService
    {
        public string UserId { get; set; }
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly ILogger<ReportService> _logger = logger;
        private readonly IMapper _mapper = mapper;
        private readonly ResultModel _resultModel = new();
        private readonly IDynamicDbContextService _dynamicDbContextService = dynamicDbContextService;

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

        //public async Task<ResultModel> Get(int id)
        //{
        //    try
        //    {
        //        List<DynamicWidgetMeta> dbwidgets = new();
        //        List<DynamicWidgetMeta> meta = new();
        //        List<StoredProcedureParameterDto> sqlParameters = [];
        //        var data = _unitOfWork.ReportRepository.Get(x => x.DeletedOn == null && x.Id == id).FirstOrDefault();
        //        if (data.Widgets != null)
        //            meta = JsonConvert.DeserializeObject<List<DynamicWidgetMeta>>(data.Widgets)!;
        //        if (data != null)
        //        {
        //            var result = _mapper.Map<ReportDto>(data);
        //            if (result.Connection != null)
        //            {
        //                var dbMeta = meta.FirstOrDefault().dbMeta.FirstOrDefault();
        //                if (dbMeta!=null)
        //                {
        //                    var parameters = dbMeta.procedureParameters;
        //                    foreach (var parameter in parameters)
        //                    {
        //                        sqlParameters.Add(new StoredProcedureParameterDto
        //                        {
        //                            Name = parameter.name,
        //                            Value = parameter.value,
        //                            DataType = parameter.dataType
        //                        });
        //                    }
        //                    string connectionString = result.Connection! + ";TrustServerCertificate=True";
        //                    var dataset = await _dynamicDbContextService.ExecuteStoredProcedureSchemaAsync(connectionString, result.Table, sqlParameters);
        //                    if (dataset.Success == true && dataset.Data != null)
        //                    {
        //                        result.Object = new DynamicDbObject
        //                        {
        //                            ObjectName = result.Table,
        //                            Columns = (List<DynamicDbSchema>)dataset.Data
        //                        };
        //                    }
        //                }
        //            }
        //            _resultModel.Success = true;
        //            _resultModel.Data = result;
        //        }
        //        else
        //        {
        //            _logger.LogInformation(MessageString.NotFound);
        //            _resultModel.Success = true;
        //            _resultModel.Message = MessageString.NotFound;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError("Error:", ex);
        //        _resultModel.Success = false;
        //        _resultModel.Message = MessageString.ServerError;
        //    }
        //    return _resultModel;
        //}
        public async Task<ResultModel> Get(int id)
        {
            try
            {
                var data = _unitOfWork.ReportRepository
                    .Get(x => x.DeletedOn == null && x.Id == id)
                    .FirstOrDefault();

                if (data == null)
                {
                    _logger.LogInformation(MessageString.NotFound);
                    return new ResultModel
                    {
                        Success = true,
                        Message = MessageString.NotFound
                    };
                }

                var result = _mapper.Map<ReportDto>(data);

                if (string.IsNullOrWhiteSpace(result.Connection))
                {
                    return new ResultModel { Success = true, Data = result };
                }

                // Parse and validate widgets metadata
                var dbMeta = GetDbMetaFromWidgets(data.Widgets);
                if (dbMeta?.procedureParameters == null || dbMeta.procedureParameters.Count() == 0)
                {
                    return new ResultModel { Success = true, Data = result };
                }

                // Build SQL parameters
                var sqlParameters = dbMeta.procedureParameters
                    .Select(p => new StoredProcedureParameterDto
                    {
                        Name = p.name,
                        Value = p.value,
                        DataType = p.dataType
                    })
                    .ToList();

                // Execute stored procedure
                string connectionString = $"{result.Connection};TrustServerCertificate=True";
                var dataset = await _dynamicDbContextService
                    .ExecuteStoredProcedureSchemaAsync(connectionString, dbMeta.table, sqlParameters);

                if (dataset.Success && dataset.Data != null)
                {
                    result.Object = new DynamicDbObject
                    {
                        ObjectName = result.Table,
                        Parameter = dbMeta.procedureParameters.ToList(),
                        Columns = (List<DynamicDbSchema>)dataset.Data
                    };
                }

                return new ResultModel { Success = true, Data = result };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving report {ReportId}", id);
                return new ResultModel { Success = false, Message = MessageString.ServerError };
            }
        }

        private Dbmeta? GetDbMetaFromWidgets(string? widgetsJson)
        {
            if (string.IsNullOrWhiteSpace(widgetsJson))
                return null;

            try
            {
                var meta = JsonConvert.DeserializeObject<List<DynamicWidgetMeta>>(widgetsJson);
                return meta.Where(x => x.dbMeta != null).FirstOrDefault().dbMeta.FirstOrDefault();
            }
            catch
            {
                return null;
            }
        }
        public ResultModel GetPreview(int id)
        {
            try
            {
                List<DynamicWidgetMeta> dbwidgets = new();
                List<DynamicWidgetMeta> meta = new();
                var data = _unitOfWork.ReportRepository.Get(x => x.DeletedOn == null && x.Id == id).FirstOrDefault();
                if (data.Widgets != null)
                    meta = JsonConvert.DeserializeObject<List<DynamicWidgetMeta>>(data.Widgets)!;
                string connectionString = data.Connection! + ";TrustServerCertificate=True";
                SqlConnection cn = new SqlConnection(connectionString);

                // (Optional) Open the connection
                cn.Open();

                // Use it in your code
                Console.WriteLine("Connection is open!");


                if (meta.Count > 0)
                {

                    foreach (var item in meta.Where(x=>x.dbMeta!=null))
                    {
                        var prodecure = item.dbMeta!.FirstOrDefault();
                        string query = "";
                        if (item.type == "textbox")
                        {
                            if (prodecure.tabletype == "table")
                            {
                                query = $"select Top 1 {prodecure.column} from {prodecure.table}";
                                item.text = DbUtil.RawSqlQuery<string>(query, x => x[0].ToString(), cn).FirstOrDefault();

                            }
                            if (prodecure.tabletype == "procedure")
                            {
                                item.text = DbUtil.RawSqlQuery<string>(prodecure.procedureName,prodecure.procedureParameters.ToList(), x => x[$"{item.text}"].ToString(),cn,CommandType.StoredProcedure).FirstOrDefault();

                            }
                        }
                        else if (item.type == "table")
                        {
                            if (prodecure.tabletype == "table")
                            {
                                query = $"select top 10 {prodecure.column} from {prodecure.table}";
                                var data2 = DbUtil.GetAllDBRows(query, cn);
                                string table = "";
                                foreach (var row in data2.Rows)
                                    table += ((System.Data.DataRow)(row)).ItemArray[0];
                                //item.tableData.rows.Append();
                            }
                            if (prodecure.tabletype == "procedure")
                            {
                                query = $"exec {prodecure.procedureName}";
                                var data2 = DbUtil.GetAllDBRows(prodecure.procedureName, cn,prodecure.procedureParameters.ToList());
                                string?[] firstRow = item.tableData.Rows[0].ToArray();
                                var selected = data2.DefaultView.ToTable(false, firstRow);
                                string table = "";
                                // Set headers (column names)
                                //item.tableData.Headers = selected.Columns
                                //                               .Cast<DataColumn>()
                                //                               .Select(c => new TabledataHeaders
                                //                               {
                                //                                   Column = c.ColumnName,
                                //                                   Width = ""
                                //                               })
                                //                               .ToList();   
                                item.tableData.Rows = new List<List<string?>>();
                                // Add rows
                                foreach (DataRow row in selected.Rows)
                                {
                                    var rowList = row.ItemArray
                                                     .Select(x => x?.ToString())
                                                     .ToList();

                                    item.tableData.Rows.Add(rowList);
                                }
                                //item.tableData.rows.Append();
                            }
                        }
                        else if (item.type == "procedures")
                        {
                            if (prodecure.tabletype == "table")
                            {
                                string param = "";
                                foreach (var para in prodecure.procedureParameters)
                                    param += $" @{para.name}={para.value}";
                                query = $"exec {prodecure.procedureName} ";
                                var data2 = DbUtil.GetAllDBRows(query, cn);
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
