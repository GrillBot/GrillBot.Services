﻿using GrillBot.Core.Models.Pagination;
using Microsoft.AspNetCore.Mvc;
using UserMeasuresService.Models.Measures;

namespace UserMeasuresService.Controllers;

public class MeasuresController(IServiceProvider serviceProvider) : GrillBot.Core.Infrastructure.Actions.ControllerBase(serviceProvider)
{
    [HttpPost("list")]
    [ProducesResponseType(typeof(PaginatedResponse<MeasuresItem>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public Task<IActionResult> GetMeasuresListAsync(MeasuresListParams parameters)
        => ProcessAsync<Actions.Measures.GetMeasuresList>(parameters);

    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public Task<IActionResult> DeleteMeasureAsync([FromQuery] DeleteMeasuresRequest request)
        => ProcessAsync<Actions.Measures.DeleteMeasureAction>(request);
}
