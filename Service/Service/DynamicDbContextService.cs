using Core.Data.Context;
using Core.Data.DTOs;
using Core.Utils;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Service.IService;
using System.Data;
using System.Reflection.PortableExecutable;
using System.Text.Json;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Service.Service
{
    public class DynamicDbContextService : IDynamicDbContextService
    {
        private readonly ResultModel _resultModel=new();
    //    public async Task<DatabaseWithTablesDto> GetDatabaseWithTablesAndColumnsAsync(string server, string database, string username, string password, string trustCertificate)
    //    {
    //        // Build the connection string dynamically
    //        var connectionString = $"Server={server};Database={database};User Id={username};Password={password};TrustServerCertificate={trustCertificate};";

    //        // Use DbContextOptionsBuilder to configure a dynamic DbContext
    //        var optionsBuilder = new DbContextOptionsBuilder<DynamicDbContext>();
    //        optionsBuilder.UseSqlServer(connectionString);

    //        using (var context = new DynamicDbContext(optionsBuilder.Options))
    //        {
    //            // Query for tables in the database
    //            var tables = await context.Tables
    //                .FromSqlRaw($"SELECT TABLE_NAME AS TableName FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE'")
    //                .Select(t => t.TableName)
    //                .ToListAsync();

    //            var tableWithColumnsList = new List<TableWithColumnsDto>();

    //            // Loop through each table and query for its columns
    //            foreach (var table in tables)
    //            {
    //                var columns = await context.Columns
    //                    .FromSqlRaw($@"
    //                SELECT 
    //                    COLUMN_NAME AS ColumnName,
    //                    DATA_TYPE AS DataType,
    //                    IS_NULLABLE AS IsNullable,
    //                    CHARACTER_MAXIMUM_LENGTH AS MaxLength
    //                FROM 
    //                    INFORMATION_SCHEMA.COLUMNS
    //                WHERE 
    //                    TABLE_NAME = '{table}'")
    //                    .Select(c => new ColumnDetailsDto
    //                    {
    //                        ColumnName = c.ColumnName,
    //                        DataType = c.DataType,
    //                        IsNullable = c.IsNullable,
    //                        CharacterMaximumLength = c.MaxLength
    //                    })
    //                    .ToListAsync();

    //                tableWithColumnsList.Add(new TableWithColumnsDto
    //                {
    //                    TableName = table,
    //                    Columns = columns // Now this is a List<ColumnDetailsDto>
    //                });
    //            }

    //            // Query for stored procedures and their properties
    //            var rawProcedureData = await context.Procedures
    //                .FromSqlRaw(@"
    //        SELECT 
    //            p.SPECIFIC_NAME AS ProcedureName,
    //            prm.PARAMETER_NAME AS ParameterName,
    //            prm.DATA_TYPE AS DataType,
    //            prm.PARAMETER_MODE AS ParameterMode,
    //            prm.CHARACTER_MAXIMUM_LENGTH AS MaxLength
    //        FROM 
    //            INFORMATION_SCHEMA.ROUTINES p
    //        LEFT JOIN 
    //            INFORMATION_SCHEMA.PARAMETERS prm
    //        ON 
    //            p.SPECIFIC_NAME = prm.SPECIFIC_NAME
    //        WHERE 
    //            p.ROUTINE_TYPE = 'PROCEDURE'
    //        ").Select(p => new ParameterDetailsDto
    //                {
    //                                ProcedureName = p.ProcedureName,
    //                                 ParameterName = p.ParameterName,
    //                                  DataType = p.DataType,
    //                                  ParameterMode = p.ParameterMode,
    //                                  MaxLength = p.MaxLength
    //                })
    //                .ToListAsync();

    //            // Group by ProcedureName
    //            var procedures = rawProcedureData
    //                .GroupBy(p => p.ProcedureName)
    //                .Select(group => new ProcedureDetailsDto
    //                {
    //                    ProcedureName = group.Key,
    //                    Parameters = group.Select(p => new ParameterDetailsDto
    //                    {
    //                        ParameterName = p.ParameterName,
    //                        DataType = p.DataType,
    //                        ParameterMode = p.ParameterMode,
    //                        MaxLength = p.MaxLength,
    //                        ProcedureName = p.ProcedureName
    //                    }).ToList()
    //                })
    //                .ToList();

    //            // Query for views and their columns
    //            var rawViewData = await context.Views
    //  .FromSqlRaw(@"
    //    SELECT 
    //        v.TABLE_NAME AS ViewName,
    //        c.COLUMN_NAME AS ColumnName,
    //        c.DATA_TYPE AS DataType,
    //        c.CHARACTER_MAXIMUM_LENGTH AS MaxLength,
    //        c.IS_NULLABLE AS IsNullable
    //    FROM 
    //        INFORMATION_SCHEMA.VIEWS v
    //    JOIN 
    //        INFORMATION_SCHEMA.COLUMNS c
    //    ON 
    //        v.TABLE_NAME = c.TABLE_NAME
    //")
    //  .Select(v => new RawViewDataDto
    //  {
    //      ViewName = v.ViewName,
    //      ColumnName = v.ColumnName,
    //      DataType = v.DataType,
    //      MaxLength = v.MaxLength,
    //      IsNullable = v.IsNullable
    //  })
    //  .ToListAsync();

    //            // Ensure correct mapping of the raw data
    //            var views = rawViewData
    //  .GroupBy(v => v.ViewName)
    //  .Select(group => new ViewDetailsDto
    //  {
    //      ViewName = group.Key,
    //      Columns = group.Select(v => new ColumnDetailsDto
    //      {
    //          ColumnName = v.ColumnName,
    //          DataType = v.DataType,
    //          IsNullable = v.IsNullable,
    //          CharacterMaximumLength = v.MaxLength
    //      }).ToList()
    //  })
    //  .ToList();


    //            // Construct the response object
    //            return new DatabaseWithTablesDto
    //            {
    //                DbName = database,
    //                Tables = tableWithColumnsList,
    //                Procedures = procedures,
    //                Views = views
    //            };
    //        }
    //    }


        public async Task<ResultModel> ExecuteStoredProcedureAsync(string connectionString, string procedureName, List<StoredProcedureParameterDto> parameters)
        {

            using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();

                using (var command = new SqlCommand(procedureName, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    // Dynamically add parameters
                    foreach (var parameter in parameters)
                    {
                        var sqlParameter = new SqlParameter
                        {
                            ParameterName = parameter.Name,
                            Value = ConvertParameterValue(parameter.Value, parameter.DataType),
                            SqlDbType = MapToSqlDbType(parameter.DataType)
                        };
                        command.Parameters.Add(sqlParameter);
                    }

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        var result = new List<Dictionary<string, object>>();

                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                var row = new Dictionary<string, object>();
                                for (int i = 0; i < reader.FieldCount; i++)
                                {
                                    row[reader.GetName(i)] = await reader.IsDBNullAsync(i) ? null : reader.GetValue(i);
                                }
                                result.Add(row);
                            }
                        }

                        _resultModel.Data = result;
                        _resultModel.Success = true;
                        return _resultModel;
                    }
                }
            }
        }
        public async Task<ResultModel> ExecuteStoredProcedureSchemaAsync(string connectionString, string procedureName, List<StoredProcedureParameterDto> parameters)
        {
            List<DynamicDbSchema> table_schema = new List<DynamicDbSchema>();
            using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();

                using (var command = new SqlCommand(procedureName, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    // Dynamically add parameters
                    foreach (var parameter in parameters)
                    {
                        var sqlParameter = new SqlParameter
                        {
                            ParameterName = parameter.Name,
                            Value = ConvertParameterValue(parameter.Value, parameter.DataType),
                            SqlDbType = MapToSqlDbType(parameter.DataType)
                        };
                        command.Parameters.Add(sqlParameter);
                    }

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        var result = new List<Dictionary<string, object>>();

                        if (reader.HasRows)
                        {
                            var schema = reader.GetSchemaTable();
                            var json = JsonConvert.SerializeObject(schema);
                            if (schema!=null)
                                table_schema = JsonConvert.DeserializeObject<List<DynamicDbSchema>>(json);
                            foreach (var item in table_schema)
                            {
                                SqlDbType sqlType = (SqlDbType)item.ProviderType;
                                item.DataType = sqlType.ToString().ToLower();
                            }
                            //while (reader.Read())
                            //{
                            //    var row = new Dictionary<string, object>();
                            //    for (int i = 0; i < reader.FieldCount; i++)
                            //    {
                            //        row[reader.GetName(i)] = await reader.IsDBNullAsync(i) ? null : reader.GetValue(i);
                            //    }
                            //    result.Add(row);
                            //}
                        }

                        _resultModel.Data = table_schema;
                        _resultModel.Success = true;
                        return _resultModel;
                    }
                }
            }
        }

        
        
        
        
        //        public async Task<List<Dictionary<string, object>>> ExecuteStoredProcedureAsync(
        //    string connectionString,
        //    string procedureName,
        //    List<StoredProcedureParameterDto> parameters,
        //    int pageNumber = 1,
        //    int pageSize = 100 // Default pagination to prevent large data loads
        //)
        //        {
        //            var result = new List<Dictionary<string, object>>();

        //            try
        //            {
        //                using (var connection = new SqlConnection(connectionString))
        //                {
        //                    await connection.OpenAsync();

        //                    using (var command = new SqlCommand(procedureName, connection))
        //                    {
        //                        command.CommandType = CommandType.StoredProcedure;

        //                        // Add parameters dynamically
        //                        foreach (var parameter in parameters)
        //                        {
        //                            var sqlParameter = new SqlParameter
        //                            {
        //                                ParameterName = parameter.Name,
        //                                Value = ConvertParameterValue(parameter.Value, parameter.DataType),
        //                                SqlDbType = MapToSqlDbType(parameter.DataType)
        //                            };
        //                            command.Parameters.Add(sqlParameter);
        //                        }

        //                        // Add pagination parameters
        //                        command.Parameters.Add(new SqlParameter("@PageNumber", SqlDbType.Int) { Value = pageNumber });
        //                        command.Parameters.Add(new SqlParameter("@PageSize", SqlDbType.Int) { Value = pageSize });

        //                        using (var reader = await command.ExecuteReaderAsync(CommandBehavior.SequentialAccess))
        //                        {
        //                            while (await reader.ReadAsync())
        //                            {
        //                                var row = new Dictionary<string, object>();
        //                                for (int i = 0; i < reader.FieldCount; i++)
        //                                {
        //                                    row[reader.GetName(i)] = await reader.IsDBNullAsync(i) ? null : reader.GetValue(i);
        //                                }
        //                                result.Add(row);
        //                            }
        //                        }
        //                    }
        //                }
        //            }
        //            catch (Exception ex)
        //            {
        //                // Log the error for debugging
        //                Console.WriteLine($"Error executing stored procedure: {ex.Message}");
        //            }

        //            return result;
        //        }


        public async Task<ResultModel> ExecuteViewAsync(string connectionString, string viewName)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();

                string query = $"SELECT * FROM {viewName}"; // Select all data from the view

                using (var command = new SqlCommand(query, connection))
                {
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        var result = new List<Dictionary<string, object>>();

                        while (await reader.ReadAsync())
                        {
                            var row = new Dictionary<string, object>();
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                row[reader.GetName(i)] = await reader.IsDBNullAsync(i) ? null : reader.GetValue(i);
                            }
                            result.Add(row);
                        }
                        _resultModel.Data = result;
                        _resultModel.Success = true;
                        return _resultModel;
                    }
                }
            }
        }

        public async Task<ResultModel> ExecuteTableAsync(string connectionString, string tableName, List<string> columns = null)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();

                // If no columns are provided, select all
                string columnList = (columns == null || !columns.Any()) ? "*" : string.Join(", ", columns);
                string query = $"SELECT {columnList} FROM {tableName}";

                using (var command = new SqlCommand(query, connection))
                {
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        var result = new List<Dictionary<string, object>>();

                        while (await reader.ReadAsync())
                        {
                            var row = new Dictionary<string, object>();
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                row[reader.GetName(i)] = await reader.IsDBNullAsync(i) ? null : reader.GetValue(i);
                            }
                            result.Add(row);
                        }


                        _resultModel.Data = result;
                        _resultModel.Success = true;
                        return _resultModel;
                    }
                }
            }
        }

        public async Task<ResultModel> GetDatabaseSummaryAsync(string server, string database, string username, string password, string trustCertificate)
        {
            // Build the connection string dynamically
            var connectionString = $"Server={server};Database={database};User Id={username};Password={password};TrustServerCertificate={trustCertificate};";

            // Use DbContextOptionsBuilder to configure a dynamic DbContext
            var optionsBuilder = new DbContextOptionsBuilder<DynamicDbContext>();
            optionsBuilder.UseSqlServer(connectionString);

            using (var context = new DynamicDbContext(optionsBuilder.Options))
            {
                // Get table names
                var tableNames = await context.Tables
                    .FromSqlRaw("SELECT TABLE_NAME AS TableName FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE'")
                    .Select(t => t.TableName)
                    .ToListAsync();

                // Get stored procedure names
                var procedureNames = await context.Procedures
                    .FromSqlRaw("SELECT SPECIFIC_NAME AS ProcedureName FROM INFORMATION_SCHEMA.ROUTINES WHERE ROUTINE_TYPE = 'PROCEDURE'")
                    .Select(p => p.ProcedureName)
                    .ToListAsync();

                // Get view names
                var viewNames = await context.Views
                    .FromSqlRaw("SELECT TABLE_NAME AS ViewName FROM INFORMATION_SCHEMA.VIEWS")
                    .Select(v => v.ViewName)
                    .ToListAsync();
                _resultModel.Success = true;
                // Construct the response object
                _resultModel.Data= new DatabaseSummaryDto
                {
                    DbName = database,
                    Tables = tableNames,
                    Procedures = procedureNames,
                    Views = viewNames
                };
                return _resultModel;
            }
        }
        public async Task<ResultModel> GetTableColumnsAsync(string server, string database, string username, string password, string trustCertificate, string tableName)
        {
            // Build the connection string dynamically
            var connectionString = $"Server={server};Database={database};User Id={username};Password={password};TrustServerCertificate={trustCertificate};";

            // Use DbContextOptionsBuilder to configure a dynamic DbContext
            var optionsBuilder = new DbContextOptionsBuilder<DynamicDbContext>();
            optionsBuilder.UseSqlServer(connectionString);

            using (var context = new DynamicDbContext(optionsBuilder.Options))
            {
                // Fetch columns only for the selected table
                var columns = await context.Columns
                    .FromSqlRaw(@"
                SELECT 
                    COLUMN_NAME AS ColumnName,
                    DATA_TYPE AS DataType,
                    IS_NULLABLE AS IsNullable,
                    CHARACTER_MAXIMUM_LENGTH AS MaxLength
                FROM 
                    INFORMATION_SCHEMA.COLUMNS
                WHERE 
                    TABLE_NAME = {0}", tableName) // Prevent SQL Injection
                    .Select(c => new ColumnDetailsDto
                    {
                        ColumnName = c.ColumnName,
                        DataType = c.DataType,
                        IsNullable = c.IsNullable,
                        CharacterMaximumLength = c.MaxLength
                    })
                    .ToListAsync();

                _resultModel.Data= columns;
                _resultModel.Success= true;
                return _resultModel;
            }
        }public async Task<ResultModel> GetTableColumnsAsync(string connectionString, string tableName)
        {
            // Build the connection string dynamically
            // Use DbContextOptionsBuilder to configure a dynamic DbContext
            var optionsBuilder = new DbContextOptionsBuilder<DynamicDbContext>();
            optionsBuilder.UseSqlServer(connectionString);

            using (var context = new DynamicDbContext(optionsBuilder.Options))
            {
                // Fetch columns only for the selected table
                var columns = await context.Columns
                    .FromSqlRaw(@"
                SELECT 
                    COLUMN_NAME AS ColumnName,
                    DATA_TYPE AS DataType,
                    IS_NULLABLE AS IsNullable,
                    CHARACTER_MAXIMUM_LENGTH AS MaxLength
                FROM 
                    INFORMATION_SCHEMA.COLUMNS
                WHERE 
                    TABLE_NAME = {0}", tableName) // Prevent SQL Injection
                    .Select(c => new ColumnDetailsDto
                    {
                        ColumnName = c.ColumnName,
                        DataType = c.DataType,
                        IsNullable = c.IsNullable,
                        CharacterMaximumLength = c.MaxLength
                    })
                    .ToListAsync();

                _resultModel.Data= columns;
                _resultModel.Success= true;
                return _resultModel;
            }
        }
        public async Task<ResultModel> GetProcedureParametersAsync(string server, string database, string username, string password, string trustCertificate, string procedureName)
        {
            // Build the connection string dynamically
            var connectionString = $"Server={server};Database={database};User Id={username};Password={password};TrustServerCertificate={trustCertificate};";

            // Use DbContextOptionsBuilder to configure a dynamic DbContext
            var optionsBuilder = new DbContextOptionsBuilder<DynamicDbContext>();
            optionsBuilder.UseSqlServer(connectionString);

            using (var context = new DynamicDbContext(optionsBuilder.Options))
            {
                var parameters = await context.Procedures
                    .FromSqlRaw(@"
                SELECT 
                    PARAMETER_NAME AS ParameterName,
                    DATA_TYPE AS DataType,
                    PARAMETER_MODE AS ParameterMode,
                    CHARACTER_MAXIMUM_LENGTH AS MaxLength
                FROM 
                    INFORMATION_SCHEMA.PARAMETERS
                WHERE 
                    SPECIFIC_NAME = {0}", procedureName) // Prevent SQL Injection
                    .Select(p => new ParameterDetailsDto
                    {
                        ParameterName = p.ParameterName,
                        DataType = p.DataType,
                        ParameterMode = p.ParameterMode,
                        MaxLength = p.MaxLength
                    })
                    .ToListAsync();

                _resultModel.Data= parameters;
                _resultModel.Success=true;
                return _resultModel;
            }
        }

        public async Task<ResultModel> GetStoredProcedureColumnsAsync(string connectionString, string spName)
        {
            // Initialize the result model
            var resultModel = new ResultModel();

            // Fix: Use the correct type for the list
            var columns = new List<DynamicDbSchema>();

            try
            {
                await using var connection = new SqlConnection(connectionString);
                await connection.OpenAsync();

                await using var command = new SqlCommand("sys.sp_describe_first_result_set", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                command.Parameters.Add(new SqlParameter("@tsql", SqlDbType.NVarChar)
                {
                    Value = $"EXEC {spName}"
                });
                command.Parameters.Add(new SqlParameter("@params", SqlDbType.NVarChar)
                {
                    Value = DBNull.Value
                });
                command.Parameters.Add(new SqlParameter("@browse_information_mode", SqlDbType.TinyInt)
                {
                    Value = 0
                });

                await using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    // Fix: Read safely from the SqlDataReader and map the specific 
                    // column names returned by 'sys.sp_describe_first_result_set'
                    columns.Add(new DynamicDbSchema
                    {
                        ColumnName = reader["name"] as string,
                        ColumnOrdinal = reader["column_ordinal"] == DBNull.Value ? null : Convert.ToInt32(reader["column_ordinal"]),
                        ColumnSize = reader["max_length"] == DBNull.Value ? null : Convert.ToInt32(reader["max_length"]),
                        NumericPrecision = reader["precision"] == DBNull.Value ? null : Convert.ToInt32(reader["precision"]),
                        IsUnique = reader["is_part_of_unique_key"] == DBNull.Value ? null : Convert.ToBoolean(reader["is_part_of_unique_key"]),
                        DataType = reader["system_type_name"] as string,
                        AllowDBNull = reader["is_nullable"] == DBNull.Value ? null : Convert.ToBoolean(reader["is_nullable"]),
                        ProviderType = reader["system_type_id"] == DBNull.Value ? null : Convert.ToInt32(reader["system_type_id"]),
                        IsIdentity = reader["is_identity_column"] == DBNull.Value ? null : Convert.ToBoolean(reader["is_identity_column"]),
                        IsAutoIncrement = reader["is_identity_column"] == DBNull.Value ? null : Convert.ToBoolean(reader["is_identity_column"]),
                        IsColumnSet = reader["is_sparse_column_set"] == DBNull.Value ? null : Convert.ToBoolean(reader["is_sparse_column_set"]),
                    });
                }

                resultModel.Data = columns;
                resultModel.Success = true;
            }
            catch (SqlException ex)
            {
                resultModel.Success = false;
                resultModel.Message = $"SQL Error: {ex.Message}";
            }
            catch (Exception ex)
            {
                resultModel.Success = false;
                resultModel.Message = $"Error: {ex.Message}";
            }

            return resultModel;
        }
    public async Task<ResultModel> GetViewColumnsAsync(string server, string database, string username, string password, string trustCertificate, string viewName)
        {
            // Build the connection string dynamically
            var connectionString = $"Server={server};Database={database};User Id={username};Password={password};TrustServerCertificate={trustCertificate};";

            // Use DbContextOptionsBuilder to configure a dynamic DbContext
            var optionsBuilder = new DbContextOptionsBuilder<DynamicDbContext>();
            optionsBuilder.UseSqlServer(connectionString);

            using (var context = new DynamicDbContext(optionsBuilder.Options))
            {
                var viewColumns = await context.Views
                    .FromSqlRaw(@"
                SELECT 
                    COLUMN_NAME AS ColumnName,
                    DATA_TYPE AS DataType,
                    IS_NULLABLE AS IsNullable,
                    CHARACTER_MAXIMUM_LENGTH AS MaxLength
                FROM 
                    INFORMATION_SCHEMA.COLUMNS
                WHERE 
                    TABLE_NAME = {0}", viewName) // Prevent SQL Injection
                    .Select(v => new ColumnDetailsDto
                    {
                        ColumnName = v.ColumnName,
                        DataType = v.DataType,
                        IsNullable = v.IsNullable,
                        CharacterMaximumLength = v.MaxLength
                    })
                    .ToListAsync();

                _resultModel.Data= viewColumns;
                _resultModel.Success=true;
                return _resultModel;
            }
        }

        private SqlDbType MapToSqlDbType(string dataType)
        {
            return dataType.ToLower() switch
            {
                "int" => SqlDbType.Int,
                "varchar" => SqlDbType.VarChar,
                "nvarchar" => SqlDbType.NVarChar,
                "decimal" => SqlDbType.Decimal,
                "bit" => SqlDbType.Bit,
                "datetime" => SqlDbType.DateTime,
                "date" => SqlDbType.DateTime,
                _ => throw new ArgumentException($"Unsupported data type: {dataType}")
            };
        }
    //    public async Task<List<Dictionary<string, object>>> ExecuteTableAsync(
    //string connectionString, string tableName, List<string> columns = null)
    //    {
    //        var result = new List<Dictionary<string, object>>();

    //        await using var connection = new SqlConnection(connectionString);
    //        await connection.OpenAsync().ConfigureAwait(false);

    //        // Ensure column names are properly formatted to prevent SQL injection risk
    //        string columnList = (columns == null || !columns.Any()) ? "*" : string.Join(", ", columns.Select(c => $"[{c}]"));
    //        string query = $"SELECT {columnList} FROM [{tableName}]"; // Wrap table name in brackets

    //        await using var command = new SqlCommand(query, connection);
    //        await using var reader = await command.ExecuteReaderAsync().ConfigureAwait(false);

    //        while (await reader.ReadAsync().ConfigureAwait(false))
    //        {
    //            var row = new Dictionary<string, object>(reader.FieldCount);
    //            var readTasks = new List<Task>(); // List to store async tasks

    //            for (int i = 0; i < reader.FieldCount; i++)
    //            {
    //                int index = i; // Capture the index inside the loop
    //                readTasks.Add(Task.Run(async () =>
    //                {
    //                    var value = await reader.IsDBNullAsync(index).ConfigureAwait(false) ? null : reader.GetValue(index);
    //                    row[reader.GetName(index)] = value;
    //                }));
    //            }

    //            await Task.WhenAll(readTasks); // Wait for all parallel read tasks
    //            result.Add(row);
    //        }

    //        return result;
    //    }

        private object ConvertParameterValue(object value, string dataType)
        {
            if (value is JsonElement jsonElement)
            {
                switch (dataType.ToLower())
                {
                    case "datetime":
                        return jsonElement.GetDateTime();  // Converts JSON DateTime to C# DateTime
                    case "int":
                        return jsonElement.GetInt32();
                    case "decimal":
                        return jsonElement.GetDecimal();
                    case "bit":
                        return jsonElement.GetBoolean();
                    default:
                        return jsonElement.ToString(); // Convert other types to string (e.g., varchar, nvarchar)
                }
            }
            return value ?? DBNull.Value;
        }

    }





}