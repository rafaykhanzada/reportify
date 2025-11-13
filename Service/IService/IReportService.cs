using Core.Data.DTOs;
using Core.Utils;

namespace Service.IService
{
    public interface IReportService
    {
        public string UserId { get; set; }
        ResultModel Get();
        Task<int> GetCount();
        Task<ResultModel> Get(int pageIndex, int pageSize, FilterDto? model);
        ResultModel Get(int id);
        ResultModel GetPreview(int id);
        Task<ResultModel> CreateOrUpdate(ReportDto model);
        Task<ResultModel> Delete(int id);
        Task<ResultModel> Export(string? Type, FilterDto? model);
    }
}
