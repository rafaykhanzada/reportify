using OfficeOpenXml;
using System.Reflection;
using System.Text;

namespace Core.Utils
{
    public static class ExportUtility
    {
        public static byte[] ExportToExcel<T>(List<T> objectList) where T : class
        {
            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
            using (ExcelPackage package = new ExcelPackage())
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Sheet1");

                // Get the properties of the object type T
                PropertyInfo[] properties = typeof(T).GetProperties();

                // Set the headers
                for (int i = 0; i < properties.Length; i++)
                {
                    worksheet.Cells[1, i + 1].Value = properties[i].Name;
                }


                // Populate the data
                for (int row = 0; row < objectList.Count; row++)
                {
                    for (int col = 0; col < properties.Length; col++)
                    {
                        worksheet.Cells[row + 2, col + 1].Value = properties[col].GetValue(objectList[row]);
                    }
                }

                // Auto-fit columns for better display
                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                // Convert the Excel package to a byte array
                byte[] fileContents = package.GetAsByteArray();

                return fileContents;
            }
        }
        public static byte[] ExportToCSV<T>(List<T> objectList) where T : class
        {
            if (objectList == null || !objectList.Any())
            {
                throw new ArgumentException("The object list is empty or null.");
            }

            var stringBuilder = new StringBuilder();

            // Get the properties of the object type T
            PropertyInfo[] properties = typeof(T).GetProperties();

            if (properties == null || properties.Length == 0)
            {
                throw new ArgumentException("The object type has no properties.");
            }

            // Write the header line
            stringBuilder.AppendLine(string.Join(",", properties.Select(p => p.Name)));

            // Write the data lines
            foreach (var obj in objectList)
            {
                var values = properties.Select(p =>
                {
                    var value = p.GetValue(obj)?.ToString() ?? string.Empty;
                    // Escape commas and quotes
                    if (value.Contains(",") || value.Contains("\""))
                    {
                        value = "\"" + value.Replace("\"", "\"\"") + "\"";
                    }
                    return value;
                });
                stringBuilder.AppendLine(string.Join(",", values));
            }

            // Convert the StringBuilder content to a byte array
            return Encoding.UTF8.GetBytes(stringBuilder.ToString());
        }
        public static byte[] ExportToExcelLines<T>(List<T> objectList) where T : class
        {
            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
            using (ExcelPackage package = new ExcelPackage())
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Sheet1");

                // Get the properties of the object type T
                PropertyInfo[] properties = typeof(T).GetProperties();

                // Set the headers
                for (int i = 0; i < properties.Length; i++)
                {
                    worksheet.Cells[1, i + 1].Value = properties[i].Name;
                }

                // Populate the data
                for (int row = 0; row < objectList.Count; row++)
                {
                    for (int col = 0; col < properties.Length; col++)
                    {
                        object value = properties[col].GetValue(objectList[row]);
                        InsertPropertyValue(worksheet.Cells[row + 2, col + 1], value);
                    }
                }

                // Auto-fit columns for better display
                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                // Convert the Excel package to a byte array
                byte[] fileContents = package.GetAsByteArray();

                return fileContents;
            }
        }

        private static void InsertPropertyValue(ExcelRangeBase cell, object value)
        {
            if (value == null)
            {
                cell.Value = DBNull.Value;
                return;
            }

            Type valueType = value.GetType();

            // Check if the value type is a simple type or a custom class
            if (valueType.IsPrimitive || valueType == typeof(string) || valueType == typeof(DateTime) || valueType == typeof(decimal))
            {
                cell.Value = value;
            }
            else if (valueType.IsClass)
            {
                // If it's a class, recursively insert its properties
                PropertyInfo[] nestedProperties = valueType.GetProperties();
                for (int i = 0; i < nestedProperties.Length; i++)
                {
                    ExcelRangeBase nestedCell = cell.Worksheet.Cells[cell.Start.Row, cell.Start.Column + i + 1];
                    object nestedValue = nestedProperties[i].GetValue(value);
                    InsertPropertyValue(nestedCell, nestedValue);
                }
            }
            else
            {
                // Handle other types as needed
                cell.Value = value.ToString();
            }
        }
    }
}
