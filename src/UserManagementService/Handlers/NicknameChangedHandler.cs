using GrillBot.Core.Infrastructure.Auth;
using GrillBot.Core.RabbitMQ.V2.Consumer;
using GrillBot.Core.Services.AuditLog.Enums;
using GrillBot.Core.Services.AuditLog.Models.Events.Create;
using GrillBot.Services.Common.Infrastructure.RabbitMQ;
using UserManagementService.Core.Entity;
using UserManagementService.Models.Events;

namespace UserManagementService.Handlers;

public class NicknameChangedHandler(
    IServiceProvider serviceProvider
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
}
