using EmoteService.Core.Entity.Suggestions;
using GrillBot.Core.Database;
using Microsoft.EntityFrameworkCore;

namespace EmoteService.Core.Entity;

public class EmoteServiceContext(DbContextOptions<EmoteServiceContext> options) : GrillBotBaseContext(options)
{
    public DbSet<EmoteDefinition> EmoteDefinitions => Set<EmoteDefinition>();
    public DbSet<EmoteUserStatItem> EmoteUserStatItems => Set<EmoteUserStatItem>();
    public DbSet<EmoteSuggestion> EmoteSuggestions => Set<EmoteSuggestion>();
    public DbSet<EmoteUserVote> EmoteUserVotes => Set<EmoteUserVote>();
    public DbSet<EmoteVoteSession> EmoteVoteSessions => Set<EmoteVoteSession>();
    public DbSet<Guild> Guilds => Set<Guild>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<EmoteSuggestion>(
            builder => builder.HasOne(o => o.VoteSession).WithOne(o => o.Suggestion).HasForeignKey<EmoteVoteSession>(o => o.Id)
        );

        modelBuilder.Entity<EmoteVoteSession>(
            builder => builder.HasMany(o => o.UserVotes).WithOne(o => o.VoteSession).HasForeignKey(o => o.VoteSessionId)
        );
    }
}
