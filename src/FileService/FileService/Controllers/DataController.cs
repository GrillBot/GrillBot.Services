using System.ComponentModel.DataAnnotations;
using FileService.Actions;
using Microsoft.AspNetCore.Mvc;
using ControllerBase = GrillBot.Core.Infrastructure.Actions.ControllerBase;

namespace FileService.Controllers;

[ApiController]
[Route("api/data")]
public class DataController : ControllerBase
{
    public DataController(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }

    [HttpPost]
    public Task<IActionResult> UploadFileAsync(IFormFile file)
        => ProcessAsync<UploadFileAction>(file);

    [HttpGet]
    public Task<IActionResult> DownloadFileAsync([Required] string filename)
        => ProcessAsync<DownloadFileAction>(filename);

    [HttpGet("link")]
    public Task<IActionResult> GenerateLinkAsync([Required] string filename)
        => ProcessAsync<GenerateLinkAction>(filename);

    [HttpDelete]
    public Task<IActionResult> DeleteAsync([Required] string filename)
        => ProcessAsync<DeleteFileAction>(filename);
}
