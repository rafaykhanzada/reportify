using Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        IWidgetRepository WidgetRepository { get; }
        IWidgetPropertyDataRepository WidgetPropertyDataRepository { get; }
        IWidgetSettingsRepository WidgetSettingsRepository { get; }
        IWidgetPropertyRepository WidgetPropertyRepository { get; }
        IWidgetReportRepository WidgetReportRepository { get; }
        IWidgetSaveDataRepository WidgetSaveDataRepository { get; }
        Task RollbackAsync();
        Task<int> CompleteAsync();
        int Complete();
    }
}
