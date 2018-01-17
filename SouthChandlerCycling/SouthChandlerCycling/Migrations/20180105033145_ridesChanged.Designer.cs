﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.Internal;
using SouthChandlerCycling.Data;
using System;

namespace SouthChandlerCycling.Migrations
{
    [DbContext(typeof(SCCDataContext))]
    [Migration("20180105033145_ridesChanged")]
    partial class ridesChanged
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.0.1-rtm-125")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("SouthChandlerCycling.Models.Ride", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("CreatorId")
                        .HasColumnName("Creator");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnName("Description")
                        .HasMaxLength(120);

                    b.Property<double>("Distance")
                        .HasColumnName("Distance");

                    b.Property<string>("RideName")
                        .IsRequired()
                        .HasColumnName("RideName")
                        .HasMaxLength(50);

                    b.Property<DateTime>("StartDate")
                        .HasColumnName("StartDate");

                    b.HasKey("ID");

                    b.ToTable("Rides");
                });

            modelBuilder.Entity("SouthChandlerCycling.Models.Rider", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("ActiveRide");

                    b.Property<string>("EmailAddress")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasColumnName("FirstName")
                        .HasMaxLength(50);

                    b.Property<string>("LastLatitude");

                    b.Property<string>("LastLongitude");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.Property<long>("LastRide");

                    b.Property<string>("Password");

                    b.Property<string>("PhoneNumber")
                        .IsRequired()
                        .HasMaxLength(14);

                    b.Property<string>("Role")
                        .HasMaxLength(50);

                    b.Property<string>("Salt");

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasColumnName("UserName")
                        .HasMaxLength(50);

                    b.HasKey("ID");

                    b.ToTable("Riders");
                });

            modelBuilder.Entity("SouthChandlerCycling.Models.Signup", b =>
                {
                    b.Property<int>("SignupID")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("RideID");

                    b.Property<int>("RiderID");

                    b.HasKey("SignupID");

                    b.HasIndex("RideID");

                    b.HasIndex("RiderID");

                    b.ToTable("SignUps");
                });

            modelBuilder.Entity("SouthChandlerCycling.Models.Signup", b =>
                {
                    b.HasOne("SouthChandlerCycling.Models.Ride", "ActualRide")
                        .WithMany("Signups")
                        .HasForeignKey("RideID")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("SouthChandlerCycling.Models.Rider", "ActualRider")
                        .WithMany("Signups")
                        .HasForeignKey("RiderID")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
