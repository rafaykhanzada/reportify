using Core.Data.Context;
using Core.Data.Models;
using Repository.IRepository;

namespace Repository.Repository
{
    public class ProjectRepository : RepositoryBase<Project>, IProjectRepository
    {
        public ProjectRepository(AppDbContext context) : base(context)
        {
        }
    }
}
