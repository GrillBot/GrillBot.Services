using GrillBot.Core.Extensions.Discord;
using GrillBot.Core.Infrastructure.Actions;
using GrillBot.Models;
using GrillBot.Services.Common.Discord;
using GrillBot.Services.Common.Infrastructure.Api;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using UnverifyService.Core.Entity;
using UnverifyService.Models.Request;
using UnverifyService.Options;

namespace UnverifyService.Actions;

public class CheckUnverifyRequirementsAction(
    IServiceProvider serviceProvider,
    IOptions<AppOptions> _options,
    DiscordManager _discordManager,
    IWebHostEnvironment _environment
) : ApiAction<UnverifyContext>(serviceProvider)
{
    public override async Task<ApiResult> ProcessAsync()
    {
        var request = GetParameter<UnverifyRequest>(0);

        var errorResponse = await ValidateKeepablesAsync(request);
        errorResponse ??= await ValidateGuildOwnerAsync(request);
        errorResponse ??= await ValidateAdministratorsAsync(request);
        errorResponse ??= await ValidateActiveUnverifyAsync(request);
        errorResponse ??= await ValidateUnverifyEnd(request);
        errorResponse ??= await ValidateUserRoles(request);
        errorResponse ??= ValidateReason(request);

        return errorResponse is null ?
            ApiResult.Ok() :
            new ApiResult(StatusCodes.Status400BadRequest, errorResponse);
    }

    private async Task<LocalizedMessageContent?> ValidateKeepablesAsync(UnverifyRequest request)
    {
        if (request.RequiredKeepables.Count == 0)
            return null;

        if (request.RequiredKeepables.Count > _options.Value.MaxAllowedKeepables)
            return new LocalizedMessageContent("Unverify/Validation/KeepableCountExceeded", [_options.Value.MaxAllowedKeepables.ToString()]);

        var requiredKeepables = request.RequiredKeepables.Select(o => o.ToUpper()).ToList();
        var keepablesQuery = DbContext.SelfUnverifyKeepables
            .AsNoTracking()
            .Where(o => requiredKeepables.Contains(o.Group.ToUpper()) || requiredKeepables.Contains(o.Name.ToUpper()))
            .GroupBy(o => o.Group.ToUpper());

        var definitions = await ContextHelper.ReadToDictionaryAsync(keepablesQuery, g => g.Key, g => g.Select(x => x.Name.ToUpper()).ToList(), CancellationToken);

        foreach (var keepable in request.RequiredKeepables.Select(o => o.ToUpper()))
        {
            var existsInDefinition = definitions.ContainsKey(keepable) || definitions.Values.Any(g => g.Contains(keepable));
            if (!existsInDefinition)
                return new LocalizedMessageContent("Unverify/Validation/UndefinedKeepable", [keepable]);
        }

        return null;
    }

    private async Task<LocalizedMessageContent?> ValidateGuildOwnerAsync(UnverifyRequest request)
    {
        var guildOwner = await _discordManager.GetGuildOwnerAsync(request.GuildId, CancellationToken);
        if (guildOwner is null)
            return new LocalizedMessageContent("Unverify/Validation/UnknownGuild", [request.GuildId.ToString()]);

        return guildOwner.Id == request.UserId
            ? new LocalizedMessageContent("Unverify/Validation/GuildOwner", [guildOwner.GetFullName()])
            : null;
    }

    private async Task<LocalizedMessageContent?> ValidateAdministratorsAsync(UnverifyRequest request)
    {
        if (_environment.IsDevelopment() || request.IsSelfUnverify)
            return null;

        var guild = await _discordManager.GetGuildAsync(request.GuildId, cancellationToken: CancellationToken);
        if (guild is null)
            return new LocalizedMessageContent("Unverify/Validation/UnknownGuild", [request.GuildId.ToString()]);

        var userQuery = DbContext.Users.AsNoTracking().Where(o => o.Id == request.UserId).Select(o => o.IsBotAdmin);
        var isBotAdmin = await ContextHelper.ReadFirstOrDefaultEntityAsync(userQuery, CancellationToken);

        var userEntity = (await _discordManager.GetGuildUserAsync(request.GuildId, request.UserId, CancellationToken))!;
        if (isBotAdmin || userEntity.GuildPermissions.Administrator)
            return new LocalizedMessageContent("Unverify/Validation/Administrator", [userEntity.GetFullName()]);

        return null;
    }

    private async Task<LocalizedMessageContent?> ValidateActiveUnverifyAsync(UnverifyRequest request)
    {
        var unverifyQuery = DbContext.ActiveUnverifies.AsNoTracking().Where(o => o.LogItem.GuildId == request.GuildId && o.LogItem.ToUserId == request.UserId);
        var activeUnverify = await ContextHelper.ReadFirstOrDefaultEntityAsync(unverifyQuery, CancellationToken);
        if (activeUnverify is null)
            return null;

        var userEntity = (await _discordManager.GetGuildUserAsync(request.GuildId, request.UserId, CancellationToken))!;
        return new LocalizedMessageContent("Unverify/Validation/MultipleUnverify", [userEntity.GetFullName(), $"DateTime:{activeUnverify.EndAtUtc:o}"]);
    }

    private async Task<LocalizedMessageContent?> ValidateUnverifyEnd(UnverifyRequest request)
    {
        var diff = request.EndAtUtc - DateTime.UtcNow.AddSeconds(-10); // Add 10 seconds tolerance.

        if (diff.TotalMinutes < 0)
            return new LocalizedMessageContent("Unverify/Validation/MustBeInFuture", []);

        if (request.IsSelfUnverify)
        {
            var userQuery = DbContext.Users.AsNoTracking().Where(o => o.Id == request.UserId).Select(o => o.SelfUnverifyMinimalTime);
            var minimalTime = await ContextHelper.ReadFirstOrDefaultEntityAsync(userQuery, CancellationToken);
            minimalTime ??= _options.Value.SelfUnverifyMinimalTime;

            if (diff < minimalTime)
                return new LocalizedMessageContent("Unverify/Validation/MinimalTime", [_options.Value.UnverifyMinimalTime.ToString("c")]);
        }

        if (diff < _options.Value.UnverifyMinimalTime)
            return new LocalizedMessageContent("Unverify/Validation/MinimalTime", [_options.Value.UnverifyMinimalTime.ToString("c")]);

        return null;
    }

    private async Task<LocalizedMessageContent?> ValidateUserRoles(UnverifyRequest request)
    {
        if (request.IsSelfUnverify)
            return null;

        var botUser = await _discordManager.GetGuildUserAsync(request.GuildId, _discordManager.CurrentUser.Id, CancellationToken);
        if (botUser is null)
            return null;

        var botRolePosition = botUser.GetRoles().Max(o => o.Position);

        var discordUser = await _discordManager.GetGuildUserAsync(request.GuildId, request.UserId, CancellationToken);
        if (discordUser is null)
            return new LocalizedMessageContent("Unverify/Validation/UnknownUser", [request.UserId.ToString()]);

        var userRoles = discordUser.GetRoles().ToList();
        if (userRoles.Count == 0 || userRoles.Max(o => o.Position) <= botRolePosition)
            return null;

        return new LocalizedMessageContent(
            "Unverify/Validation/HigherRoles",
            [
                discordUser.GetFullName(),
                string.Join(", ", userRoles.Where(o => o.Position > botRolePosition).Select(o => o.Name))
            ]
        );
    }

    private static LocalizedMessageContent? ValidateReason(UnverifyRequest request)
    {
        return string.IsNullOrEmpty((request.Reason ?? "").Trim()) && !request.IsSelfUnverify ?
            new LocalizedMessageContent("Unverify/Validation/UnverifyWithoutReason", []) :
            null;
    }
}
