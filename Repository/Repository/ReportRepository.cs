using Core.Data.Context;
using Core.Data.Models;
using Repository.IRepository;

namespace Repository.Repository
{
    public class ReportRepository(AppDbContext context) : RepositoryBase<Report>(context), IReportRepository
    {
    }
}
