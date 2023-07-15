using Microsoft.EntityFrameworkCore;

namespace AuditLogService.Core.Entity;

public class AuditLogServiceContext : DbContext
{
    public AuditLogServiceContext(DbContextOptions<AuditLogServiceContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<LogItem>(b =>
        {
            b.HasMany(o => o.Files).WithOne(o => o.LogItem).HasForeignKey(o => o.LogItemId);

            b.HasOne(o => o.ApiRequest).WithOne(o => o.LogItem).HasForeignKey<ApiRequest>(o => o.LogItemId);
            b.HasOne(o => o.LogMessage).WithOne(o => o.LogItem).HasForeignKey<LogMessage>(o => o.LogItemId);
            b.HasOne(o => o.DeletedEmote).WithOne(o => o.LogItem).HasForeignKey<DeletedEmote>(o => o.LogItemId);
            b.HasOne(o => o.Unban).WithOne(o => o.LogItem).HasForeignKey<Unban>(o => o.LogItemId);
            b.HasOne(o => o.Job).WithOne(o => o.LogItem).HasForeignKey<JobExecution>(o => o.LogItemId);
            b.HasOne(o => o.ChannelCreated).WithOne(o => o.LogItem).HasForeignKey<ChannelCreated>(o => o.LogItemId);
            b.HasOne(o => o.ChannelDeleted).WithOne(o => o.LogItem).HasForeignKey<ChannelDeleted>(o => o.LogItemId);
            b.HasOne(o => o.ChannelUpdated).WithOne(o => o.LogItem).HasForeignKey<ChannelUpdated>(o => o.LogItemId);
            b.HasOne(o => o.GuildUpdated).WithOne(o => o.LogItem).HasForeignKey<GuildUpdated>(o => o.LogItemId);
            b.HasMany(o => o.MemberRolesUpdated).WithOne(o => o.LogItem).HasForeignKey(o => o.LogItemId);
            b.HasOne(o => o.MessageDeleted).WithOne(o => o.LogItem).HasForeignKey<MessageDeleted>(o => o.LogItemId);
            b.HasOne(o => o.MessageEdited).WithOne(o => o.LogItem).HasForeignKey<MessageEdited>(o => o.LogItemId);
            b.HasOne(o => o.OverwriteCreated).WithOne(o => o.LogItem).HasForeignKey<OverwriteCreated>(o => o.LogItemId);
            b.HasOne(o => o.OverwriteUpdated).WithOne(o => o.LogItem).HasForeignKey<OverwriteUpdated>(o => o.LogItemId);
            b.HasOne(o => o.OverwriteDeleted).WithOne(o => o.LogItem).HasForeignKey<OverwriteDeleted>(o => o.LogItemId);
            b.HasOne(o => o.UserJoined).WithOne(o => o.LogItem).HasForeignKey<UserJoined>(o => o.LogItemId);
            b.HasOne(o => o.UserLeft).WithOne(o => o.LogItem).HasForeignKey<UserLeft>(o => o.LogItemId);
            b.HasOne(o => o.InteractionCommand).WithOne(o => o.LogItem).HasForeignKey<InteractionCommand>(o => o.LogItemId);
            b.HasOne(o => o.ThreadDeleted).WithOne(o => o.LogItem).HasForeignKey<ThreadDeleted>(o => o.LogItemId);
            b.HasOne(o => o.MemberUpdated).WithOne(o => o.LogItem).HasForeignKey<MemberUpdated>(o => o.LogItemId);
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
}
