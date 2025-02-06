using Core.Data.Context;
using Core.Data.Models;
using Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Repository
{
    public class WidgetSaveDataRepository(AppDbContext context):RepositoryBase<WidgetSaveData>(context),IWidgetSaveDataRepository
    {
    }
}
