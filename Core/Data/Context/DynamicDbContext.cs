using Core.Data.DTOs;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Data.Context
{
    public class DynamicDbContext : DbContext
    {
        public DynamicDbContext(DbContextOptions<DynamicDbContext> options) : base(options)
        {
        }

        public DbSet<TableDto> Tables { get; set; }
        public DbSet<ColumnDto> Columns { get; set; }

        public DbSet<ParameterDetailsDto> Procedures { get; set; }

        public DbSet<RawViewDataDto> Views { get; set; }

        //public DbSet<DataDto> Data {  get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure TableDto as a keyless entity
            modelBuilder.Entity<TableDto>().HasNoKey();

            // Configure ColumnDto as a keyless entity
            modelBuilder.Entity<ColumnDto>().HasNoKey();

            // Configure TableDto as a keyless entity
            modelBuilder.Entity<ProcedureDetailsDto>().HasNoKey();

            // Configure ColumnDto as a keyless entity
            modelBuilder.Entity<ViewDetailsDto>().HasNoKey();

            // Configure ColumnDto as a keyless entity
            modelBuilder.Entity<RawViewDataDto>().HasNoKey();

            // Configure ColumnDto as a keyless entity
            modelBuilder.Entity<ParameterDetailsDto>().HasNoKey();

        }
    }

}
