using Core.Data.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.IService;

namespace ReportGeneratorProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DynamicDbController : ControllerBase
    {
        private readonly IDynamicDbContextService _dynamicDbContextService;

        public DynamicDbController(IDynamicDbContextService dynamicDbContextService)
        {
            _dynamicDbContextService = dynamicDbContextService;
        }
        public class DynamicDbRequest
        {
            public string Server { get; set; }
            public string Database { get; set; }
            public string Username { get; set; }
            public string Password { get; set; }

            public string Certificate { get; set; }
        }
        [HttpPost("dynamicdb")]
        public async Task<IActionResult> DynamicDb([FromBody] DynamicDbRequest request)
        {
            try
            {
                // Call the service to retrieve the database tables and columns
                var databaseWithTables = await _dynamicDbContextService.GetDatabaseWithTablesAndColumnsAsync(request.Server, request.Database, request.Username, request.Password,request.Certificate);

                // Return the JSON response
                return Ok(databaseWithTables);
            }
            catch (Exception ex)
            {
                // Handle errors
                return BadRequest($"Error: {ex.Message}");
            }
        }

        //[HttpPost("dynamicProcedures")]
        //public async Task<IActionResult> DynamicSPs(RequestDynamicDTO request)
        //{
        //    try
        //    {
        //        if (request.Type.ToLower() == "procedure")
        //        {
        //            if (request.Parameters == null)
        //            {
        //                return BadRequest("Provide parameters for the procedure to execute");
        //            }
        //            var result = await _dynamicDbContextService.ExecuteStoredProcedureAsync(request.ConnectionString, request.name, request.Parameters);

        //            return Ok(result);
        //        }
        //        else if(request.Type.ToLower() == "views")
        //        {

        //            var result = await _dynamicDbContextService.ExecuteViewAsync(request.ConnectionString, request.name);

        //            return Ok(result);
        //        }
        //        else if(request.Type.ToLower() == "tables")
        //        {
        //            var result = await _dynamicDbContextService.ExecuteTableAsync(request.ConnectionString, request.name, request.Columns);

        //            return Ok(result);
        //        }
        //        else
        //        {
        //            return BadRequest("Type not provided");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }
        //}

        [HttpPost("dynamicProcedures")]
        public async Task<IActionResult> DynamicSPs(RequestDynamicDTO request)
        {
            try
            {
                object result; // Declaring result at the start

                switch (request.Type.ToLower())
                {
                    case "procedure":
                        if (request.Parameters == null)
                        {
                            return BadRequest("Provide parameters for the procedure to execute");
                        }
                        result = await _dynamicDbContextService.ExecuteStoredProcedureAsync(
                            request.ConnectionString, request.name, request.Parameters);
                        break;

                    case "views":
                        result = await _dynamicDbContextService.ExecuteViewAsync(
                            request.ConnectionString, request.name);
                        break;

                    case "tables":
                        result = await _dynamicDbContextService.ExecuteTableAsync(
                            request.ConnectionString, request.name, request.Columns);
                        break;

                    default:
                        return BadRequest("Invalid Type. Valid options are: procedure, views, tables.");
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


    }
}
