﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using Property.Core.Database;

#nullable disable

namespace Property.Core.Migrations
{
    [DbContext(typeof(PropertyContext))]
    [Migration("20240123043028_V8")]
    partial class V8
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.HasPostgresExtension(modelBuilder, "pg_trgm");
            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Property.Core.Model.Customer", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("FullName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Customers");
                });

            modelBuilder.Entity("Property.Core.Model.Property", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<bool>("AutoAcceptReservations")
                        .HasColumnType("boolean");

                    b.Property<double>("AverageRating")
                        .HasColumnType("double precision");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string[]>("Images")
                        .IsRequired()
                        .HasColumnType("text[]");

                    b.Property<string>("Location")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("MaxPeople")
                        .HasColumnType("integer");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<int>("NumberOfReviews")
                        .HasColumnType("integer");

                    b.Property<Guid>("OwnerId")
                        .HasColumnType("uuid");

                    b.Property<int>("PricingStrategy")
                        .HasColumnType("integer");

                    b.Property<decimal>("UnitPrice")
                        .HasColumnType("numeric");

                    b.HasKey("Id");

                    b.HasIndex("Location");

                    NpgsqlIndexBuilderExtensions.HasMethod(b.HasIndex("Location"), "gin");
                    NpgsqlIndexBuilderExtensions.HasOperators(b.HasIndex("Location"), new[] { "gin_trgm_ops" });

                    b.HasIndex("MaxPeople");

                    b.ToTable("Properties");
                });

            modelBuilder.Entity("Property.Core.Model.Reservation", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTimeOffset>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid>("CustomerId")
                        .HasColumnType("uuid");

                    b.Property<DateOnly>("End")
                        .HasColumnType("date");

                    b.Property<int>("People")
                        .HasColumnType("integer");

                    b.Property<decimal>("Price")
                        .HasColumnType("numeric");

                    b.Property<Guid>("PropertyId")
                        .HasColumnType("uuid");

                    b.Property<bool>("ReviewedByCustomer")
                        .HasColumnType("boolean");

                    b.Property<Guid?>("SlotId")
                        .HasColumnType("uuid");

                    b.Property<DateOnly>("Start")
                        .HasColumnType("date");

                    b.Property<int>("Status")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("CustomerId");

                    b.HasIndex("PropertyId");

                    b.HasIndex("SlotId");

                    b.ToTable("Reservations");
                });

            modelBuilder.Entity("Property.Core.Model.Review", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<Guid>("CustomerId")
                        .HasColumnType("uuid");

                    b.Property<Guid?>("PropertyId")
                        .HasColumnType("uuid");

                    b.Property<int>("Rating")
                        .HasColumnType("integer");

                    b.Property<DateTimeOffset>("Timestamp")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.HasIndex("CustomerId");

                    b.HasIndex("PropertyId");

                    b.ToTable("Reviews");
                });

            modelBuilder.Entity("Property.Core.Model.Slot", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateOnly>("End")
                        .HasColumnType("date");

                    b.Property<bool>("IsAvailable")
                        .HasColumnType("boolean");

                    b.Property<Guid>("PropertyId")
                        .HasColumnType("uuid");

                    b.Property<DateOnly>("Start")
                        .HasColumnType("date");

                    b.HasKey("Id");

                    b.HasIndex("End");

                    b.HasIndex("IsAvailable");

                    b.HasIndex("PropertyId");

                    b.HasIndex("Start");

                    b.ToTable("Slots");
                });

            modelBuilder.Entity("Property.Core.Model.Reservation", b =>
                {
                    b.HasOne("Property.Core.Model.Customer", "Customer")
                        .WithMany()
                        .HasForeignKey("CustomerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Property.Core.Model.Property", "Property")
                        .WithMany()
                        .HasForeignKey("PropertyId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Property.Core.Model.Slot", "Slot")
                        .WithMany()
                        .HasForeignKey("SlotId");

                    b.Navigation("Customer");

                    b.Navigation("Property");

                    b.Navigation("Slot");
                });

            modelBuilder.Entity("Property.Core.Model.Review", b =>
                {
                    b.HasOne("Property.Core.Model.Customer", "Customer")
                        .WithMany()
                        .HasForeignKey("CustomerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Property.Core.Model.Property", null)
                        .WithMany("Reviews")
                        .HasForeignKey("PropertyId");

                    b.Navigation("Customer");
                });

            modelBuilder.Entity("Property.Core.Model.Slot", b =>
                {
                    b.HasOne("Property.Core.Model.Property", "Property")
                        .WithMany()
                        .HasForeignKey("PropertyId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Property");
                });

            modelBuilder.Entity("Property.Core.Model.Property", b =>
                {
                    b.Navigation("Reviews");
                });
#pragma warning restore 612, 618
        }
    }
}
