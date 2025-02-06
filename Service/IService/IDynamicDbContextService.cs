using Core.Data.DTOs;
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
        Task<DatabaseWithTablesDto> GetDatabaseWithTablesAndColumnsAsync(string server, string database, string username, string password,string trustCertificate);
        Task<List<Dictionary<string, object>>> ExecuteStoredProcedureAsync(string connectionString, string procedureName, List<StoredProcedureParameterDto> parameters);

        Task<List<Dictionary<string, object>>> ExecuteTableAsync(string connectionString, string tableName, List<string> columns = null);

        Task<List<Dictionary<string, object>>> ExecuteViewAsync(string connectionString, string viewName);
    }
}
