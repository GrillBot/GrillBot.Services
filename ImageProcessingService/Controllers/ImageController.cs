using System.ComponentModel.DataAnnotations;
using ImageProcessingService.Actions;
using ImageProcessingService.Models;
using Microsoft.AspNetCore.Mvc;
using ControllerBase = GrillBot.Core.Infrastructure.Actions.ControllerBase;

namespace ImageProcessingService.Controllers;

public class ImageController : ControllerBase
{
    public ImageController(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }

    [HttpPost("peepolove")]
    public Task<IActionResult> CreatePeepoloveImageAsync([Required, FromBody] PeepoRequest request)
        => ProcessAsync<PeepoLoveAction>(request);

    [HttpPost("peepoangry")]
    public Task<IActionResult> CreatePeepoangryImageAsync([Required, FromBody] PeepoRequest request)
        => ProcessAsync<PeepoAngryAction>(request);
}
