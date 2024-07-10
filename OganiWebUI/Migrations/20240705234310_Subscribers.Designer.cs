﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using OganiWebUI.Models.Contexts;

#nullable disable

namespace OganiWebUI.Migrations
{
    [DbContext(typeof(DataContext))]
    [Migration("20240705234310_Subscribers")]
    partial class Subscribers
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.20")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("OganiWebUI.Models.Entities.ContactPost", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime?>("AnsweredAt")
                        .HasColumnType("datetime");

                    b.Property<int?>("AnsweredBy")
                        .HasColumnType("int");

                    b.Property<string>("AnsweredMessage")
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("varchar");

                    b.Property<string>("FullName")
                        .IsRequired()
                        .HasMaxLength(150)
                        .HasColumnType("nvarchar");

                    b.Property<string>("Message")
                        .IsRequired()
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar");

                    b.HasKey("Id");

                    b.ToTable("ContactPosts", (string)null);
                });

            modelBuilder.Entity("OganiWebUI.Models.Entities.Subscribe", b =>
                {
                    b.Property<string>("Email")
                        .HasMaxLength(100)
                        .HasColumnType("varchar");

                    b.Property<DateTime?>("ApprovedAt")
                        .HasColumnType("datetime");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime");

                    b.HasKey("Email");

                    b.ToTable("Subscribers", (string)null);
                });
#pragma warning restore 612, 618
        }
    }
}
