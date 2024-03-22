using Microsoft.AspNetCore.Mvc;
using PointsService.Actions.Channels;
using PointsService.Models.Channels;
using ControllerBase = GrillBot.Core.Infrastructure.Actions.ControllerBase;

namespace PointsService.Controllers;

public class ChannelController : ControllerBase
{
    public ChannelController(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }

    [HttpGet("{guildId}/{channelId}/info")]
    [ProducesResponseType(typeof(ChannelInfo), StatusCodes.Status200OK)]
    public Task<IActionResult> GetChannelInfoAsync(string guildId, string channelId)
        => ProcessAsync<GetChannelInfoAction>(guildId, channelId);
}
