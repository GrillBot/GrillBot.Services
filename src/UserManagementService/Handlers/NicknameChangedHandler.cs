using GrillBot.Core.Extensions;
using GrillBot.Core.Infrastructure.Auth;
using GrillBot.Core.RabbitMQ.V2.Consumer;
using GrillBot.Core.Services.AuditLog.Enums;
using GrillBot.Core.Services.AuditLog.Models.Events.Create;
using GrillBot.Services.Common.Discord;
using GrillBot.Services.Common.Infrastructure.RabbitMQ;
using System.Text;
using UserManagementService.Core.Entity;
using UserManagementService.Models.Events;

namespace UserManagementService.Handlers;

public class NicknameChangedHandler(
    IServiceProvider serviceProvider,
    DiscordManager _discordManager
) : BaseEventHandlerWithDb<NicknameChangedMessage, UserManagementContext>(serviceProvider)
{
    protected override async Task<RabbitConsumptionResult> HandleInternalAsync(
        NicknameChangedMessage message,
        ICurrentUserProvider currentUser,
        Dictionary<string, string> headers
    )
    {
        if (message.NicknameBefore == message.NicknameAfter)
            return RabbitConsumptionResult.Success;

        var user = await _discordManager.GetUserAsync(message.UserId);
        if (user is null)
            return RabbitConsumptionResult.Reject;

        var isUser = !(user.IsBot || user.IsWebhook);
        message.NicknameBefore = SanitizeNickname(message.NicknameBefore, isUser);
        message.NicknameAfter = SanitizeNickname(message.NicknameAfter, isUser);

        await UpdateNicknameHistoryAsync(message.GuildId, message.UserId, message.NicknameBefore);
        await UpdateNicknameHistoryAsync(message.GuildId, message.UserId, message.NicknameAfter);
        await UpdateCurrentNicknameAsync(message);
        await ContextHelper.SaveChangesAsync();
        await NotifyAuditLogAsync(message);

        return RabbitConsumptionResult.Success;
    }

    private async Task UpdateNicknameHistoryAsync(ulong guildId, ulong userId, string? nickname)
    {
        if (string.IsNullOrEmpty(nickname))
            return;

        var existsQuery = DbContext.Nicknames.Where(o => o.UserId == userId && o.GuildId == guildId && o.Value == nickname);
        if (await ContextHelper.IsAnyAsync(existsQuery))
            return;

        await DbContext.AddAsync(new GuildUserNickname
        {
            GuildId = guildId,
            UserId = userId,
            Value = nickname
        });
    }

    private async Task UpdateCurrentNicknameAsync(NicknameChangedMessage message)
    {
        var guildUserQuery = DbContext.GuildUsers.Where(o => o.GuildId == message.GuildId && o.UserId == message.UserId);
        var guildUser = await ContextHelper.ReadFirstOrDefaultEntityAsync(guildUserQuery);

        if (guildUser is null)
        {
            guildUser = new GuildUser
            {
                GuildId = message.GuildId,
                UserId = message.UserId,
            };

            await DbContext.AddAsync(guildUser);
        }

        guildUser.CurrentNickname = message.NicknameAfter;
    }

    private Task NotifyAuditLogAsync(NicknameChangedMessage message)
    {
        var logRequest = new LogRequest(LogType.MemberUpdated, DateTime.UtcNow, message.GuildId.ToString())
        {
            MemberUpdated = new MemberUpdatedRequest { UserId = message.UserId.ToString() }
        };

        return Publisher.PublishAsync(new CreateItemsMessage(logRequest));
    }

    private static string? SanitizeNickname(string? nickname, bool isUser)
    {
        if (string.IsNullOrEmpty(nickname))
            return null;

        var builder = new StringBuilder();
        var nicknameValue = isUser ? nickname : nickname.Cut(32, true)!;

        foreach (var character in nicknameValue.Where(ch => Rune.TryCreate(ch, out _)))
            builder.Append(character);

        return builder.ToString();
    }
}
