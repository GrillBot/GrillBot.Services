﻿using GrillBot.Services.Common.EntityFramework;
using GrillBot.Services.Common.Telemetry.Database;
using Microsoft.EntityFrameworkCore;

namespace InviteService.Core.Entity;

public class InviteContext(DbContextOptions options, DatabaseTelemetryCollector collector) : GrillBotServiceDbContext(options, collector)
{
    public DbSet<Invite> Invites => Set<Invite>();
    public DbSet<InviteUse> InviteUses => Set<InviteUse>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Invite>(b =>
            b.HasMany(o => o.Uses).WithOne(o => o.Invite).HasForeignKey(o => new { o.Code, o.GuildId })
        );
    }
}
