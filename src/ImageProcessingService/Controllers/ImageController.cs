using System.ComponentModel.DataAnnotations;
using ImageProcessingService.Actions;
using ImageProcessingService.Models;
using Microsoft.AspNetCore.Mvc;
using ControllerBase = GrillBot.Core.Infrastructure.Actions.ControllerBase;

namespace ImageProcessingService.Controllers;

public class ImageController(IServiceProvider serviceProvider) : ControllerBase(serviceProvider)
{
    [HttpPost("peepolove")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public Task<IActionResult> CreatePeepoloveImageAsync([Required, FromBody] PeepoRequest request)
        => ProcessAsync<PeepoLoveAction>(request);

    [HttpPost("peepoangry")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public Task<IActionResult> CreatePeepoangryImageAsync([Required, FromBody] PeepoRequest request)
        => ProcessAsync<PeepoAngryAction>(request);

    [HttpPost("points")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public Task<IActionResult> CreatePointsImageAsync([Required, FromBody] PointsRequest request)
        => ProcessAsync<PointsAction>(request);

    [HttpPost("without-accident")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public Task<IActionResult> CreateWithoutAccidentImageAsync([Required, FromBody] WithoutAccidentImageRequest request)
        => ProcessAsync<WithoutAccidentAction>(request);

    [HttpPost("chart")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public Task<IActionResult> CreateChartImageAsync([Required, FromBody] ChartRequest request)
        => ProcessAsync<ChartAction>(request);
}
