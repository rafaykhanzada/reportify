using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Data.DTOs
{
    public class DynamicDbDTO
    {
    }
    public class DatabaseWithTablesDto
    {
        public string DbName { get; set; }
        public List<TableWithColumnsDto> Tables { get; set; }

        public List<ProcedureDetailsDto> Procedures { get; set; }

        public List<ViewDetailsDto> Views { get; set; }
    }

    //public class TableWithColumnsDto
    //{
    //    public string TableName { get; set; }
    //    public List<string> Columns { get; set; }


    //}
    public class TableWithColumnsDto
    {
        public string TableName { get; set; }
        public List<ColumnDetailsDto> Columns { get; set; }  // Change this to List<ColumnDetailsDto>
    }

    public class TableDto
    {
        public string TableName { get; set; }
    }

    public class ColumnDto
    {
        public string ColumnName { get; set; }
        public string DataType { get; set; }
        public string IsNullable { get; set; }
        public int? MaxLength { get; set; }
        //public int? NUMERIC_PRECISION { get; set; }
        //public int? NUMERIC_SCALE { get; set; }
    }
    //public class DataDto
    //{
    //    public object DataName { get; set; }
    //    public class DatabaseWithTablesDto
    //    {
    //        public string DbName { get; set; }
    //        public List<TableWithColumnsDto> Tables { get; set; }
    //    }

    //public class TableWithColumnsDto
    //{
    //    public string TableName { get; set; }
    //    public List<ColumnWithDataDto> Columns { get; set; }
    //}

    //    public class ColumnWithDataDto
    //    {
    //        public string ColumnName { get; set; }
    //        public List<object> Data { get; set; }  // Using 'object' to support different types of column data
    //    }

    public class ProcedureDetailsDto
    {
        public string ProcedureName { get; set; }
        public List<ParameterDetailsDto> Parameters { get; set; }
    }

    public class ParameterDetailsDto
    {

        public string ParameterName { get; set; }
        public string DataType { get; set; }
        public string ParameterMode { get; set; }
        public string ProcedureName { get; set; }
        public int? MaxLength { get; set; }

    }

    public class ViewDetailsDto
    {
        public string ViewName { get; set; }
        public List<ColumnDetailsDto> Columns { get; set; }

    }
    public class RawViewDataDto
    {
        public string ViewName { get; set; }
        public string ColumnName { get; set; }
        public string DataType { get; set; }
        public int? MaxLength { get; set; }
        public string IsNullable { get; set; }
    }

    public class ColumnDetailsDto
    {
        public string ColumnName  { get; set; }

        public string DataType { get; set; }
        public string IsNullable { get; set; }
        public int? CharacterMaximumLength { get; set; }

    }

    public class DynamicDataDto
    {
        public List<TableWithColumnsDto> Tables { get; set; }
        public List<ProcedureDetailsDto> Procedures { get; set; }
        public List<ViewDetailsDto> Views { get; set; }
    }

    public class StoredProcedureParameterDto
    {
        public string Name { get; set; } // Parameter name
        public object Value { get; set; } // Parameter value
        public string DataType { get; set; } // Parameter SQL data type (e.g., int, varchar)
    }

    public class RequestDynamicDTO
    {
        public string Type { get; set; }
        public string ConnectionString { get; set; }
        public string name { get; set;}

        public List<StoredProcedureParameterDto>? Parameters { get; set; } = null; // Optional
        public List<string>? Columns { get; set; } = null; // Optional

    }



}

