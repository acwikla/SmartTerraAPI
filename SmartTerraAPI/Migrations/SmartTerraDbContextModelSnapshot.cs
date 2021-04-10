﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SmartTerraAPI;

namespace SmartTerraAPI.Migrations
{
    [DbContext(typeof(SmartTerraDbContext))]
    partial class SmartTerraDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.9")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("SmartTerraAPI.Models.Device", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

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
                        .HasColumnType("int");

                    b.Property<string>("Body")
                        .IsRequired()
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime(6)");

                    b.Property<int>("DeviceId")
                        .HasColumnType("int");

                    b.Property<bool>("Done")
                        .HasColumnType("tinyint(1)");

                    b.Property<DateTime?>("ExecutionTime")
                        .IsRequired()
                        .HasColumnType("datetime(6)");

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
                        .HasColumnType("int");

                    b.Property<int>("DeviceId")
                        .HasColumnType("int");

                    b.Property<double>("HeatIndex")
                        .HasColumnType("double");

                    b.Property<double>("Humidity")
                        .HasColumnType("double");

                    b.Property<double>("LEDBrightness")
                        .HasColumnType("double");

                    b.Property<string>("LEDHexColor")
                        .IsRequired()
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<double>("SoilMoisturePercentage")
                        .HasColumnType("double");

                    b.Property<double>("Temperature")
                        .HasColumnType("double");

                    b.Property<bool>("isLiquidLevelSufficient")
                        .HasColumnType("tinyint(1)");

                    b.HasKey("Id");

                    b.HasIndex("DeviceId");

                    b.ToTable("DeviceProperties");
                });

            modelBuilder.Entity("SmartTerraAPI.Models.Job", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("Description")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

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
                        .HasColumnType("int");

                    b.Property<int>("DeviceId")
                        .HasColumnType("int");

                    b.Property<TimeSpan>("HourOfDawn")
                        .HasColumnType("time(6)");

                    b.Property<double>("Humidity")
                        .HasColumnType("double");

                    b.Property<bool>("IsOn")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("varchar(60) CHARACTER SET utf8mb4")
                        .HasMaxLength(60);

                    b.Property<double>("Temperature")
                        .HasColumnType("double");

                    b.Property<TimeSpan>("TwilightHour")
                        .HasColumnType("time(6)");

                    b.HasKey("Id");

                    b.HasIndex("DeviceId")
                        .IsUnique();

                    b.ToTable("Modes");
                });

            modelBuilder.Entity("SmartTerraAPI.Models.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("Login")
                        .IsRequired()
                        .HasColumnType("varchar(30) CHARACTER SET utf8mb4")
                        .HasMaxLength(30);

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

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
