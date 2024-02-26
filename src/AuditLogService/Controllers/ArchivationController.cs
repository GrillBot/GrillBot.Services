using AuditLogService.Actions.Archivation;
using AuditLogService.Models.Response;
using Microsoft.AspNetCore.Mvc;
using ControllerBase = GrillBot.Core.Infrastructure.Actions.ControllerBase;

namespace AuditLogService.Controllers;

public class ArchivationController : ControllerBase
{
    public ArchivationController(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }

    [HttpPost]
    [ProducesResponseType(typeof(ArchivationResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> CreateArchivationDataAsync()
        => await ProcessAsync<CreateArchivationDataAction>();
}
