using GrillBot.Core.Models.Pagination;
using GrillBot.Core.Validation;
using Microsoft.AspNetCore.Mvc;
using RemindService.Actions;
using RemindService.Models.Request;
using RemindService.Models.Response;

namespace RemindService.Controllers;

public class RemindController : GrillBot.Core.Infrastructure.Actions.ControllerBase
{
    public RemindController(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }

    [HttpPost("process-pending")]
    [ProducesResponseType(typeof(ProcessPendingRemindersResult), StatusCodes.Status200OK)]
    public Task<IActionResult> ProcessPendingRemindersAsync()
        => ProcessAsync<ProcessPendingRemindersAction>();

    [HttpPut("cancel")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public Task<IActionResult> CancelReminderAsync([FromBody] CancelReminderRequest request)
        => ProcessAsync<CancelReminderAction>(request);

    [HttpPost("list")]
    [ProducesResponseType(typeof(PaginatedResponse<RemindMessageItem>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public Task<IActionResult> GetReminderListAsync([FromBody] ReminderListRequest request)
        => ProcessAsync<ReminderListAction>(request);

    [HttpPost("create")]
    [ProducesResponseType(typeof(CreateReminderResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public Task<IActionResult> CreateReminderAsync([FromBody] CreateReminderRequest request)
        => ProcessAsync<CreateReminderAction>(request);

    [HttpPost("copy")]
    [ProducesResponseType(typeof(CreateReminderResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status404NotFound)]
    public Task<IActionResult> CopyReminderAsync([FromBody] CopyReminderRequest request)
        => ProcessAsync<CopyReminderAction>(request);

    [HttpGet("suggestions/{userId}")]
    [ProducesResponseType(typeof(List<ReminderSuggestionItem>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public Task<IActionResult> GetSuggestionAsync([DiscordId] string userId)
        => ProcessAsync<ReminderSuggestionsAction>(userId);
}
