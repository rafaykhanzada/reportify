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
using UnitOfWork;

namespace Service.Service
{
    public class ReportService(IUnitOfWork unitOfWork, ILogger<ReportService> logger, IMapper mapper, IDynamicDbContextService dynamicDbContextService, IFormulaEvaluationService formulaEvaluationService) : IReportService
    {
        private const string WidgetTypeTextbox = "textbox";
        private const string WidgetTypeTable = "table";
        private const string WidgetTypeProcedures = "procedures";
        private const string WidgetTypeFormula = "formula";
        private const string TableTypeTable = "table";
        private const string TableTypeProcedure = "procedure";
        private const string TableTypeFormula = "formula";
        private const string TrustServerCertificateSuffix = ";TrustServerCertificate=True";

        public string UserId { get; set; }
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly ILogger<ReportService> _logger = logger;
        private readonly IMapper _mapper = mapper;
        private readonly ResultModel _resultModel = new();
        private readonly IDynamicDbContextService _dynamicDbContextService = dynamicDbContextService;
        private readonly IFormulaEvaluationService _formulaEvaluationService = formulaEvaluationService;

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
                        ObjectName = dbMeta.table,
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
                var report = _unitOfWork.ReportRepository
                    .Get(x => x.DeletedOn == null && x.Id == id)
                    .FirstOrDefault();

                if (report == null)
                {
                    _logger.LogInformation(MessageString.NotFound);
                    return new ResultModel { Success = true, Message = MessageString.NotFound };
                }

                var meta = ParseWidgetMeta(report.Widgets);
                if (meta is { Count: > 0 } && !string.IsNullOrWhiteSpace(report.Connection))
                {
                    var connectionString = report.Connection.TrimEnd(';') + TrustServerCertificateSuffix;
                    using var connection = new SqlConnection(connectionString);
                    connection.Open();

                    foreach (var widget in meta.Where(w => w.dbMeta != null && w.dbMeta.Length > 0))
                    {
                        var dbMeta = widget.dbMeta![0];
                        try
                        {
                            PopulateWidgetPreview(widget, dbMeta, connection);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogWarning(ex, "Failed to populate widget {WidgetId} for report {ReportId}", widget.id, id);
                        }
                    }

                    report.Widgets = JsonConvert.SerializeObject(meta);
                }

                var result = _mapper.Map<ReportDto>(report);
                return new ResultModel { Success = true, Data = result };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating preview for report {ReportId}", id);
                return new ResultModel { Success = false, Message = MessageString.ServerError };
            }
        }

        private static List<DynamicWidgetMeta> ParseWidgetMeta(string? widgetsJson)
        {
            if (string.IsNullOrWhiteSpace(widgetsJson))
                return new List<DynamicWidgetMeta>();
            try
            {
                return JsonConvert.DeserializeObject<List<DynamicWidgetMeta>>(widgetsJson) ?? new List<DynamicWidgetMeta>();
            }
            catch
            {
                return new List<DynamicWidgetMeta>();
            }
        }

        private void PopulateWidgetPreview(DynamicWidgetMeta widget, Dbmeta dbMeta, SqlConnection connection)
        {
            if (dbMeta.tabletype == TableTypeFormula)
            {
                PopulateFormulaWidget(widget, dbMeta, connection);
                return;
            }

            switch (widget.type)
            {
                case WidgetTypeTextbox:
                    PopulateTextboxWidget(widget, dbMeta, connection);
                    break;
                case WidgetTypeTable:
                    PopulateTableWidget(widget, dbMeta, connection);
                    break;
                case WidgetTypeProcedures:
                    PopulateProceduresWidget(widget, dbMeta, connection);
                    break;
                case WidgetTypeFormula:
                    PopulateFormulaWidget(widget, dbMeta, connection);
                    break;
            }
        }

        private void PopulateTextboxWidget(DynamicWidgetMeta widget, Dbmeta dbMeta, SqlConnection connection)
        {
            if (dbMeta.tabletype == TableTypeTable && !string.IsNullOrWhiteSpace(dbMeta.column) && !string.IsNullOrWhiteSpace(dbMeta.table))
            {
                var query = $"SELECT TOP 1 {dbMeta.column} FROM {dbMeta.table}";
                widget.text = DbUtil.RawSqlQuery<string>(query, x => x[0].ToString(), connection).FirstOrDefault();
                return;
            }

            if (dbMeta.tabletype == TableTypeProcedure && !string.IsNullOrWhiteSpace(dbMeta.procedureName))
            {
                var parameters = dbMeta.procedureParameters?.ToList() ?? new List<Procedureparameter>();
                var columnName = widget.text ?? dbMeta.column ?? " ";
                widget.text = DbUtil.RawSqlQuery<string>(
                    dbMeta.procedureName,
                    parameters,
                    x => x[columnName].ToString(),
                    connection,
                    CommandType.StoredProcedure).FirstOrDefault();
            }
        }

        private void PopulateTableWidget(DynamicWidgetMeta widget, Dbmeta dbMeta, SqlConnection connection)
        {
            if (dbMeta.tabletype == TableTypeTable && !string.IsNullOrWhiteSpace(dbMeta.column) && !string.IsNullOrWhiteSpace(dbMeta.table))
            {
                var query = $"SELECT TOP 10 {dbMeta.column} FROM {dbMeta.table}";
                var dataTable = DbUtil.GetAllDBRows(query, connection);
                widget.tableData ??= new Tabledata();
                widget.tableData.Headers = dataTable.Columns.Cast<DataColumn>()
                    .Select(c => new TabledataHeaders { column = c.ColumnName, width = 0 })
                    .ToList();
                widget.tableData.Rows = new List<List<string?>>();
                foreach (DataRow row in dataTable.Rows)
                    widget.tableData.Rows.Add(row.ItemArray.Select(x => x?.ToString()).ToList());
                return;
            }

            if (dbMeta.tabletype != TableTypeProcedure || string.IsNullOrWhiteSpace(dbMeta.procedureName))
                return;

            var procedureResult = DbUtil.GetAllDBRows(dbMeta.procedureName, connection, dbMeta.procedureParameters?.ToList());
            widget.tableData ??= new Tabledata();

            var columnNames = widget.tableData.Rows?.FirstOrDefault()
                ?.Select(c => c ?? string.Empty).ToArray()
                ?? procedureResult.Columns.Cast<DataColumn>().Select(c => c.ColumnName).ToArray();
            var selectedTable = procedureResult.DefaultView.ToTable(false, columnNames);

            widget.tableData.Rows = new List<List<string?>>();
            foreach (DataRow row in selectedTable.Rows)
                widget.tableData.Rows.Add(row.ItemArray.Select(x => x?.ToString()).ToList());
        }

        private void PopulateProceduresWidget(DynamicWidgetMeta widget, Dbmeta dbMeta, SqlConnection connection)
        {
            if (dbMeta.tabletype != TableTypeTable || string.IsNullOrWhiteSpace(dbMeta.procedureName))
                return;

            var query = $"EXEC {dbMeta.procedureName} ";
            DbUtil.GetAllDBRows(query, connection);
        }

        private void PopulateFormulaWidget(DynamicWidgetMeta widget, Dbmeta dbMeta, SqlConnection connection)
        {
            var formula = dbMeta.formula ?? widget.miscValues?.formula;
            if (string.IsNullOrWhiteSpace(formula) || string.IsNullOrWhiteSpace(dbMeta.procedureName))
                return;

            var parameters = dbMeta.procedureParameters?.ToList() ?? new List<Procedureparameter>();
            var dataTable = DbUtil.GetAllDBRows(dbMeta.procedureName, connection, parameters);
            if (dataTable.Rows.Count == 0)
                return;

            var firstRow = dataTable.Rows[0];
            var procedureName = dbMeta.procedureName;
            var contextParams = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            foreach (DataColumn col in dataTable.Columns)
            {
                var key = $"{procedureName}.{col.ColumnName}";
                var value = firstRow[col];
                contextParams[key] = value is DBNull || value == null ? string.Empty : value;
            }

            var context = new FormulaContext
            {
                CurrentRow = firstRow,
                AllData = dataTable,
                Parameters = contextParams
            };

            var result = _formulaEvaluationService.EvaluateAsString(formula, context);
            widget.text = result ?? widget.text;
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
