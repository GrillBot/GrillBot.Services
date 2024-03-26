using ControllerBase = GrillBot.Core.Infrastructure.Actions.ControllerBase;
using Microsoft.AspNetCore.Mvc;
using GrillBot.Core.Validation;
using System.ComponentModel.DataAnnotations;
using EmoteService.Actions.Statistics;
using EmoteService.Models.Request;
using GrillBot.Core.Models.Pagination;
using EmoteService.Models.Response;

namespace EmoteService.Controllers;

public class StatisticsController : ControllerBase
{
    public StatisticsController(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }

    [HttpDelete("{guildId}/{emoteId}")]
    [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public Task<IActionResult> DeleteStatisticsAsync(
        [DiscordId, StringLength(32)] string guildId,
        [EmoteId, StringLength(255)] string emoteId,
        [DiscordId, StringLength(32), FromQuery] string? userId = null
    ) => ProcessAsync<DeleteStatisticsAction>(guildId, emoteId, userId);

    [HttpPut("{guildId}/{sourceEmoteId}/{destinationEmoteId}/merge")]
    [ProducesResponseType(typeof(MergeStatisticsResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public Task<IActionResult> MergeStatisticsAsync(
        [DiscordId, StringLength(32)] string guildId,
        [EmoteId, StringLength(255)] string sourceEmoteId,
        [EmoteId, StringLength(255)] string destinationEmoteId
    ) => ProcessAsync<MergeStatisticsAction>(guildId, sourceEmoteId, destinationEmoteId);

    [HttpPost("emoteUsersUsage")]
    [ProducesResponseType(typeof(PaginatedResponse<EmoteUserUsageItem>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public Task<IActionResult> GetUserEmoteUsageListAsync([FromBody] EmoteUserUsageListRequest request)
        => ProcessAsync<GetUserEmoteUsageListAction>(request);

    [HttpPost("emoteStatistics")]
    [ProducesResponseType(typeof(PaginatedResponse<EmoteStatisticsItem>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public Task<IActionResult> GetEmoteStatisticsListAsync([FromBody] EmoteStatisticsListRequest request)
        => ProcessAsync<GetEmoteStatisticsListAction>(request);
}
