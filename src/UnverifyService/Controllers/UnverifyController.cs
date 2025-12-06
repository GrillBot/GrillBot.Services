using GrillBot.Core.Models.Pagination;
using GrillBot.Core.Validation;
using GrillBot.Models;
using GrillBot.Services.Common.Infrastructure.Api.OpenApi.Attributes;
using Microsoft.AspNetCore.Mvc;
using UnverifyService.Actions;
using UnverifyService.Models.Request;
using UnverifyService.Models.Response;

namespace UnverifyService.Controllers;

public class UnverifyController(IServiceProvider serviceProvider) : GrillBot.Core.Infrastructure.Actions.ControllerBase(serviceProvider)
{
    [HttpPost("list")]
    [ProducesResponseType<PaginatedResponse<ActiveUnverifyListItemResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    public Task<IActionResult> GetActiveUnverifyListAsync([FromBody] ActiveUnverifyListRequest request)
        => ProcessAsync<GetActiveUnverifyListAction>(request);

    [SwaggerRequireAuthorization]
    [HttpPost("list/current-user")]
    [ProducesResponseType<PaginatedResponse<ActiveUnverifyListItemResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public Task<IActionResult> GetCurrentUserUnverifyListAsync([FromBody] ActiveUnverifyListRequest request)
        => ProcessAsync<GetCurrentUserUnverifyListAction>(request);

    [HttpPost("validate")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType<LocalizedMessageContent>(StatusCodes.Status400BadRequest)]
    public Task<IActionResult> CheckUnverifyRequirementsAsync([FromBody] UnverifyRequest request)
        => ProcessAsync<CheckUnverifyRequirementsAction>(request);

    [SwaggerRequireAuthorization]
    [HttpDelete("{guildId}/{userId}")]
    [ProducesResponseType<RemoveUnverifyResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<RemoveUnverifyResponse>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    public Task<IActionResult> RemoveUnverifyAsync(
        [FromRoute, DiscordId] ulong guildId,
        [FromRoute, DiscordId] ulong userId,
        [FromQuery] bool isForceRemove
    ) => ProcessAsync<RemoveUnverifyAction>(guildId, userId, isForceRemove);

    [SwaggerRequireAuthorization]
    [HttpPut]
    [ProducesResponseType<LocalizedMessageContent>(StatusCodes.Status200OK)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<LocalizedMessageContent>(StatusCodes.Status404NotFound)]
    public Task<IActionResult> UpdateUnverifyAsync([FromBody] UpdateUnverifyRequest request)
        => ProcessAsync<UpdateUnverifyAction>(request);

    [HttpGet("to-remove")]
    [ProducesResponseType<List<ScheduleUnverifyRemoveItem>>(StatusCodes.Status200OK)]
    public Task<IActionResult> GetUnverifiesToRemoveAsync()
        => ProcessAsync<GetUnverifiesToRemoveAction>();

    [HttpGet("{guildId}/{userId}")]
    [ProducesResponseType<UnverifyDetail>(StatusCodes.Status200OK)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public Task<IActionResult> GetActiveUnverifyDetailAsync(
        [FromRoute, DiscordId] ulong guildId,
        [FromRoute, DiscordId] ulong userId
    ) => ProcessAsync<GetActiveUnverifyDetailAction>(guildId, userId);

    [HttpPost("recovery/validate")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    public Task<IActionResult> CheckRecoveryRequirementsAsync([FromQuery] Guid? logId, [FromQuery] long? logNumber)
        => ProcessAsync<CheckRecoveryRequirementsAction>(logId, logNumber);
}
