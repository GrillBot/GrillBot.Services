using AuditLogService.Actions.Info;
using Microsoft.AspNetCore.Mvc;
using ControllerBase = GrillBot.Core.Infrastructure.Actions.ControllerBase;

namespace AuditLogService.Controllers;

public class InfoController : ControllerBase
{
    public InfoController(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }

    [HttpGet("jobs")]
    public async Task<IActionResult> GetJobsInfoAsync()
        => await ProcessAsync<GetJobsInfoAction>();
}
