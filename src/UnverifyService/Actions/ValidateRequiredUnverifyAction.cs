using GrillBot.Core.Infrastructure.Actions;
using GrillBot.Core.Services.GrillBot.Models;
using GrillBot.Services.Common.Discord;
using GrillBot.Services.Common.Extensions;
using GrillBot.Services.Common.Infrastructure.Api;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using UnverifyService.Core.Entity;
using UnverifyService.Models.Request;
using UnverifyService.Options;

namespace UnverifyService.Actions;

public class ValidateRequiredUnverifyAction(
    IServiceProvider serviceProvider,
    IOptions<AppOptions> _options,
    DiscordManager _discordManager,
    IWebHostEnvironment _environment
) : ApiAction<UnverifyContext>(serviceProvider)
{
    public override async Task<ApiResult> ProcessAsync()
    {
        var request = GetParameter<UnverifyRequest>(0);

        var errorResponse = ValidateRequestUsers(request);
        errorResponse ??= await ValidateKeepablesAsync(request);
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

    private static LocalizedMessageResponse? ValidateRequestUsers(UnverifyRequest request)
    {
        var usersCount = request.Users.Values.SelectMany(o => o).Distinct().Count();
        return usersCount == 0 ? new LocalizedMessageResponse("Unverify/Validation/NoUsers", []) : null;
    }

    private async Task<LocalizedMessageResponse?> ValidateKeepablesAsync(UnverifyRequest request)
    {
        if (request.RequiredKeepables.Count == 0)
            return null;

        if (request.RequiredKeepables.Count > _options.Value.MaxAllowedKeepables)
            return new LocalizedMessageResponse("Unverify/Validation/KeepableCountExceeded", [_options.Value.MaxAllowedKeepables.ToString()]);

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
                return new LocalizedMessageResponse("Unverify/Validation/UndefinedKeepable", [keepable]);
        }

        return null;
    }

    private async Task<LocalizedMessageResponse?> ValidateGuildOwnerAsync(UnverifyRequest request)
    {
        foreach (var guildGroup in request.Users)
        {
            var guildOwner = await _discordManager.GetGuildOwnerAsync(guildGroup.Key, CancellationToken);
            if (guildOwner is null)
                return new LocalizedMessageResponse("Unverify/Validation/UnknownGuild", [guildGroup.Key.ToString()]);

            if (guildGroup.Value.Contains(guildOwner.Id))
                return new LocalizedMessageResponse("Unverify/Validation/GuildOwner", [guildOwner.GetFullName()]);
        }

        return null;
    }

    private async Task<LocalizedMessageResponse?> ValidateAdministratorsAsync(UnverifyRequest request)
    {
        if (_environment.IsDevelopment() || request.IsSelfUnverify)
            return null;

        foreach (var guildGroup in request.Users)
        {
            var guild = await _discordManager.GetGuildAsync(guildGroup.Key, cancellationToken: CancellationToken);
            if (guild is null)
                return new LocalizedMessageResponse("Unverify/Validation/UnknownGuild", [guildGroup.Key.ToString()]);

            foreach (var userId in guildGroup.Value)
            {
                var userQuery = DbContext.Users.AsNoTracking().Where(o => o.Id == userId).Select(o => o.IsBotAdmin);
                var isBotAdmin = await ContextHelper.ReadFirstOrDefaultEntityAsync(userQuery, CancellationToken);

                var userEntity = (await _discordManager.GetGuildUserAsync(guildGroup.Key, userId, CancellationToken))!;
                if (isBotAdmin || userEntity.GuildPermissions.Administrator)
                    return new LocalizedMessageResponse("Unverify/Validation/Administrator", [userEntity.GetFullName()]);
            }
        }

        return null;
    }

    private async Task<LocalizedMessageResponse?> ValidateActiveUnverifyAsync(UnverifyRequest request)
    {
        foreach (var guildGroup in request.Users)
        {
            foreach (var userId in guildGroup.Value)
            {
                var unverifyQuery = DbContext.ActiveUnverifies.AsNoTracking().Where(o => o.LogItem.GuildId == guildGroup.Key && o.LogItem.ToUserId == userId);
                var activeUnverify = await ContextHelper.ReadFirstOrDefaultEntityAsync(unverifyQuery, CancellationToken);
                if (activeUnverify is null)
                    continue;

                var userEntity = (await _discordManager.GetGuildUserAsync(guildGroup.Key, userId, CancellationToken))!;
                return new LocalizedMessageResponse("Unverify/Validation/MultipleUnverify", [userEntity.GetFullName(), $"DateTime:{activeUnverify.EndAtUtc:o}"]);
            }
        }

        return null;
    }

    private async Task<LocalizedMessageResponse?> ValidateUnverifyEnd(UnverifyRequest request)
    {
        var diff = request.EndAtUtc - DateTime.UtcNow.AddSeconds(-10); // Add 10 seconds tolerance.

        if (diff.TotalMinutes < 0)
            return new LocalizedMessageResponse("Unverify/Validation/MustBeInFuture", []);

        if (request.IsSelfUnverify)
        {
            foreach (var userId in request.Users.Values.SelectMany(o => o).Distinct())
            {
                var userQuery = DbContext.Users.AsNoTracking().Where(o => o.Id == userId).Select(o => o.SelfUnverifyMinimalTime);
                var minimalTime = await ContextHelper.ReadFirstOrDefaultEntityAsync(userQuery, CancellationToken);
                minimalTime ??= _options.Value.SelfUnverifyMinimalTime;

                if (diff < minimalTime)
                    return new LocalizedMessageResponse("Unverify/Validation/MinimalTime", [_options.Value.UnverifyMinimalTime.ToString("c")]);
            }
        }

        if (diff < _options.Value.UnverifyMinimalTime)
            return new LocalizedMessageResponse("Unverify/Validation/MinimalTime", [_options.Value.UnverifyMinimalTime.ToString("c")]);

        return null;
    }

    private async Task<LocalizedMessageResponse?> ValidateUserRoles(UnverifyRequest request)
    {
        if (request.IsSelfUnverify)
            return null;

        foreach (var guildGroup in request.Users)
        {
            var botUser = await _discordManager.GetGuildUserAsync(guildGroup.Key, _discordManager.CurrentUser.Id, CancellationToken);
            if (botUser is null)
                continue;

            var botRolePosition = botUser.GetRoles().Max(o => o.Position);

            foreach (var userId in guildGroup.Value)
            {
                var discordUser = await _discordManager.GetGuildUserAsync(guildGroup.Key, userId, CancellationToken);
                if (discordUser is null)
                    return new LocalizedMessageResponse("Unverify/Validation/UnknownUser", [userId.ToString()]);

                var userRoles = discordUser.GetRoles().ToList();
                if (userRoles.Count == 0 || userRoles.Max(o => o.Position) <= botRolePosition)
                    continue;

                return new LocalizedMessageResponse(
                    "Unverify/Validation/HigherRoles",
                    [
                        discordUser.GetFullName(),
                        string.Join(", ", userRoles.Where(o => o.Position > botRolePosition).Select(o => o.Name))
                    ]
                );
            }
        }

        return null;
    }

    private static LocalizedMessageResponse? ValidateReason(UnverifyRequest request)
    {
        return string.IsNullOrEmpty((request.Reason ?? "").Trim()) && !request.IsSelfUnverify ?
            new LocalizedMessageResponse("Unverify/Validation/UnverifyWithoutReason", []) :
            null;
    }
}
