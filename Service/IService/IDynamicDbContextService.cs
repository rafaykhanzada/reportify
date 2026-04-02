using Core.Data.DTOs;
using Core.Utils;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.IService
{
    public interface IDynamicDbContextService
    {
        //Task<DatabaseWithTablesDto> GetDatabaseWithTablesAndColumnsAsync(string server, string database, string username, string password,string trustCertificate);
        Task<ResultModel> ExecuteStoredProcedureAsync(string connectionString, string procedureName, List<StoredProcedureParameterDto> parameters);
        Task<ResultModel> ExecuteStoredProcedureSchemaAsync(string connectionString, string procedureName, List<StoredProcedureParameterDto> parameters);

        Task<ResultModel> ExecuteTableAsync(string connectionString, string tableName, List<string> columns = null);

        Task<ResultModel> ExecuteViewAsync(string connectionString, string viewName);

        Task<ResultModel> GetDatabaseSummaryAsync(string server, string database, string username, string password, string trustCertificate);
        Task<ResultModel> GetTableColumnsAsync(string server, string database, string username, string password, string trustCertificate, string tableName);
        Task<ResultModel> GetTableColumnsAsync(string connectionString, string tableName);
        Task<ResultModel> GetStoredProcedureColumnsAsync(string connectionString, string spName);
        Task<ResultModel> GetProcedureParametersAsync(string server, string database, string username, string password, string trustCertificate, string procedureName);
        Task<ResultModel> GetViewColumnsAsync(string server, string database, string username, string password, string trustCertificate, string viewName);
    }
}
