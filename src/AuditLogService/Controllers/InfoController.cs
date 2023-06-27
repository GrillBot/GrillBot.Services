using AuditLogService.Actions.Info;
using AuditLogService.Models.Response.Info;
using Microsoft.AspNetCore.Mvc;
using ControllerBase = GrillBot.Core.Infrastructure.Actions.ControllerBase;

namespace AuditLogService.Controllers;

public class InfoController : ControllerBase
{
    public InfoController(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }

    [HttpGet("jobs")]
    [ProducesResponseType(typeof(List<JobInfo>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetJobsInfoAsync()
        => await ProcessAsync<GetJobsInfoAction>();

    [HttpGet("guild/{guildId}/count")]
    [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetItemsCountOfGuildAsync(string guildId)
        => await ProcessAsync<GetItemsCountOfGuildAction>(guildId);
}
