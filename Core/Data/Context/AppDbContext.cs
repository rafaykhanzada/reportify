using Core.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Data.Context
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<Widgets> Widgets { get; set; }
        public DbSet<WidgetSettings> WidgetSettings { get; set; }
        public DbSet<WidgetProperty> WidgetProperty { get; set; }
        public DbSet<WidgetPropertyData> WidgetPropertyData { get; set; }
        public DbSet<WidgetReportData> WidgetReportData { get; set; }
        public DbSet<WidgetSaveData> WidgetSaveData { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Widgets>()
            .HasMany(w => w.WidgetSettings)
            .WithMany(ws => ws.Widgets)
            .UsingEntity(j => j.ToTable("WidgetWidgetSettings"));
            modelBuilder.Entity<WidgetProperty>()
        .HasOne(wp => wp.WidgetSettings)
        .WithMany(ws => ws.WidgetProperty)
        .HasForeignKey(wp => wp.WsId);
            modelBuilder.Entity<WidgetPropertyData>()
        .HasOne(wpd => wpd.WidgetProperty)
        .WithMany(wp => wp.WidgetPropertyData)
        .HasForeignKey(wpd => wpd.propId);
            // WidgetSaveData and WidgetPropertyData One-to-One (based on pId as foreign key)
            modelBuilder.Entity<WidgetSaveData>()
                .HasOne(wsd => wsd.WidgetProperty)
                .WithOne()
                .HasForeignKey<WidgetSaveData>(wsd => wsd.pId);

            // WidgetReportData and WidgetSaveData One-to-Many (corrected relationship)
            modelBuilder.Entity<WidgetSaveData>()
                .HasOne(wsd => wsd.WidgetReportData)
                .WithMany(wrd => wrd.WidgetSaveData)
                .HasForeignKey(wsd => wsd.RId);
        }
        


    }
}
