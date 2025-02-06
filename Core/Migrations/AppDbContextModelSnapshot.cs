﻿// <auto-generated />
using Core.Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Core.Migrations
{
    [DbContext(typeof(AppDbContext))]
    partial class AppDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.10")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Core.Data.Models.WidgetProperty", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Datasource")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("WidgetId")
                        .HasColumnType("int");

                    b.Property<int>("WsId")
                        .HasColumnType("int");

                    b.Property<string>("pLabel")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("pName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("pType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("pValue")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("WidgetId");

                    b.HasIndex("WsId");

                    b.ToTable("WidgetProperty");
                });

            modelBuilder.Entity("Core.Data.Models.WidgetPropertyData", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("DefaultValue")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("propId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("propId");

                    b.ToTable("WidgetPropertyData");
                });

            modelBuilder.Entity("Core.Data.Models.WidgetReportData", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("Count")
                        .HasColumnType("int");

                    b.Property<string>("ReportName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("WidgetReportData");
                });

            modelBuilder.Entity("Core.Data.Models.WidgetSaveData", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("RId")
                        .HasColumnType("int");

                    b.Property<int>("pId")
                        .HasColumnType("int");

                    b.Property<string>("pLabel")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("pName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("pType")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("pValue")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("pWId")
                        .HasColumnType("int");

                    b.Property<string>("pWidgetName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("RId");

                    b.HasIndex("pId")
                        .IsUnique();

                    b.ToTable("WidgetSaveData");
                });

            modelBuilder.Entity("Core.Data.Models.WidgetSettings", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("WidgetSettings");
                });

            modelBuilder.Entity("Core.Data.Models.Widgets", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("DefaultImage")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("DragImage")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Widgets");
                });

            modelBuilder.Entity("WidgetSettingsWidgets", b =>
                {
                    b.Property<int>("WidgetSettingsId")
                        .HasColumnType("int");

                    b.Property<int>("WidgetsId")
                        .HasColumnType("int");

                    b.HasKey("WidgetSettingsId", "WidgetsId");

                    b.HasIndex("WidgetsId");

                    b.ToTable("WidgetWidgetSettings", (string)null);
                });

            modelBuilder.Entity("Core.Data.Models.WidgetProperty", b =>
                {
                    b.HasOne("Core.Data.Models.Widgets", "Widget")
                        .WithMany()
                        .HasForeignKey("WidgetId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Core.Data.Models.WidgetSettings", "WidgetSettings")
                        .WithMany("WidgetProperty")
                        .HasForeignKey("WsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Widget");

                    b.Navigation("WidgetSettings");
                });

            modelBuilder.Entity("Core.Data.Models.WidgetPropertyData", b =>
                {
                    b.HasOne("Core.Data.Models.WidgetProperty", "WidgetProperty")
                        .WithMany("WidgetPropertyData")
                        .HasForeignKey("propId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("WidgetProperty");
                });

            modelBuilder.Entity("Core.Data.Models.WidgetSaveData", b =>
                {
                    b.HasOne("Core.Data.Models.WidgetReportData", "WidgetReportData")
                        .WithMany("WidgetSaveData")
                        .HasForeignKey("RId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Core.Data.Models.WidgetProperty", "WidgetProperty")
                        .WithOne()
                        .HasForeignKey("Core.Data.Models.WidgetSaveData", "pId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("WidgetProperty");

                    b.Navigation("WidgetReportData");
                });

            modelBuilder.Entity("WidgetSettingsWidgets", b =>
                {
                    b.HasOne("Core.Data.Models.WidgetSettings", null)
                        .WithMany()
                        .HasForeignKey("WidgetSettingsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Core.Data.Models.Widgets", null)
                        .WithMany()
                        .HasForeignKey("WidgetsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Core.Data.Models.WidgetProperty", b =>
                {
                    b.Navigation("WidgetPropertyData");
                });

            modelBuilder.Entity("Core.Data.Models.WidgetReportData", b =>
                {
                    b.Navigation("WidgetSaveData");
                });

            modelBuilder.Entity("Core.Data.Models.WidgetSettings", b =>
                {
                    b.Navigation("WidgetProperty");
                });
#pragma warning restore 612, 618
        }
    }
}
