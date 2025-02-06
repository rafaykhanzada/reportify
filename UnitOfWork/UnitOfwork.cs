using Core.Data.Context;
using Repository.IRepository;
using Repository.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace UnitOfWork
{
    public class UnitOfwork(AppDbContext _db) : IUnitOfWork, IDisposable
    {
        private IWidgetRepository widgetRepository;
        private IWidgetSettingsRepository widgetSettingsRepository;
        private IWidgetPropertyRepository widgetPropertyRepository;
        private IWidgetPropertyDataRepository widgetPropertyDataRepository;
        private IWidgetSaveDataRepository widgetSaveDataRepository;
        private IWidgetReportRepository widgetReportRepository;
        public IWidgetRepository WidgetRepository => widgetRepository ??= new WidgetRepository(_db);
        public IWidgetSettingsRepository WidgetSettingsRepository => widgetSettingsRepository ??= new WidgetSettingsRepository(_db);
        public IWidgetPropertyRepository WidgetPropertyRepository => widgetPropertyRepository ??= new WidgetPropertyRepository(_db);
        public IWidgetPropertyDataRepository WidgetPropertyDataRepository => widgetPropertyDataRepository ??= new WidgetPropertyDataRepository(_db);
        public IWidgetSaveDataRepository WidgetSaveDataRepository => widgetSaveDataRepository ??= new WidgetSaveDataRepository(_db);
        public IWidgetReportRepository WidgetReportRepository => widgetReportRepository ??= new WidgetReportRepository(_db);
        public async Task<int> CompleteAsync() => await _db.SaveChangesAsync();
        public async Task RollbackAsync()
        {
            var entries = _db.ChangeTracker.Entries().ToList();
            foreach (var entry in entries)
            {
                await Task.Run(() => entry.Reload());
            }
        }
        public int Complete() => _db.SaveChanges();

        public void Rollback()
        {
            _db.ChangeTracker.Entries().ToList().ForEach(x => x.Reload());
        }
        public void Dispose()
        {
            _db.Dispose();
        }
    }
}
