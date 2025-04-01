﻿// <auto-generated />
using System;
using InviteService.Core.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace InviteService.Core.Migrations
{
    [DbContext(typeof(InviteContext))]
    [Migration("20250401063940_InitialMigration")]
    partial class InitialMigration
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.14")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("InviteService.Core.Entity.Invite", b =>
                {
                    b.Property<string>("Code")
                        .HasMaxLength(10)
                        .HasColumnType("character varying(10)");

                    b.Property<string>("GuildId")
                        .HasMaxLength(32)
                        .HasColumnType("character varying(32)");

                    b.Property<DateTime?>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("CreatorId")
                        .HasMaxLength(32)
                        .HasColumnType("character varying(32)");

                    b.HasKey("Code", "GuildId");

                    b.ToTable("Invites");
                });

            modelBuilder.Entity("InviteService.Core.Entity.InviteUse", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uuid");

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasMaxLength(10)
                        .HasColumnType("character varying(10)");

                    b.Property<string>("GuildId")
                        .IsRequired()
                        .HasMaxLength(32)
                        .HasColumnType("character varying(32)");

                    b.Property<DateTime>("UsedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasMaxLength(32)
                        .HasColumnType("character varying(32)");

                    b.HasKey("Id");

                    b.HasIndex("GuildId", "Code");

                    b.ToTable("InviteUses");
                });

            modelBuilder.Entity("InviteService.Core.Entity.InviteUse", b =>
                {
                    b.HasOne("InviteService.Core.Entity.Invite", "Invite")
                        .WithMany("Uses")
                        .HasForeignKey("GuildId", "Code")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Invite");
                });

            modelBuilder.Entity("InviteService.Core.Entity.Invite", b =>
                {
                    b.Navigation("Uses");
                });
#pragma warning restore 612, 618
        }
    }
}
