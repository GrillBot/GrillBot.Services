﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using RubbergodService.Core.Entity;

#nullable disable

namespace RubbergodService.Core.Migrations
{
    [DbContext(typeof(RubbergodServiceContext))]
    [Migration("20230704134333_DroppedMemberCacheTable")]
    partial class DroppedMemberCacheTable
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("RubbergodService.Core.Entity.Karma", b =>
                {
                    b.Property<string>("MemberId")
                        .HasMaxLength(32)
                        .HasColumnType("character varying(32)")
                        .HasAnnotation("Relational:JsonPropertyName", "member_ID");

                    b.Property<int>("KarmaValue")
                        .HasColumnType("integer")
                        .HasAnnotation("Relational:JsonPropertyName", "karma");

                    b.Property<int>("Negative")
                        .HasColumnType("integer")
                        .HasAnnotation("Relational:JsonPropertyName", "negative");

                    b.Property<int>("Positive")
                        .HasColumnType("integer")
                        .HasAnnotation("Relational:JsonPropertyName", "positive");

                    b.HasKey("MemberId");

                    b.ToTable("Karma");
                });

            modelBuilder.Entity("RubbergodService.Core.Entity.PinCacheItem", b =>
                {
                    b.Property<string>("GuildId")
                        .HasMaxLength(32)
                        .HasColumnType("character varying(32)");

                    b.Property<string>("ChannelId")
                        .HasMaxLength(32)
                        .HasColumnType("character varying(32)");

                    b.Property<string>("Filename")
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<DateTime>("CreatedAtUtc")
                        .HasColumnType("timestamp with time zone");

                    b.Property<byte[]>("Data")
                        .IsRequired()
                        .HasColumnType("bytea");

                    b.HasKey("GuildId", "ChannelId", "Filename");

                    b.ToTable("PinCache");
                });
#pragma warning restore 612, 618
        }
    }
}
