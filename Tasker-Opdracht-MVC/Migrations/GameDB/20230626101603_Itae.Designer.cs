﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Tasker_Opdracht_MVC.Data;

#nullable disable

namespace Tasker_Opdracht_MVC.Migrations.GameDB
{
    [DbContext(typeof(GameDBContext))]
    [Migration("20230626101603_Itae")]
    partial class Itae
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Tasker_Opdracht_MVC.Data.Entities.Game", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Board")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("GameId")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PlayerWon")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("PlayerWonEmail")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("TimeFinished")
                        .HasColumnType("datetime2");

                    b.Property<string>("User1")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("User2")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.HasKey("Id");

                    b.ToTable("Games");

                    b.HasData(
                        new
                        {
                            Id = 2,
                            Board = "X,X,,O,O,O,,X,",
                            GameId = "0540fe66-b10b-4d28-b7c9-3036120220db",
                            PlayerWon = "user1",
                            PlayerWonEmail = "user1@example.com",
                            TimeFinished = new DateTime(2023, 6, 26, 10, 16, 3, 240, DateTimeKind.Utc).AddTicks(2372),
                            User1 = "user1",
                            User2 = "user2"
                        },
                        new
                        {
                            Id = 1,
                            Board = "X,X,,O,O,O,,X,",
                            GameId = "95276d1e-5b26-439d-b0bc-cec99754781b",
                            PlayerWon = "user3",
                            PlayerWonEmail = "user4@example.com",
                            TimeFinished = new DateTime(2023, 6, 26, 10, 16, 3, 240, DateTimeKind.Utc).AddTicks(2377),
                            User1 = "user3",
                            User2 = "user4"
                        });
                });
#pragma warning restore 612, 618
        }
    }
}