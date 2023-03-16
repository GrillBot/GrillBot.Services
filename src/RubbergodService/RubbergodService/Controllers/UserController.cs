using Microsoft.AspNetCore.Mvc;
using RubbergodService.MemberSynchronization;

namespace RubbergodService.Controllers;

[ApiController]
[Route("api/user")]
public class UserController : Controller
{
    private MemberSyncQueue MemberSyncQueue { get; }

    public UserController(MemberSyncQueue memberSyncQueue)
    {
        MemberSyncQueue = memberSyncQueue;
    }

    [HttpPatch("{memberId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult> RefreshUserAsync(string memberId)
    {
        await MemberSyncQueue.AddToQueueAsync(memberId);
        return Ok();
    }
}
