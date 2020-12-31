﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SmartTerraAPI;

namespace SmartTerraAPI.Migrations
{
    [DbContext(typeof(SmartTerraDbContext))]
    [Migration("20201231004122_UdateJobsDescriptionV2")]
    partial class UdateJobsDescriptionV2
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.9")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("SmartTerraAPI.Models.Device", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("Devices");
                });

            modelBuilder.Entity("SmartTerraAPI.Models.DeviceJob", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Body")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreatedDate")
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("datetime2")
                        .HasDefaultValueSql("GETDATE()");

                    b.Property<int>("DeviceId")
                        .HasColumnType("int");

                    b.Property<bool>("Done")
                        .HasColumnType("bit");

                    b.Property<DateTime?>("ExecutionTime")
                        .IsRequired()
                        .HasColumnType("datetime2");

                    b.Property<int>("JobId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("DeviceId");

                    b.HasIndex("JobId");

                    b.ToTable("DeviceJobs");
                });

            modelBuilder.Entity("SmartTerraAPI.Models.DeviceProperties", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("DeviceId")
                        .HasColumnType("int");

                    b.Property<double>("HeatIndex")
                        .HasColumnType("float");

                    b.Property<double>("Humidity")
                        .HasColumnType("float");

                    b.Property<double>("LEDBrightness")
                        .HasColumnType("float");

                    b.Property<string>("LEDHexColor")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<double>("SoilMoisturePercentage")
                        .HasColumnType("float");

                    b.Property<double>("Temperature")
                        .HasColumnType("float");

                    b.Property<bool>("isLiquidLevelSufficient")
                        .HasColumnType("bit");

                    b.HasKey("Id");

                    b.HasIndex("DeviceId");

                    b.ToTable("DeviceProperties");
                });

            modelBuilder.Entity("SmartTerraAPI.Models.Job", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Jobs");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Description = "Turn on the LED strip and set color of the LEDs .",
                            Name = "TurnOnLED",
                            Type = "LED"
                        },
                        new
                        {
                            Id = 2,
                            Description = "Turn off the LED strip.",
                            Name = "TurnOffLED",
                            Type = "LED"
                        },
                        new
                        {
                            Id = 3,
                            Description = "Turn on the water pump for given period of time.",
                            Name = "TurnOnWaterPump",
                            Type = "PUMP"
                        });
                });

            modelBuilder.Entity("SmartTerraAPI.Models.Mode", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("DeviceId")
                        .HasColumnType("int");

                    b.Property<TimeSpan>("HourOfDawn")
                        .HasColumnType("time");

                    b.Property<double>("Humidity")
                        .HasColumnType("float");

                    b.Property<bool>("IsOn")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(60)")
                        .HasMaxLength(60);

                    b.Property<double>("Temperature")
                        .HasColumnType("float");

                    b.Property<TimeSpan>("TwilightHour")
                        .HasColumnType("time");

                    b.HasKey("Id");

                    b.HasIndex("DeviceId")
                        .IsUnique();

                    b.ToTable("Modes");
                });

            modelBuilder.Entity("SmartTerraAPI.Models.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Login")
                        .IsRequired()
                        .HasColumnType("nvarchar(30)")
                        .HasMaxLength(30);

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("SmartTerraAPI.Models.Device", b =>
                {
                    b.HasOne("SmartTerraAPI.Models.User", "User")
                        .WithMany("Devices")
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("SmartTerraAPI.Models.DeviceJob", b =>
                {
                    b.HasOne("SmartTerraAPI.Models.Device", "Device")
                        .WithMany("DeviceJobs")
                        .HasForeignKey("DeviceId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SmartTerraAPI.Models.Job", "Job")
                        .WithMany("DeviceJobs")
                        .HasForeignKey("JobId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("SmartTerraAPI.Models.DeviceProperties", b =>
                {
                    b.HasOne("SmartTerraAPI.Models.Device", "Device")
                        .WithMany("DeviceProperties")
                        .HasForeignKey("DeviceId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("SmartTerraAPI.Models.Mode", b =>
                {
                    b.HasOne("SmartTerraAPI.Models.Device", "Device")
                        .WithOne("Mode")
                        .HasForeignKey("SmartTerraAPI.Models.Mode", "DeviceId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
