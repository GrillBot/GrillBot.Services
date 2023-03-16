using System.ComponentModel.DataAnnotations;
using FileService.Managers;
using Microsoft.AspNetCore.Mvc;

namespace FileService.Controllers;

[ApiController]
[Route("api/data")]
public class DataController : Controller
{
    private StorageManager StorageManager { get; }

    public DataController(StorageManager storageManager)
    {
        StorageManager = storageManager;
    }

    [HttpPost]
    public async Task<ActionResult> StoreFileAsync(IFormFile file)
        => StatusCode(await StorageManager.UploadFileAsync(file));

    [HttpGet]
    public async Task<ActionResult> DownloadFileAsync([Required] string filename)
    {
        var result = await StorageManager.DownloadFileAsync(filename);
        if (result == null)
            return NotFound();

        return File(result.Value.content, result.Value.contentType, filename);
    }

    [HttpGet("link")]
    public ActionResult<string> GenerateLink([Required] string filename)
    {
        var result = StorageManager.GenerateLink(filename);
        if (result == null)
            return NotFound();

        return Ok(result);
    }

    [HttpDelete]
    public async Task<ActionResult> DeleteAsync([Required] string filename)
    {
        await StorageManager.DeleteAsync(filename);
        return Ok();
    }
}
