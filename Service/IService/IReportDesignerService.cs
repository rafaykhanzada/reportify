using Core.Data.DTOs.ReportDesigner;
using Core.Utils;

namespace Service.IService
{
    /// <summary>
    /// Service for managing report designs
    /// </summary>
    public interface IReportDesignerService
    {
        string UserId { get; set; }
        
        Task<ResultModel> CreateOrUpdate(ReportDesignDto model);
        Task<ResultModel> Get(int id);
        Task<ResultModel> GetAll(int pageIndex = 0, int pageSize = int.MaxValue);
        Task<ResultModel> Delete(int id);
        Task<ResultModel> Duplicate(int id, string newName);
        Task<ResultModel> GetElements(int reportId);
        Task<ResultModel> SaveElement(int reportId, ReportElementDto element);
        Task<ResultModel> DeleteElement(int elementId);
    }
}
