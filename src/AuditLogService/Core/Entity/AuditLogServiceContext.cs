using GrillBot.Core.Database.ValueConverters;
using GrillBot.Services.Common.EntityFramework;
using GrillBot.Services.Common.Telemetry.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Linq.Expressions;

namespace AuditLogService.Core.Entity;

public class AuditLogServiceContext(DbContextOptions<AuditLogServiceContext> options, DatabaseTelemetryCollector collector) : GrillBotServiceDbContext(options, collector)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        static void WithForeignKeyReference<TEntity>(EntityTypeBuilder<LogItem> builder, Expression<Func<LogItem, TEntity?>> hasOneExpression) where TEntity : ChildEntityBase
            => builder.HasOne(hasOneExpression).WithOne(o => o.LogItem).HasForeignKey<TEntity>(o => o.LogItemId);

        modelBuilder.Entity<LogItem>(b =>
        {
            b.HasMany(o => o.Files).WithOne(o => o.LogItem).HasForeignKey(o => o.LogItemId);

            WithForeignKeyReference(b, o => o.ApiRequest);
            WithForeignKeyReference(b, o => o.LogMessage);
            WithForeignKeyReference(b, o => o.DeletedEmote);
            WithForeignKeyReference(b, o => o.Unban);
            WithForeignKeyReference(b, o => o.Job);
            WithForeignKeyReference(b, o => o.ChannelCreated);
            WithForeignKeyReference(b, o => o.ChannelDeleted);
            WithForeignKeyReference(b, o => o.ChannelUpdated);
            WithForeignKeyReference(b, o => o.GuildUpdated);
            WithForeignKeyReference(b, o => o.MessageDeleted);
            WithForeignKeyReference(b, o => o.MessageEdited);
            WithForeignKeyReference(b, o => o.OverwriteCreated);
            WithForeignKeyReference(b, o => o.OverwriteUpdated);
            WithForeignKeyReference(b, o => o.OverwriteDeleted);
            WithForeignKeyReference(b, o => o.UserJoined);
            WithForeignKeyReference(b, o => o.UserLeft);
            WithForeignKeyReference(b, o => o.InteractionCommand);
            WithForeignKeyReference(b, o => o.ThreadDeleted);
            WithForeignKeyReference(b, o => o.MemberUpdated);
            WithForeignKeyReference(b, o => o.RoleDeleted);

            b.HasMany(o => o.MemberRolesUpdated).WithOne(o => o.LogItem).HasForeignKey(o => o.LogItemId);
        });

        modelBuilder.Entity<ChannelCreated>(b => b.HasOne(o => o.ChannelInfo).WithMany().HasForeignKey(o => o.ChannelInfoId));
        modelBuilder.Entity<ChannelDeleted>(b => b.HasOne(o => o.ChannelInfo).WithMany().HasForeignKey(o => o.ChannelInfoId));
        modelBuilder.Entity<ChannelUpdated>(b =>
        {
            b.HasOne(o => o.Before).WithMany().HasForeignKey(o => o.BeforeId);
            b.HasOne(o => o.After).WithMany().HasForeignKey(o => o.AfterId);
        });

        modelBuilder.Entity<GuildUpdated>(b =>
        {
            b.HasOne(o => o.Before).WithMany().HasForeignKey(o => o.BeforeId);
            b.HasOne(o => o.After).WithMany().HasForeignKey(o => o.AfterId);
        });

        modelBuilder.Entity<MessageDeleted>(b => b.HasMany(o => o.Embeds).WithOne(o => o.MessageDeleted).HasForeignKey(o => o.MessageDeletedId));
        modelBuilder.Entity<EmbedInfo>(b => b.HasMany(o => o.Fields).WithOne(o => o.EmbedInfo).HasForeignKey(o => o.EmbedInfoId));

        modelBuilder.Entity<OverwriteCreated>(b => b.HasOne(o => o.OverwriteInfo).WithMany().HasForeignKey(o => o.OverwriteInfoId));
        modelBuilder.Entity<OverwriteDeleted>(b => b.HasOne(o => o.OverwriteInfo).WithMany().HasForeignKey(o => o.OverwriteInfoId));
        modelBuilder.Entity<OverwriteUpdated>(b =>
        {
            b.HasOne(o => o.Before).WithMany().HasForeignKey(o => o.BeforeId);
            b.HasOne(o => o.After).WithMany().HasForeignKey(o => o.AfterId);
        });

        modelBuilder.Entity<ThreadDeleted>(b => b.HasOne(o => o.ThreadInfo).WithMany().HasForeignKey(o => o.ThreadInfoId));
        modelBuilder.Entity<ThreadUpdated>(b =>
        {
            b.HasOne(o => o.Before).WithMany().HasForeignKey(o => o.BeforeId);
            b.HasOne(o => o.After).WithMany().HasForeignKey(o => o.AfterId);
        });

        modelBuilder.Entity<MemberUpdated>(b =>
        {
            b.HasOne(o => o.Before).WithMany().HasForeignKey(o => o.BeforeId);
            b.HasOne(o => o.After).WithMany().HasForeignKey(o => o.AfterId);
        });

        modelBuilder.Entity<ApiRequest>(b =>
        {
            b.HasIndex(x => x.Role).HasFilter("\"Role\" IS NOT NULL");

            b.Property(o => o.Headers).HasConversion(new JsonValueConverter<Dictionary<string, string>>());
            b.Property(o => o.Parameters).HasConversion(new JsonValueConverter<Dictionary<string, string>>());
        });

        modelBuilder.Entity<RoleDeleted>(b => b.HasOne(o => o.RoleInfo).WithMany().HasForeignKey(o => o.RoleInfoId));
        modelBuilder.Entity<InteractionCommand>(b => b.Property(o => o.Parameters).HasConversion(new JsonValueConverter<List<InteractionCommandParameter>>()));
        modelBuilder.Entity<RoleInfo>(b => b.Property(o => o.Permissions).HasConversion(new JsonValueConverter<List<string>>()));
        modelBuilder.Entity<ThreadInfo>(b => b.Property(o => o.Tags).HasConversion(new JsonValueConverter<List<string>>()));
    }

    public DbSet<ApiRequest> ApiRequests => Set<ApiRequest>();
    public DbSet<ChannelCreated> ChannelCreatedItems => Set<ChannelCreated>();
    public DbSet<ChannelDeleted> ChannelDeletedItems => Set<ChannelDeleted>();
    public DbSet<ChannelInfo> ChannelInfoItems => Set<ChannelInfo>();
    public DbSet<ChannelUpdated> ChannelUpdatedItems => Set<ChannelUpdated>();
    public DbSet<DeletedEmote> DeletedEmotes => Set<DeletedEmote>();
    public DbSet<EmbedField> EmbedFields => Set<EmbedField>();
    public DbSet<EmbedInfo> EmbedInfoItems => Set<EmbedInfo>();
    public DbSet<File> Files => Set<File>();
    public DbSet<GuildInfo> GuildInfoItems => Set<GuildInfo>();
    public DbSet<GuildUpdated> GuildUpdatedItems => Set<GuildUpdated>();
    public DbSet<InteractionCommand> InteractionCommands => Set<InteractionCommand>();
    public DbSet<JobExecution> JobExecutions => Set<JobExecution>();
    public DbSet<LogItem> LogItems => Set<LogItem>();
    public DbSet<LogMessage> LogMessages => Set<LogMessage>();
    public DbSet<MemberRoleUpdated> MemberRoleUpdatedItems => Set<MemberRoleUpdated>();
    public DbSet<MessageDeleted> MessageDeletedItems => Set<MessageDeleted>();
    public DbSet<MessageEdited> MessageEditedItems => Set<MessageEdited>();
    public DbSet<OverwriteCreated> OverwriteCreatedItems => Set<OverwriteCreated>();
    public DbSet<OverwriteDeleted> OverwriteDeletedItems => Set<OverwriteDeleted>();
    public DbSet<OverwriteInfo> OverwriteInfoItems => Set<OverwriteInfo>();
    public DbSet<OverwriteUpdated> OverwriteUpdatedItems => Set<OverwriteUpdated>();
    public DbSet<ThreadDeleted> ThreadDeletedItems => Set<ThreadDeleted>();
    public DbSet<ThreadInfo> ThreadInfoItems => Set<ThreadInfo>();
    public DbSet<ThreadUpdated> ThreadUpdatedItems => Set<ThreadUpdated>();
    public DbSet<Unban> Unbans => Set<Unban>();
    public DbSet<UserJoined> UserJoinedItems => Set<UserJoined>();
    public DbSet<UserLeft> UserLeftItems => Set<UserLeft>();
    public DbSet<MemberInfo> MemberInfos => Set<MemberInfo>();
    public DbSet<MemberUpdated> MemberUpdatedItems => Set<MemberUpdated>();
    public DbSet<RoleInfo> RoleInfos => Set<RoleInfo>();
    public DbSet<RoleDeleted> RoleDeleted => Set<RoleDeleted>();
}
