using GrillBot.Core.Models.Pagination;
using MessageService.Actions.AutoReply;
using MessageService.Models.Request.AutoReply;
using MessageService.Models.Response.AutoReply;
using Microsoft.AspNetCore.Mvc;

namespace MessageService.Controllers;

public class AutoReplyController(IServiceProvider serviceProvider) : GrillBot.Core.Infrastructure.Actions.ControllerBase(serviceProvider)
{
    [HttpPost]
    [ProducesResponseType<AutoReplyDefinition>(StatusCodes.Status200OK)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    public Task<IActionResult> CreateAutoReplyDefinition([FromBody] AutoReplyDefinitionRequest request)
        => ProcessAsync<CreateAutoReplyDefinitionAction>(request);

    [HttpGet("{id:guid}")]
    [ProducesResponseType<AutoReplyDefinition>(StatusCodes.Status200OK)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public Task<IActionResult> GetAutoReplyDefinition([FromRoute] Guid id)
        => ProcessAsync<GetAutoReplyDefinitionAction>(id);

    [HttpPost("list")]
    [ProducesResponseType<PaginatedResponse<AutoReplyDefinition>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    public Task<IActionResult> GetAutoReplyDefinitionListAsync([FromBody] AutoReplyDefinitionListRequest request)
        => ProcessAsync<GetAutoReplyDefinitionListAction>(request);

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public Task<IActionResult> DeleteAutoReplyDefinitionAsync([FromRoute] Guid id)
        => ProcessAsync<DeleteAutoReplyDefinitionAction>(id);

    [HttpPut("{id:guid}")]
    [ProducesResponseType<AutoReplyDefinition>(StatusCodes.Status200OK)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public Task<IActionResult> UpdateAutoReplyDefinitionAsync([FromRoute] Guid id, [FromBody] AutoReplyDefinitionRequest request)
        => ProcessAsync<UpdateAutoReplyDefinitionAction>(id, request);
}
