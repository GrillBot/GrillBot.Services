﻿using GrillBot.Core.Models.Pagination;
using Microsoft.AspNetCore.Mvc;
using PointsService.Actions;
using PointsService.Models;
using ControllerBase = GrillBot.Core.Infrastructure.Actions.ControllerBase;

namespace PointsService.Controllers;

public class AdminController : ControllerBase
{
    public AdminController(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }

    [HttpPost("list")]
    [ProducesResponseType(typeof(PaginatedResponse<TransactionItem>), StatusCodes.Status200OK)]
    public Task<IActionResult> GetTransactionListAsync([FromBody] AdminListRequest request)
        => ProcessAsync<AdminListAction>(request);

    [HttpPost("create")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public Task<IActionResult> CreateTransactionAsync([FromBody] AdminTransactionRequest request)
        => ProcessAsync<AdminCreateTransactionAction>(request);
}
