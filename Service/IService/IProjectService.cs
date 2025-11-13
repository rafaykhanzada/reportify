using Core.Data.DTOs;
using Core.Data.Models;
using Core.Utils;

namespace Service.IService
{
    public interface IProjectService
    {
        public string UserId { get; set; }
        ResultModel Get();
        Task<int> GetCount();
        Task<ResultModel> Get(int pageIndex, int pageSize, FilterDto? model);
        ResultModel Get(int id);
        Task<ResultModel> CreateOrUpdate(ProjectDto model);
        Task<ResultModel> Delete(int id);
        Task<ResultModel> Export(string? Type, FilterDto? model);
    }
}
